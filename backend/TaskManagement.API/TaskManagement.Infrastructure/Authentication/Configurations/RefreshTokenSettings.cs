namespace TaskManagement.Infrastructure.Authentication.Configurations;

public class RefreshTokenSettings
{
    public int RefreshTokenExpiryDays { get; set; }

    public bool AllowMultipleSessions { get; set; }

    public int MaxActiveSessions { get; set; }
}