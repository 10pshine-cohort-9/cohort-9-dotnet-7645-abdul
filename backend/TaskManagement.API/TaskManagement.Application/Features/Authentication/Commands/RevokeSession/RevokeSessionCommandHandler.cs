using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler
    : IRequestHandler<RevokeSessionCommand, RevokeSessionResponse>
{
    private readonly ISessionService _sessionService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RevokeSessionCommandHandler> _logger;
    public RevokeSessionCommandHandler(
        ISessionService sessionService,
        IRefreshTokenService refreshTokenService,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        ILogger<RevokeSessionCommandHandler> logger)
    {
        _sessionService = sessionService;
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<RevokeSessionResponse> Handle(
      RevokeSessionCommand request,
      CancellationToken cancellationToken)
    {
        var userId =
            _userManager.GetUserId(_httpContextAccessor.HttpContext!.User);

        var session =
            await _sessionService.GetByIdAsync(request.SessionId);

        if (session == null)
        {
            _logger.LogWarning(
                "Session not found. SessionId: {SessionId}",
                request.SessionId);

            return new RevokeSessionResponse
            {
                Succeeded = false,
                Message = "Session not found."
            };
        }

        if (session.ApplicationUserId != userId)
        {
            _logger.LogWarning(
                "Unauthorized session revoke attempt. UserId: {UserId}, SessionId: {SessionId}",
                userId,
                request.SessionId);

            return new RevokeSessionResponse
            {
                Succeeded = false,
                Message = "Unauthorized."
            };
        }

        var refreshToken =
            await _refreshTokenService.GetByIdAsync(session.RefreshTokenId);

        if (refreshToken != null)
        {
            await _refreshTokenService.RevokeAsync(
                refreshToken,
                session.IpAddress ?? "Unknown",
                "Session terminated");

            _logger.LogInformation(
                "Refresh token revoked. TokenId: {TokenId}",
                refreshToken.Id);
        }

        await _sessionService.TerminateSessionAsync(session.Id);

        _logger.LogInformation(
            "Session terminated successfully. SessionId: {SessionId}, UserId: {UserId}",
            session.Id,
            userId);

        return new RevokeSessionResponse
        {
            Succeeded = true,
            Message = "Session terminated successfully."
        };
    }
}