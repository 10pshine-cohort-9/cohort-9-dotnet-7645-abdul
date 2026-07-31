using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateAsync(
        ApplicationUser user,
        string ipAddress);

    bool Validate(RefreshToken token);

    Task<RefreshToken> RotateAsync(
        RefreshToken refreshToken,
        string ipAddress);

    Task RevokeAsync(
        RefreshToken refreshToken,
        string ipAddress,
        string reason);

    bool IsExpired(RefreshToken refreshToken);
}