namespace TaskManagement.Application.Features.Authentication.Commands.Register;

public class RegisterResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public string? Email { get; set; }

    public bool RequiresEmailVerification { get; set; }
}