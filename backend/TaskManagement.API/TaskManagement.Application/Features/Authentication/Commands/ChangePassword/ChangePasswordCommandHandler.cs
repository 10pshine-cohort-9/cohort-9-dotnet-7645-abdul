using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ChangePasswordResponse> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.GetUserAsync(
                _httpContextAccessor.HttpContext!.User);

            if (user == null)
            {
                _logger.LogWarning(
                    "Password change attempted for an unknown authenticated user.");

                return new ChangePasswordResponse
                {
                    Succeeded = false,
                    Message = "User not found."
                };
            }

            if (!user.IsActive)
            {
                _logger.LogWarning(
                    "Inactive user {UserId} attempted to change password.",
                    user.Id);

                return new ChangePasswordResponse
                {
                    Succeeded = false,
                    Message = "Your account is inactive."
                };
            }

            _logger.LogInformation(
                "Password change requested for UserId {UserId}.",
                user.Id);

            var result = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(x => x.Description));

                _logger.LogWarning(
                    "Password change failed for UserId {UserId}. Errors: {Errors}",
                    user.Id,
                    errors);

                return new ChangePasswordResponse
                {
                    Succeeded = false,
                    Message = errors
                };
            }

            // Security: revoke every refresh token
            await _refreshTokenService.RevokeAllAsync(
                user.Id,
                "Password changed");

            // Security: terminate every active session
            await _sessionService.TerminateAllSessionsAsync(
                user.Id);

            _logger.LogInformation(
                "Password changed successfully for UserId {UserId}. All refresh tokens revoked and all sessions terminated.",
                user.Id);

            return new ChangePasswordResponse
            {
                Succeeded = true,
                Message = "Password changed successfully. Please log in again."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while changing password.");

            return new ChangePasswordResponse
            {
                Succeeded = false,
                Message = "An unexpected error occurred."
            };
        }
    }
}