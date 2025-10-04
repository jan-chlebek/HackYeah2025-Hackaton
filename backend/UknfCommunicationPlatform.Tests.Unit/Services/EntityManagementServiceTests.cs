using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.Entities;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for EntityManagementService
/// </summary>
public class EntityManagementServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EntityManagementService _sut;

    public EntityManagementServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new EntityManagementService(_context);
    }

    [Fact]
    public async Task GetEntitiesAsync_WithoutFilters_ShouldReturnAllEntities()
    {
        // Arrange
        await SeedEntitiesAsync();

        // Act
        var (entities, totalCount) = await _sut.GetEntitiesAsync(page: 1, pageSize: 10);

        // Assert
        entities.Should().HaveCount(3);
        totalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetEntitiesAsync_WithSearchTerm_ShouldReturnMatchingEntities()
    {
        // Arrange
        await SeedEntitiesAsync();

        // Act
        var (entities, totalCount) = await _sut.GetEntitiesAsync(
            page: 1, 
            pageSize: 10, 
            searchTerm: "Test Bank");

        // Assert
        entities.Should().HaveCount(1);
        entities.First().Name.Should().Be("Test Bank");
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetEntitiesAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedEntitiesAsync();

        // Act
        var (entities, totalCount) = await _sut.GetEntitiesAsync(page: 2, pageSize: 1);

        // Assert
        entities.Should().HaveCount(1);
        totalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetEntityByIdAsync_ExistingEntity_ShouldReturnEntity()
    {
        // Arrange
        var entity = await SeedSingleEntityAsync();

        // Act
        var result = await _sut.GetEntityByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(entity.Name);
        result.UknfCode.Should().Be(entity.UKNFCode);
    }

    [Fact]
    public async Task GetEntityByIdAsync_NonExistingEntity_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetEntityByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateEntityAsync_ValidData_ShouldCreateEntity()
    {
        // Arrange
        var request = new CreateEntityRequest
        {
            Name = "New Entity",
            EntityType = "Bank",
            NIP = "1234567890",
            REGON = "123456789",
            KRS = "0000012345",
            Street = "Test Street",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "new@example.com"
        };

        // Act
        var result = await _sut.CreateEntityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.UknfCode.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateEntityAsync_DuplicateNIP_ShouldThrowException()
    {
        // Arrange
        await SeedSingleEntityAsync();
        
        var request = new CreateEntityRequest
        {
            Name = "Another Entity",
            EntityType = "Bank",
            NIP = "1234567890",
            REGON = "999999999"
        };

        // Act
        var act = async () => await _sut.CreateEntityAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*NIP*already exists*");
    }

    [Fact]
    public async Task DeleteEntityAsync_ExistingEntity_ShouldSoftDelete()
    {
        // Arrange
        var entity = await SeedSingleEntityAsync();

        // Act
        var result = await _sut.DeleteEntityAsync(entity.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedEntity = await _context.SupervisedEntities.FindAsync(entity.Id);
        deletedEntity!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteEntityAsync_NonExistingEntity_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteEntityAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    private async Task<SupervisedEntity> SeedSingleEntityAsync()
    {
        var entity = new SupervisedEntity
        {
            UKNFCode = "UKNF000001",
            Name = "Test Bank",
            EntityType = "Bank",
            NIP = "1234567890",
            REGON = "123456789",
            KRS = "0000012345",
            Street = "Test Street",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Country = "Poland",
            IsActive = true
        };

        _context.SupervisedEntities.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    private async Task SeedEntitiesAsync()
    {
        var entities = new List<SupervisedEntity>
        {
            new SupervisedEntity
            {
                UKNFCode = "UKNF000001",
                Name = "Test Bank",
                EntityType = "Bank",
                NIP = "1234567890",
                REGON = "123456789",
                IsActive = true
            },
            new SupervisedEntity
            {
                UKNFCode = "UKNF000002",
                Name = "Insurance Company",
                EntityType = "InsuranceCompany",
                NIP = "0987654321",
                REGON = "987654321",
                IsActive = true
            },
            new SupervisedEntity
            {
                UKNFCode = "UKNF000003",
                Name = "Inactive Entity",
                EntityType = "Bank",
                NIP = "5555555555",
                REGON = "555555555",
                IsActive = false
            }
        };

        _context.SupervisedEntities.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
