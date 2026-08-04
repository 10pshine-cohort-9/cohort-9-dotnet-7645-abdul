using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IApplicationUrlService _applicationUrlService;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
         IApplicationUrlService applicationUrlService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _applicationUrlService = applicationUrlService;
        _logger = logger;
    }

    public async Task<ForgotPasswordResponse> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var genericResponse = new ForgotPasswordResponse
        {
            Succeeded = true,
            Message =
                "If an account exists with this email, a password reset link has been sent."
        };

        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning(
                "Forgot password requested for non-existing email: {Email}",
                request.Email);

            return genericResponse;
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            _logger.LogWarning(
                "Forgot password requested for unverified email. UserId: {UserId}",
                user.Id);

            return genericResponse;
        }

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken =
            WebUtility.UrlEncode(token);

        var resetLink =
     $"{_applicationUrlService.FrontendBaseUrl}/reset-password" +
     $"?email={Uri.EscapeDataString(user.Email!)}" +
     $"&token={encodedToken}";

        await _emailService.SendPasswordResetEmailAsync(
            user.Email!,
            resetLink);

        _logger.LogInformation(
            "Password reset email sent to UserId: {UserId}",
            user.Id);

        return genericResponse;
    }
}