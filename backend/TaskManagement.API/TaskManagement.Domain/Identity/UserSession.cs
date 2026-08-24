namespace TaskManagement.Domain.Identity;

public class UserSession
{
    public Guid Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    public ApplicationUser ApplicationUser { get; set; } = default!;

    public Guid RefreshTokenId { get; set; }

    public RefreshToken RefreshToken { get; set; } = default!;

    public string? DeviceName { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LoggedOutAt { get; set; }
}