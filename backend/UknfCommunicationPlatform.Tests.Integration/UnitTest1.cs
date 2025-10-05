using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Basic database integration tests to validate EF Core setup with PostgreSQL
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class DatabaseIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public DatabaseIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        // Use lightweight reset - only clean test-created data, keep seed data
        await _factory.ResetTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CanCreateAndRetrieveUser()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var uniqueEmail = $"test{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
        var user = new User
        {
            Email = uniqueEmail,
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var retrieved = await context.Users.FirstOrDefaultAsync(u => u.Email == uniqueEmail);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be(uniqueEmail);
        retrieved.FirstName.Should().Be("Test");
    }

    [Fact]
    public async Task CanCreateSupervisedEntity()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var uniqueCode = $"TST{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        var entity = new SupervisedEntity
        {
            Name = "Test Bank",
            UKNFCode = uniqueCode,
            EntityType = "Bank",
            NIP = "1234567890",
            Email = "contact@testbank.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        context.SupervisedEntities.Add(entity);
        await context.SaveChangesAsync();

        var retrieved = await context.SupervisedEntities
            .FirstOrDefaultAsync(e => e.UKNFCode == uniqueCode);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Bank");
        retrieved.UKNFCode.Should().Be(uniqueCode);
    }
}

