using Microsoft.Extensions.Options;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Infrastructure.Authentication.Settings;

namespace TaskManagement.Infrastructure.Services;

public sealed class ApplicationUrlService : IApplicationUrlService
{
    private readonly AppSettings _settings;

    public ApplicationUrlService(IOptions<AppSettings> settings)
    {
        _settings = settings.Value;
    }

    public string FrontendBaseUrl => _settings.FrontendBaseUrl;
}