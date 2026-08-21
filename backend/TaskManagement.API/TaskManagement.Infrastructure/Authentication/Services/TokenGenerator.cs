using System.Security.Cryptography;
using System.Text;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }

    public string GenerateRandomIdentifier(int length = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);

        return Convert.ToHexString(bytes);
    }
}