//using Microsoft.AspNetCore.Http;
//using TaskManagement.Application.Common.Interfaces;
//using TaskManagement.Application.Common.Models;

//namespace TaskManagement.Infrastructure.Authentication.Services;

//public class ClientInfoService : IClientInfoService
//{
//    private readonly IHttpContextAccessor _httpContextAccessor;

//    public ClientInfoService(IHttpContextAccessor httpContextAccessor)
//    {
//        _httpContextAccessor = httpContextAccessor;
//    }

//    public SessionInfo GetSessionInfo()
//    {
//        var request = _httpContextAccessor.HttpContext?.Request;

//        return new SessionInfo
//        {
//            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
//            UserAgent = request?.Headers.UserAgent.ToString(),
//            Browser = request?.Headers.UserAgent.ToString(),
//            OperatingSystem = request?.Headers.UserAgent.ToString(),
//            DeviceName = null
//        };
//    }
//}

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

        var userAgent = request?.Headers.UserAgent.ToString() ?? string.Empty;

        var browser = "Unknown";

        if (userAgent.Contains("Edg"))
            browser = "Microsoft Edge";
        else if (userAgent.Contains("Chrome"))
            browser = "Google Chrome";
        else if (userAgent.Contains("Firefox"))
            browser = "Mozilla Firefox";
        else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
            browser = "Safari";

        var operatingSystem = "Unknown";

        if (userAgent.Contains("Windows"))
            operatingSystem = "Windows";
        else if (userAgent.Contains("Android"))
            operatingSystem = "Android";
        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            operatingSystem = "iOS";
        else if (userAgent.Contains("Mac"))
            operatingSystem = "macOS";
        else if (userAgent.Contains("Linux"))
            operatingSystem = "Linux";

        return new SessionInfo
        {
            IpAddress = _httpContextAccessor.HttpContext?
                .Connection.RemoteIpAddress?.ToString(),

            Browser = browser,
            OperatingSystem = operatingSystem,
            UserAgent = userAgent,
            DeviceName = null
        };
    }
}