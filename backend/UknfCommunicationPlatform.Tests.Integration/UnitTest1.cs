using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Basic database integration tests to validate EF Core setup with PostgreSQL
/// </summary>
public class DatabaseIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public DatabaseIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CanCreateAndRetrieveUser()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = new User
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var retrieved = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be("test@example.com");
        retrieved.FirstName.Should().Be("Test");
    }

    [Fact]
    public async Task CanCreateSupervisedEntity()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var entity = new SupervisedEntity
        {
            Name = "Test Bank",
            NIP = "1234567890",
            Email = "contact@testbank.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.SupervisedEntities.Add(entity);
        await context.SaveChangesAsync();

        var retrieved = await context.SupervisedEntities
            .FirstOrDefaultAsync(e => e.NIP == "1234567890");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Bank");
    }
}

