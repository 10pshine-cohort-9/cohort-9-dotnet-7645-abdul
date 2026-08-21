using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Application.Features.Authentication.Commands.Logout;

public class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IClientInfoService _clientInfoService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IClientInfoService clientInfoService,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _clientInfoService = clientInfoService;
        _logger = logger;
    }

    public async Task<LogoutResponse> Handle(
     LogoutCommand request,
     CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshToken =
            await _refreshTokenService.GetByTokenAsync(request.RefreshToken);

        if (refreshToken == null)
        {
            _logger.LogWarning(
                "Logout failed. Invalid refresh token received.");

            return new LogoutResponse
            {
                Succeeded = false,
                Message = "Invalid refresh token."
            };
        }

        var client = _clientInfoService.GetSessionInfo();

        // Revoke refresh token
        await _refreshTokenService.RevokeAsync(
            refreshToken,
            client.IpAddress ?? "Unknown",
            "User logged out.");

        _logger.LogInformation(
            "Refresh token revoked. TokenId: {TokenId}",
            refreshToken.Id);

        // Find active session
        var sessions =
            await _sessionService.GetActiveSessionsAsync(
                refreshToken.ApplicationUserId);

        var currentSession =
            sessions.FirstOrDefault(x =>
                x.RefreshTokenId == refreshToken.Id);

        if (currentSession != null)
        {
            currentSession.IsActive = false;
            currentSession.LoggedOutAt = DateTime.UtcNow;
            currentSession.LastActivityAt = DateTime.UtcNow;

            await _sessionService.UpdateAsync(currentSession);

            _logger.LogInformation(
                "Session terminated. SessionId: {SessionId}",
                currentSession.Id);
        }
        else
        {
            _logger.LogWarning(
                "No active session found for RefreshTokenId: {RefreshTokenId}",
                refreshToken.Id);
        }

        _logger.LogInformation(
            "User {UserId} logged out successfully.",
            refreshToken.ApplicationUserId);

        return new LogoutResponse
        {
            Succeeded = true,
            Message = "Logout successful."
        };
    }
}