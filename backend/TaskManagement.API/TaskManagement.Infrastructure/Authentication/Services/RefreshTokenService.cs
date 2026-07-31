using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class RefreshTokenService : IRefreshTokenService
{
    public Task<RefreshToken> GenerateAsync(
        ApplicationUser user,
        string ipAddress)
    {
        throw new NotImplementedException();
    }

    public bool Validate(RefreshToken token)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> RotateAsync(
        RefreshToken refreshToken,
        string ipAddress)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAsync(
        RefreshToken refreshToken,
        string ipAddress,
        string reason)
    {
        throw new NotImplementedException();
    }

    public bool IsExpired(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }
}