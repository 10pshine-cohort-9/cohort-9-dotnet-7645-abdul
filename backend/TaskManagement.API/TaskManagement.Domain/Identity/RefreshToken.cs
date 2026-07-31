using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Identity;

public class RefreshToken
{
    public Guid Id { get; set; }

    [Required]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public string? ReasonRevoked { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;

    public ApplicationUser ApplicationUser { get; set; } = default!;

    public ICollection<UserSession> UserSessions { get; set; }
        = new List<UserSession>();

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;
}