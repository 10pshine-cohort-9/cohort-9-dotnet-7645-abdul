using MediatR;

namespace TaskManagement.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailCommand : IRequest<VerifyEmailResponse>
{
    public string UserId { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}