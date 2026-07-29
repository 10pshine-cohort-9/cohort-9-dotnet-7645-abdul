using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;

namespace TaskManagement.Infrastructure.Authentication.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser User
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                return new CurrentUser
                {
                    IsAuthenticated = false
                };
            }

            return new CurrentUser
            {
                IsAuthenticated = true,

                UserId = principal.FindFirstValue(
                    ClaimTypes.NameIdentifier),

                Email = principal.FindFirstValue(
                    ClaimTypes.Email),

                Username = principal.FindFirstValue(
                    ClaimTypes.Name),

                Roles = principal
                    .FindAll(ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToArray()
            };
        }
    }
}