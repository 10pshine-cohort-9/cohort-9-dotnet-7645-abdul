using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace TaskManagement.API.DependencyInjection;

public static class PresentationServiceRegistration
{
    public static IServiceCollection AddPresentation(
      this IServiceCollection services)
    {
         ArgumentNullException.ThrowIfNull(services);
        services.AddControllers();

        services.AddFluentValidationAutoValidation();

        services.AddFluentValidationClientsideAdapters();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }
}