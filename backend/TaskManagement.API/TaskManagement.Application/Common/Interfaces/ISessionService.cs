using TaskManagement.Domain.Identity;
namespace TaskManagement.Application.Common.Interfaces;

public interface ISessionService
{
    Task<UserSession> CreateAsync(UserSession session);

    Task UpdateLastActivityAsync(Guid sessionId);

    Task<List<UserSession>> GetActiveSessionsAsync(string userId);

    Task TerminateSessionAsync(Guid sessionId);

    Task TerminateAllSessionsAsync(string userId);

    Task RemoveExpiredSessionsAsync();

    Task UpdateAsync(UserSession session);
    Task<UserSession?> GetByIdAsync(Guid sessionId);
}