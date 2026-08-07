namespace TaskManagement.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}