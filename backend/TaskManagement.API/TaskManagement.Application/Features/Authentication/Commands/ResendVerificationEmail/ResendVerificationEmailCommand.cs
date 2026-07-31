using MediatR;
 

namespace TaskManagement.Application.Features.Authentication.ResendVerificationEmail;

public class ResendVerificationEmailCommand
    : IRequest<ResendVerificationEmailResponse>
{
    public string Email { get; set; } = string.Empty;
}