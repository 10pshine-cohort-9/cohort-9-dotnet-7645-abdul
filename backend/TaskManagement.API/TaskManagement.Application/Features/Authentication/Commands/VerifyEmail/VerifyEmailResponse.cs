namespace TaskManagement.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }
}