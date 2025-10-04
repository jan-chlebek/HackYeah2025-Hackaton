using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add infrastructure services including database context
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register services
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<UserManagementService>();
        services.AddScoped<EntityManagementService>();

        // TODO: Register additional repositories and services here
        // services.AddScoped<IReportRepository, ReportRepository>();
        // services.AddScoped<IMessageService, MessageService>();

        return services;
    }
}
