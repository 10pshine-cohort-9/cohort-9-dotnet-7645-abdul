using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? ProfilePictureUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();

    public ICollection<UserSession> UserSessions { get; set; }
        = new List<UserSession>();
}