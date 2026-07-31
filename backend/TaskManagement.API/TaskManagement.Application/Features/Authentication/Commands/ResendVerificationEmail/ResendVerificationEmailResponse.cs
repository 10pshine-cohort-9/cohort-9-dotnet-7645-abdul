namespace TaskManagement.Application.Features.Authentication.ResendVerificationEmail;

public class ResendVerificationEmailResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}