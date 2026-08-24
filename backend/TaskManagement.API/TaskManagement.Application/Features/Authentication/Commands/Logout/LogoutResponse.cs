namespace TaskManagement.Application.Features.Authentication.Commands.Logout;

public class LogoutResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}