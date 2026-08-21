namespace TaskManagement.Infrastructure.Authentication.Settings;

public sealed class AppSettings
{
    public const string SectionName = "AppSettings";

    public string FrontendBaseUrl { get; set; } = string.Empty;
}