using Microsoft.AspNetCore.Http;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Infrastructure.Authentication.Services;

public class ClientInfoService : IClientInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientInfoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public SessionInfo GetSessionInfo()
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        return new SessionInfo
        {
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = request?.Headers.UserAgent.ToString(),
            Browser = request?.Headers.UserAgent.ToString(),
            OperatingSystem = request?.Headers.UserAgent.ToString(),
            DeviceName = null
        };
    }
}