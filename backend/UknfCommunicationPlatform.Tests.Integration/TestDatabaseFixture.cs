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
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RefreshTokens\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Reports\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Messages\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Documents\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ContactEntries\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"FAQItems\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"FolderCases\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" CASCADE");
            await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"SupervisedEntities\" CASCADE");

            // Reset sequences
            await dbContext.Database.ExecuteSqlRawAsync(@"
                DO $$
                DECLARE
                    seq_name TEXT;
                BEGIN
                    FOR seq_name IN
                        SELECT sequence_name
                        FROM information_schema.sequences
                        WHERE sequence_schema = 'public'
                    LOOP
                        EXECUTE 'ALTER SEQUENCE ' || seq_name || ' RESTART WITH 1';
                    END LOOP;
                END $$;
            ");
        }
        catch
        {
            // Ignore errors during cleanup - tables might not exist yet
        }
    }
}
