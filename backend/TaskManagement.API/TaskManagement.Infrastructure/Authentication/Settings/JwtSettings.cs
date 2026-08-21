namespace TaskManagement.Infrastructure.Authentication.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int AccessTokenExpiryMinutes { get; set; }

    public int RefreshTokenExpiryDays { get; set; }
}