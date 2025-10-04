using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Authorization;
using UknfCommunicationPlatform.Core.Configuration;
using UknfCommunicationPlatform.Core.Interfaces;
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

        // Register JWT settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Register authentication and authorization services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();

        // Register HTTP context accessor for current user service
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, EntityOwnershipAuthorizationHandler>();

        // Register other services
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<UserManagementService>();
        services.AddScoped<EntityManagementService>();
        services.AddScoped<MessageService>();

        // TODO: Register additional repositories and services here
        // services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }
}
