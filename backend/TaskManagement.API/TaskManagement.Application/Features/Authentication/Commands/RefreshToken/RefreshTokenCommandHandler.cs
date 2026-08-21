using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IClientInfoService _clientInfoService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IClientInfoService clientInfoService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _clientInfoService = clientInfoService;
        _logger = logger;
    }

    public async Task<RefreshTokenResponse> Handle(
      RefreshTokenCommand request,
      CancellationToken cancellationToken)
    {
        // Find refresh token (hash lookup happens inside the service)
        var storedToken =
            await _refreshTokenService.GetByTokenAsync(request.RefreshToken);

        if (storedToken == null)
        {
            _logger.LogWarning(
                "Invalid refresh token received.");

            return new RefreshTokenResponse
            {
                Succeeded = false,
                Message = "Invalid refresh token."
            };
        }

        // Validate refresh token
        if (!_refreshTokenService.Validate(storedToken))
        {
            _logger.LogWarning(
                "Expired or revoked refresh token. TokenId: {TokenId}",
                storedToken.Id);

            return new RefreshTokenResponse
            {
                Succeeded = false,
                Message = "Refresh token has expired or has been revoked."
            };
        }

        // Load associated user
        var user = storedToken.ApplicationUser;

        if (user == null)
        {
            _logger.LogWarning(
                "Refresh token belongs to a deleted user.");

            return new RefreshTokenResponse
            {
                Succeeded = false,
                Message = "User not found."
            };
        }

        // Check account status
        if (!user.IsActive)
        {
            _logger.LogWarning(
                "Refresh token attempt for inactive account. UserId: {UserId}",
                user.Id);

            return new RefreshTokenResponse
            {
                Succeeded = false,
                Message = "Your account is inactive."
            };
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate new access token
        var accessToken =
            await _jwtService.GenerateAccessTokenAsync(
                user.Id,
                user.UserName!,
                user.Email!,
                roles);

        // Get client information
        var client = _clientInfoService.GetSessionInfo();

        _logger.LogInformation(
            "Refreshing tokens for UserId: {UserId}",
            user.Id);

        // Rotate refresh token
        var refreshResult =
            await _refreshTokenService.RotateAsync(
                storedToken,
                client.IpAddress ?? "Unknown");

        // Update current session
        var sessions =
            await _sessionService.GetActiveSessionsAsync(user.Id);

        var currentSession =
            sessions.FirstOrDefault(x =>
                x.RefreshTokenId == storedToken.Id);

        if (currentSession != null)
        {
            currentSession.RefreshTokenId =
                refreshResult.RefreshTokenEntity.Id;

            currentSession.LastActivityAt = DateTime.UtcNow;

            currentSession.ExpiresAt =
                refreshResult.RefreshTokenEntity.ExpiresAt;

            await _sessionService.UpdateAsync(currentSession);

            _logger.LogInformation(
                "Session updated successfully. SessionId: {SessionId}",
                currentSession.Id);
        }
        else
        {
            _logger.LogWarning(
                "No active session found for RefreshTokenId: {RefreshTokenId}",
                storedToken.Id);
        }

        _logger.LogInformation(
            "Refresh token rotated successfully. UserId: {UserId}",
            user.Id);

        return new RefreshTokenResponse
        {
            Succeeded = true,
            Message = "Token refreshed successfully.",
            AccessToken = accessToken,
            RefreshToken = refreshResult.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}