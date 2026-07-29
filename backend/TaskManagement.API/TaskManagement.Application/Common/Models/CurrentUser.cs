namespace TaskManagement.Application.Common.Models;

public sealed class CurrentUser
{
    public string? UserId { get; init; }

    public string? Email { get; init; }

    public string? Username { get; init; }

    public bool IsAuthenticated { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; }
        = Array.Empty<string>();
}