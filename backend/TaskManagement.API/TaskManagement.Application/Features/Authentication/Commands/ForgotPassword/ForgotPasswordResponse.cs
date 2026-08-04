namespace TaskManagement.Application.Features.Authentication.Commands.ForgotPassword;

public sealed class ForgotPasswordResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}