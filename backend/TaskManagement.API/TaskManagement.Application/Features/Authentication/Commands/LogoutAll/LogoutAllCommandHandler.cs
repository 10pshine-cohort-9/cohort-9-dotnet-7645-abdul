using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Application.Features.Authentication.Commands.LogoutAll;

public class LogoutAllCommandHandler
    : IRequestHandler<LogoutAllCommand, LogoutAllResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IClientInfoService _clientInfoService;
    private readonly ILogger<LogoutAllCommandHandler> _logger;

    public LogoutAllCommandHandler(
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IClientInfoService clientInfoService,
        ILogger<LogoutAllCommandHandler> logger)
    {
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _clientInfoService = clientInfoService;
        _logger = logger;
    }

    public async Task<LogoutAllResponse> Handle(
     LogoutAllCommand request,
     CancellationToken cancellationToken)
    {
        var client = _clientInfoService.GetSessionInfo();

        // Get all active refresh tokens
        var refreshTokens =
            await _refreshTokenService.GetActiveTokensAsync(
                request.UserId);

        if (!refreshTokens.Any())
        {
            _logger.LogWarning(
                "Logout all requested but no active refresh tokens found for UserId: {UserId}",
                request.UserId);
        }

        // Revoke all refresh tokens
        foreach (var token in refreshTokens)
        {
            await _refreshTokenService.RevokeAsync(
                token,
                client.IpAddress ?? "Unknown",
                "Logout from all devices");
        }

        _logger.LogInformation(
            "All active refresh tokens revoked for UserId: {UserId}",
            request.UserId);

        // Terminate all sessions
        await _sessionService.TerminateAllSessionsAsync(
            request.UserId);

        _logger.LogInformation(
            "All sessions terminated for UserId: {UserId}",
            request.UserId);

        _logger.LogInformation(
            "User {UserId} logged out from all devices successfully.",
            request.UserId);

        return new LogoutAllResponse
        {
            Succeeded = true,
            Message = "Logged out from all devices successfully."
        };
    }
}