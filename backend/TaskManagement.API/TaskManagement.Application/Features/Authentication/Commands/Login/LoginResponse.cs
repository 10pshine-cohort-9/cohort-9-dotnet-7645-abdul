namespace TaskManagement.Application.Features.Authentication.Login;

public class LoginResponse
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public string? Email { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime ExpiresAt { get; set; }
}