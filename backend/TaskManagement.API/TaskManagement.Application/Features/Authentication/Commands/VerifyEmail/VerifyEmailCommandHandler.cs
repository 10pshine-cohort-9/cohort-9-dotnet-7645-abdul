using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailCommandHandler
    : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public VerifyEmailCommandHandler(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<VerifyEmailResponse> Handle(
     VerifyEmailCommand request,
     CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return new VerifyEmailResponse
            {
                Succeeded = false,
                Message = "User not found.",
                IsEmailVerified = false
            };
        }

        if (user.EmailConfirmed)
        {
            return new VerifyEmailResponse
            {
                Succeeded = true,
                Message = "Email is already verified.",
                IsEmailVerified = true
            };
        }

        var decodedToken = Uri.UnescapeDataString(request.Token);

        var result = await _userManager.ConfirmEmailAsync(
            user,
            decodedToken);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description
                ?? "Invalid or expired verification token.";

            return new VerifyEmailResponse
            {
                Succeeded = false,
                Message = error,
                IsEmailVerified = false
            };
        }

        user.EmailVerifiedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new VerifyEmailResponse
        {
            Succeeded = true,
            Message = "Email verified successfully.",
            IsEmailVerified = true
        };
    }
}