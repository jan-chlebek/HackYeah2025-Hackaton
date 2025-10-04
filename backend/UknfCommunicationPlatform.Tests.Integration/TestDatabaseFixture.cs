using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Api;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Test fixture that manages PostgreSQL database for integration tests.
/// Uses the development database and cleans it between tests to ensure isolation.
/// NOTE: Ensure PostgreSQL is running before tests (use ensure-test-db.sh)
/// </summary>
public class TestDatabaseFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _connectionString;

    public TestDatabaseFixture()
    {
        // Use development database connection
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "127.0.0.1";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "uknf_user";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "uknf_password";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "uknf_db";

        _connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext pointing to test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Initialize the test database before any tests run
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure migrations are applied
        await dbContext.Database.MigrateAsync();

        // Clean the database before tests start
        await ResetDatabaseAsync();
    }

    /// <summary>
    /// Clean up after all tests complete
    /// </summary>
    public new async Task DisposeAsync()
    {
        // Clean the database after tests finish
        await ResetDatabaseAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Get a database context scope for direct database operations in tests
    /// </summary>
    public IServiceScope CreateDbContextScope()
    {
        return Services.CreateScope();
    }

    /// <summary>
    /// Seed test data into the database
    /// </summary>
    public async Task SeedTestDataAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(dbContext, passwordHasher, logger);
        await seeder.SeedAsync();
    }

    /// <summary>
    /// Clear all data from the test database between tests
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Delete all data from tables in reverse order of dependencies
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RefreshTokens\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"FileLibraryPermissions\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"CaseDocuments\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"CaseHistories\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Cases\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"AnnouncementAttachments\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"AnnouncementReads\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"AnnouncementHistories\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Announcements\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Reports\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"MessageAttachments\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Messages\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Contacts\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"FaqQuestions\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"AuditLogs\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PasswordHistories\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"UserRoles\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RolePermissions\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Permissions\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Roles\" RESTART IDENTITY CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"SupervisedEntities\" RESTART IDENTITY CASCADE");
        }
        catch (Exception)
        {
            // Ignore errors during cleanup - tables might not exist yet
        }
    }
}
