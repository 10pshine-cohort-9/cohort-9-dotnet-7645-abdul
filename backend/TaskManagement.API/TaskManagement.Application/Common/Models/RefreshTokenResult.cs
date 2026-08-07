namespace TaskManagement.Application.Common.Models;

using TaskManagement.Domain.Identity;

public class RefreshTokenResult
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshToken RefreshTokenEntity { get; set; } = default!;
}