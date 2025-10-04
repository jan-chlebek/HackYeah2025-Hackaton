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
                // Suppress PendingModelChangesWarning for tests
                // This warning appears when there are minor differences between model and migration snapshot
                options.ConfigureWarnings(warnings => 
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
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

        var tables = new[]
        {
            "refresh_tokens",
            "file_library_permissions",
            "case_documents",
            "case_histories",
            "cases",
            "announcement_attachments",
            "announcement_reads",
            "announcement_histories",
            "announcement_recipients",
            "announcements",
            "reports",
            "message_attachments",
            "messages",
            "contact_group_members",
            "contact_groups",
            "contacts",
            "faq_questions",
            "faq_ratings",
            "file_libraries",
            "audit_logs",
            "password_histories",
            "password_policies",
            "user_roles",
            "users",
            "role_permissions",
            "permissions",
            "roles",
            "entities"
        };

        // Try to truncate each table individually - ignore errors for tables that don't exist
        foreach (var table in tables)
        {
            try
            {
                #pragma warning disable EF1002 // Risk of SQL injection - table names are from hardcoded array
                await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE");
                #pragma warning restore EF1002
            }
            catch
            {
                // Ignore errors - table might not exist yet
            }
        }
    }
}
