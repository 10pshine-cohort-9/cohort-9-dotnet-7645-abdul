namespace TaskManagement.Application.Features.Authentication.Queries.GetSessions;

public class SessionDto
{
    public Guid SessionId { get; set; }

    public string? DeviceName { get; set; }

    public string? Browser { get; set; }

    public string? OperatingSystem { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsCurrentDevice { get; set; }
}