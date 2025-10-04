using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Test fixture that manages PostgreSQL database for integration tests.
/// Uses the development database and cleans it between tests to ensure isolation.
/// NOTE: Ensure PostgreSQL is running before tests (use ensure-test-db.sh)
/// </summary>
public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly ServiceProvider _serviceProvider;

    public TestDatabaseFixture()
    {
        // Use development database connection
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "127.0.0.1";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "uknf_user";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "uknf_password";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "uknf_db";

        _connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

        // Create a minimal service provider with just the DbContext
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_connectionString));
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Initialize the test database before any tests run
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure migrations are applied
        await dbContext.Database.MigrateAsync();

        // Clean the database before tests start
        await ResetDatabaseAsync();
    }

    /// <summary>
    /// Clean up after all tests complete
    /// </summary>
    public async Task DisposeAsync()
    {
        // Clean the database after tests finish
        await ResetDatabaseAsync();
        await _serviceProvider.DisposeAsync();
    }

    /// <summary>
    /// Get a database context scope for direct database operations in tests
    /// </summary>
    public IServiceScope CreateDbContextScope()
    {
        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Clear all data from the test database between tests
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
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
