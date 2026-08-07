using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class SessionService : ISessionService
{
    public Task<UserSession> CreateAsync(UserSession session)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLastActivityAsync(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserSession>> GetActiveSessionsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task TerminateSessionAsync(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public Task TerminateAllSessionsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveExpiredSessionsAsync()
    {
        throw new NotImplementedException();
    }
}