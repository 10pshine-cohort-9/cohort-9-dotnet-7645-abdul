using System.Security.Claims;

namespace TaskManagement.Application.Common.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(
        string userId,
        string userName,
        string email,
        IList<string> roles);

    Task<List<Claim>> GenerateClaimsAsync(
        string userId,
        string userName,
        string email,
        IList<string> roles);

    ClaimsPrincipal? ValidateToken(string token);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}