using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Integration tests for SupervisedEntity operations and relationships
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class SupervisedEntityIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public SupervisedEntityIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CanCreateSupervisedEntityWithFullDetails()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var uniqueCode = $"BANK{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        var entity = new SupervisedEntity
        {
            Name = "Example Bank S.A.",
            UKNFCode = uniqueCode,
            EntityType = "Bank",
            NIP = "1234567890",
            KRS = "0000123456",
            REGON = "123456789",
            LEI = "123456789012345678XX",
            Street = "Main Street",
            BuildingNumber = "123",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "contact@examplebank.com",
            Phone = "+48123456789",
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
        retrieved!.Name.Should().Be("Example Bank S.A.");
        retrieved.NIP.Should().Be("1234567890");
        retrieved.KRS.Should().Be("0000123456");
        retrieved.Email.Should().Be("contact@examplebank.com");
    }

    [Fact]
    public async Task UKNFCode_MustBeUnique()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var duplicateCode = $"DUP{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

        var entity1 = new SupervisedEntity
        {
            Name = "First Bank",
            UKNFCode = duplicateCode,
            EntityType = "Bank",
            NIP = "1111111111",
            Email = "first@bank.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var entity2 = new SupervisedEntity
        {
            Name = "Second Bank",
            UKNFCode = duplicateCode,
            EntityType = "Bank",
            NIP = "2222222222",
            Email = "second@bank.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.SupervisedEntities.Add(entity1);
        await context.SaveChangesAsync();

        context.SupervisedEntities.Add(entity2);

        // Act & Assert
        var act = async () => await context.SaveChangesAsync();
        var exception = await act.Should().ThrowAsync<DbUpdateException>();
        exception.And.InnerException.Should().NotBeNull();
        exception.And.InnerException!.Message.Should().Contain("duplicate key");
    }

    [Fact]
    public async Task CanQuerySupervisedEntitiesByType()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var guid = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        var entities = new[]
        {
            new SupervisedEntity
            {
                Name = "Test Bank 1",
                UKNFCode = $"BANK1{guid}",
                EntityType = "Bank",
                NIP = $"1111{guid}",
                Email = $"bank1{guid}@test.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new SupervisedEntity
            {
                Name = "Test Bank 2",
                UKNFCode = $"BANK2{guid}",
                EntityType = "Bank",
                NIP = $"2222{guid}",
                Email = $"bank2{guid}@test.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new SupervisedEntity
            {
                Name = "Test Insurance",
                UKNFCode = $"INS1{guid}",
                EntityType = "Insurance",
                NIP = $"3333{guid}",
                Email = $"insurance{guid}@test.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.SupervisedEntities.AddRange(entities);
        await context.SaveChangesAsync();

        // Act
        var banks = await context.SupervisedEntities
            .Where(e => e.EntityType == "Bank" && e.UKNFCode.Contains(guid))
            .ToListAsync();

        // Assert
        banks.Should().HaveCount(2);
        banks.Should().Contain(e => e.UKNFCode == $"BANK1{guid}");
        banks.Should().Contain(e => e.UKNFCode == $"BANK2{guid}");
    }

    [Fact]
    public async Task CanUpdateSupervisedEntityDetails()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var uniqueCode = $"UPD{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        var entity = new SupervisedEntity
        {
            Name = "Old Name Bank",
            UKNFCode = uniqueCode,
            EntityType = "Bank",
            NIP = "9999999999",
            Email = "old@bank.com",
            Phone = "+48111111111",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.SupervisedEntities.Add(entity);
        await context.SaveChangesAsync();

        // Act
        entity.Name = "New Name Bank";
        entity.Email = "new@bank.com";
        entity.Phone = "+48222222222";
        entity.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var retrieved = await context.SupervisedEntities
            .FirstOrDefaultAsync(e => e.UKNFCode == uniqueCode);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("New Name Bank");
        retrieved.Email.Should().Be("new@bank.com");
        retrieved.Phone.Should().Be("+48222222222");
        retrieved.UpdatedAt.Should().BeAfter(retrieved.CreatedAt);
    }
}
