namespace TaskManagement.Application.Features.Authentication.Commands.RevokeSession;

public sealed class RevokeSessionResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;
}