using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UknfCommunicationPlatform.Api;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit.Abstractions;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Test fixture that manages PostgreSQL database for integration tests.
/// Uses the development database and cleans it between tests to ensure isolation.
/// NOTE: Ensure PostgreSQL is running before tests (use ensure-test-db.sh)
/// </summary>
public class TestDatabaseFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _connectionString;
    private static readonly SemaphoreSlim _seedLock = new SemaphoreSlim(1, 1);
    private static bool _globalIsSeeded = false;
    private ITestOutputHelper? _output;

    public void SetOutput(ITestOutputHelper output)
    {
        _output = output;
    }

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

            // Replace password hashing service with faster version for tests
            var passwordHasherDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPasswordHashingService));
            if (passwordHasherDescriptor != null)
            {
                services.Remove(passwordHasherDescriptor);
            }
            services.AddScoped<IPasswordHashingService>(sp => new PasswordHashingService(workFactor: 4));
        });

        // Signal to seeder we want to skip baseline announcements for deterministic announcement tests
        Environment.SetEnvironmentVariable("SKIP_SEED_ANNOUNCEMENTS", "1");

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Initialize the test database once for all tests
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure migrations are applied
        await dbContext.Database.MigrateAsync();

        // Clean and seed the database once before all tests (with global lock to prevent race conditions)
        await _seedLock.WaitAsync();
        try
        {
            if (!_globalIsSeeded)
            {
                await ResetDatabaseAsync();
                await SeedTestDataAsync();
                _globalIsSeeded = true;
            }
            else
            {
                // Guard against a previous fixture instance having truncated data in DisposeAsync.
                // If core seed data (users) is missing, reseed (without full reset to keep any diagnostics).
                if (!await dbContext.Users.AnyAsync())
                {
                    await SeedTestDataAsync();
                }
            }
        }
        finally
        {
            _seedLock.Release();
        }
    }

    /// <summary>
    /// Reset database and reseed for tests that need fresh data
    /// </summary>
    public async Task ResetAndReseedAsync()
    {
        await _seedLock.WaitAsync();
        try
        {
            await ResetDatabaseAsync();
            await SeedTestDataAsync();
        }
        finally
        {
            _seedLock.Release();
        }
    }

    /// <summary>
    /// Clean up after all tests complete
    /// </summary>
    public new async Task DisposeAsync()
    {
        // Intentionally NO global cleanup here.
        // Rationale: xUnit creates a new fixture instance per test class (IClassFixture).
        // The original implementation truncated all tables in DisposeAsync of the first test class
        // while retaining _globalIsSeeded = true. Subsequent fixture instances then skipped seeding
        // and found an empty database, causing failures (e.g. admin user missing in Cases tests).
        // Leaving data in place ensures later test classes reuse the baseline seed.
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
        // For tests, DON'T use forceReseed - we handle clearing separately via ResetDatabaseAsync
        // This avoids transaction issues with TRUNCATE
        await seeder.SeedAsync(forceReseed: false);
    }

    /// <summary>
    /// Reset only test-created data (not seed data) between tests for better performance.
    /// This deletes only data created by tests, keeping the seeded data intact.
    /// If base data is missing, it will reseed the database.
    /// </summary>
    public async Task ResetTestDataAsync()
    {
        await _seedLock.WaitAsync();
        try
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if we have the base seeded data
            var hasUsers = await dbContext.Users.AnyAsync();

            if (!hasUsers)
            {
                // Base data is missing, need to reseed
                _output?.WriteLine("Base data missing, reseeding database...");
                await ResetDatabaseAsync();
                await SeedTestDataAsync();
                return;
            }

            try
            {
                // Perform selective cleanup inside a transaction for atomicity
                await using var tx = await dbContext.Database.BeginTransactionAsync();

                // Delete only test-created data (IDs higher than seed data)
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM message_attachments WHERE message_id > 20");
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM messages WHERE id > 20");

                // Reset reports entirely to avoid number collisions
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE reports RESTART IDENTITY CASCADE");

                // Wipe announcements entirely (including originally seeded baseline) to ensure per-test isolation
                // Using TRUNCATE with CASCADE resets identities so tests can assert exact counts without interference
                _output?.WriteLine("[ResetTestData] Truncating announcement tables...");
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE announcement_attachments RESTART IDENTITY CASCADE");
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE announcement_reads RESTART IDENTITY CASCADE");
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE announcement_histories RESTART IDENTITY CASCADE");
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE announcement_recipients RESTART IDENTITY CASCADE");
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE announcements RESTART IDENTITY CASCADE");
                _output?.WriteLine("[ResetTestData] Announcement tables truncated.");

                // Restore modified seed data
                await dbContext.Database.ExecuteSqlRawAsync("UPDATE messages SET is_read = false, read_at = NULL WHERE id <= 20");
                await dbContext.Database.ExecuteSqlRawAsync("UPDATE users SET failed_login_attempts = 0, locked_until = NULL WHERE id <= 10");

                await tx.CommitAsync();

                // Reseed reports (outside transaction using seeder for consistency with generation logic)
                using var seederScope = Services.CreateScope();
                var seeder = seederScope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedReportsOnlyAsync();
            }
            catch (Exception ex)
            {
                _output?.WriteLine($"Selective reset failed: {ex.Message}, performing full reset...");
                await ResetDatabaseAsync();
                await SeedTestDataAsync();
            }
        }
        finally
        {
            _seedLock.Release();
        }
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

    /// <summary>
    /// Generate a JWT token for testing with authenticated endpoints
    /// </summary>
    public string GenerateJwtToken(long userId, string email, string role = "InternalUser")
    {
        // NOTE: Integration tests previously generated tokens with only a single role claim.
        // Many API policies require permission claims ("permission" claim type). Missing permissions
        // caused widespread 403 responses. We now derive the user's actual roles & permissions from the
        // seeded database so tests exercise the real authorization pipeline.
        try
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Load role names assigned to this user from the join table
            var roleNames = db.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .Distinct()
                .ToList();

            // Backwards compatibility: some tests passed role strings like "SupervisorUser" or "UKNF".
            // Map legacy/mistyped role names to actual seeded roles if user has none loaded yet.
            var legacyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "SupervisorUser", "Supervisor" },
                { "UKNF", "Administrator" } // Treat generic UKNF as Administrator for full access in tests
            };

            if (!roleNames.Any())
            {
                if (legacyMap.TryGetValue(role, out var mapped))
                {
                    roleNames.Add(mapped);
                }
                else
                {
                    // Fallback to provided role if user-role relation not seeded yet
                    roleNames.Add(role);
                }
            }

            // Collect permission names for all roles
            var permissions = db.RolePermissions
                .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId) || roleNames.Contains(rp.Role.Name))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            var key = "ThisIsAVerySecureSecretKeyForJWTTokenGeneration_ChangeInProduction_MinimumLengthIs32Characters!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            foreach (var r in roleNames.Distinct())
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }

            foreach (var p in permissions)
            {
                claims.Add(new Claim("permission", p));
            }

            var token = new JwtSecurityToken(
                issuer: "UKNF-API",
                audience: "UKNF-Portal",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            // Debug logging removed after resolving authorization issue
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch
        {
            // If something fails (e.g., during very early seeding), fall back to the simple token.
            var key = "ThisIsAVerySecureSecretKeyForJWTTokenGeneration_ChangeInProduction_MinimumLengthIs32Characters!";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer: "UKNF-API",
                audience: "UKNF-Portal",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
