namespace TaskManagement.Application.Features.Authentication.Commands.LogoutAll;

public class LogoutAllResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}