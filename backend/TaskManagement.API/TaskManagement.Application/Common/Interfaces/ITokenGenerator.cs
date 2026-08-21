namespace TaskManagement.Application.Common.Interfaces;

public interface ITokenGenerator
{
    string GenerateRefreshToken();

    string HashToken(string token);

    string GenerateRandomIdentifier(int length = 32);
}