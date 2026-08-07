using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Infrastructure.Persistence.Contexts;

namespace TaskManagement.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
   public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' was not found.");

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    return services;
}
}