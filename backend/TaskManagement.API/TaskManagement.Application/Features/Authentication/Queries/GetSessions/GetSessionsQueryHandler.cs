using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Application.Features.Authentication.Queries.GetSessions;

public sealed class GetSessionsQueryHandler
    : IRequestHandler<GetSessionsQuery, List<SessionDto>>
{
    private readonly ISessionService _sessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetSessionsQueryHandler(
        ISessionService sessionService,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _sessionService = sessionService;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<List<SessionDto>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user is null)
        {
            return new List<SessionDto>();
        }

        var userId = _userManager.GetUserId(user);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new List<SessionDto>();
        }

        var sessions = await _sessionService.GetActiveSessionsAsync(userId);

        return sessions
            .OrderByDescending(x => x.LastActivityAt)
            .Select(x => new SessionDto
            {
                SessionId = x.Id,
                DeviceName = x.DeviceName,
                Browser = x.Browser,
                OperatingSystem = x.OperatingSystem,
                IpAddress = x.IpAddress,
                CreatedAt = x.CreatedAt,
                LastActivityAt = x.LastActivityAt,
                ExpiresAt = x.ExpiresAt,

                // We'll improve this when implementing current-device detection
                IsCurrentDevice = false
            })
            .ToList();
    }
}