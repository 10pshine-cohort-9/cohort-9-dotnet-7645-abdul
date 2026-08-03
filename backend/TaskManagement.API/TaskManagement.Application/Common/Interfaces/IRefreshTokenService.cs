using TaskManagement.Application.Common.Models;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenResult> GenerateAsync(
     ApplicationUser user,
     string ipAddress,
      bool rememberMe = false);

    bool Validate(RefreshToken token);

    Task<RefreshTokenResult> RotateAsync(
        RefreshToken refreshToken,
        string ipAddress);

    Task RevokeAsync(
        RefreshToken refreshToken,
        string ipAddress,
        string reason);

    bool IsExpired(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string rawToken);

    Task<List<RefreshToken>> GetActiveTokensAsync(string userId);
    Task<RefreshToken?> GetByIdAsync(Guid id);

}