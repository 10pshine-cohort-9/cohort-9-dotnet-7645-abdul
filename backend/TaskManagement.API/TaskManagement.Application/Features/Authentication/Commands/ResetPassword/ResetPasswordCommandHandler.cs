using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ISessionService _sessionService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenService refreshTokenService,
        ISessionService sessionService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task<ResetPasswordResponse> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Password reset attempted for unknown email: {Email}",
                    request.Email);

                return new ResetPasswordResponse
                {
                    Succeeded = false,
                    Message = "Invalid or expired password reset link."
                };
            }

            // Check account status
            if (!user.IsActive)
            {
                _logger.LogWarning(
                    "Inactive user attempted password reset. UserId: {UserId}",
                    user.Id);

                return new ResetPasswordResponse
                {
                    Succeeded = false,
                    Message = "Your account is inactive."
                };
            }

            // Optional security check
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning(
                    "Password reset attempted for unverified email. UserId: {UserId}",
                    user.Id);

                return new ResetPasswordResponse
                {
                    Succeeded = false,
                    Message = "Email address is not verified."
                };
            }

            _logger.LogInformation(
                "Password reset requested for UserId {UserId}.",
                user.Id);

            // Decode token
            var decodedToken = WebUtility.UrlDecode(request.Token);

            // Reset password
            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(x => x.Description));

                _logger.LogWarning(
                    "Password reset failed for UserId {UserId}. Errors: {Errors}",
                    user.Id,
                    errors);

                return new ResetPasswordResponse
                {
                    Succeeded = false,
                    Message = "Invalid or expired password reset link."
                };
            }

            // Security: revoke every refresh token
            await _refreshTokenService.RevokeAllAsync(
                user.Id,
                "Password reset");

            // Security: terminate every active session
            await _sessionService.TerminateAllSessionsAsync(
                user.Id);

            _logger.LogInformation(
                "Password reset completed successfully for UserId {UserId}. All refresh tokens revoked and all sessions terminated.",
                user.Id);

            return new ResetPasswordResponse
            {
                Succeeded = true,
                Message = "Password has been reset successfully. Please log in again."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error occurred while resetting password.");

            return new ResetPasswordResponse
            {
                Succeeded = false,
                Message = "An unexpected error occurred."
            };
        }
    }
}