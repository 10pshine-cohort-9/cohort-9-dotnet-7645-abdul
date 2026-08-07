using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Contexts;

namespace TaskManagement.API.Configurations;

public static class HealthCheckConfiguration
{
    public static IServiceCollection AddApplicationHealthChecks(
        this IServiceCollection services)
    {
         ArgumentNullException.ThrowIfNull(services);
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "Database");

        return services;
    }
}