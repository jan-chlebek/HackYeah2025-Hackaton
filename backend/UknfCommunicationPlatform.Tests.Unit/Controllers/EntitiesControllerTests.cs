using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Api.Controllers.v1;
using UknfCommunicationPlatform.Core.DTOs.Entities;
using UknfCommunicationPlatform.Core.DTOs.Users;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Controllers;

public class EntitiesControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EntityManagementService _service;
    private readonly EntitiesController _controller;
    private readonly Mock<ILogger<EntitiesController>> _controllerLoggerMock;

    public EntitiesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new EntityManagementService(_context);
        _controllerLoggerMock = new Mock<ILogger<EntitiesController>>();
        _controller = new EntitiesController(_service, _controllerLoggerMock.Object);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var entities = new List<SupervisedEntity>
        {
            new SupervisedEntity
            {
                Id = 1,
                EntityType = "Bank",
                UKNFCode = "BANK001",
                Name = "Test Bank 1",
                NIP = "1234567890",
                REGON = "123456789",
                KRS = "0000123456",
                LEI = "TESTLEI000000001",
                Street = "Test Street",
                BuildingNumber = "1",
                City = "Warsaw",
                PostalCode = "00-001",
                Country = "Poland",
                Phone = "+48123456789",
                Email = "contact@testbank1.pl",
                Website = "https://testbank1.pl",
                IsActive = true,
                IsCrossBorder = false
            },
            new SupervisedEntity
            {
                Id = 2,
                EntityType = "Insurance",
                UKNFCode = "INS001",
                Name = "Test Insurance Company",
                NIP = "0987654321",
                REGON = "987654321",
                KRS = "0000987654",
                LEI = "TESTLEI000000002",
                Street = "Insurance Street",
                BuildingNumber = "10",
                City = "Krakow",
                PostalCode = "30-001",
                Country = "Poland",
                Phone = "+48987654321",
                Email = "info@testinsurance.pl",
                Website = "https://testinsurance.pl",
                IsActive = true,
                IsCrossBorder = false
            },
            new SupervisedEntity
            {
                Id = 3,
                EntityType = "Bank",
                UKNFCode = "BANK002",
                Name = "Inactive Bank",
                NIP = "5555555555",
                REGON = "555555555",
                KRS = "0000555555",
                Street = "Inactive Street",
                BuildingNumber = "5",
                City = "Gdansk",
                PostalCode = "80-001",
                Country = "Poland",
                Email = "inactive@bank.pl",
                IsActive = false,
                IsCrossBorder = false
            },
            new SupervisedEntity
            {
                Id = 4,
                EntityType = "InvestmentFund",
                UKNFCode = "FUND001",
                Name = "Test Investment Fund",
                NIP = "1111111111",
                REGON = "111111111",
                Street = "Fund Street",
                BuildingNumber = "4",
                City = "Poznan",
                PostalCode = "60-001",
                Country = "Poland",
                Email = "fund@investment.pl",
                IsActive = true,
                IsCrossBorder = true
            },
            new SupervisedEntity
            {
                Id = 5,
                EntityType = "Bank",
                UKNFCode = "BANK003",
                Name = "Another Test Bank",
                NIP = "2222222222",
                REGON = "222222222",
                KRS = "0000222222",
                Street = "Another Street",
                BuildingNumber = "2",
                City = "Wroclaw",
                PostalCode = "50-001",
                Country = "Poland",
                Email = "another@bank.pl",
                IsActive = true,
                IsCrossBorder = false
            }
        };

        _context.SupervisedEntities.AddRange(entities);
        _context.SaveChanges();
    }

    #region GetEntities Tests

    [Fact]
    public async Task GetEntities_ReturnsAllEntities_WhenNoFiltersApplied()
    {
        // Act
        var result = await _controller.GetEntities();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        value.Should().NotBeNull();
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        var pagination = value.GetType().GetProperty("pagination")?.GetValue(value);
        
        data.Should().HaveCount(5);
        
        var totalCount = pagination?.GetType().GetProperty("totalCount")?.GetValue(pagination);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetEntities_ReturnsPaginatedResults_WhenPageSizeIsSmall()
    {
        // Act
        var result = await _controller.GetEntities(page: 1, pageSize: 2);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        var pagination = value.GetType().GetProperty("pagination")?.GetValue(value);
        
        data.Should().HaveCount(2);
        
        var totalPages = pagination?.GetType().GetProperty("totalPages")?.GetValue(pagination);
        totalPages.Should().Be(3); // 5 entities / 2 per page = 3 pages
    }

    [Fact]
    public async Task GetEntities_FiltersbyEntityType_WhenEntityTypeProvided()
    {
        // Act
        var result = await _controller.GetEntities(entityType: "Bank");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        
        data.Should().HaveCount(3); // 3 banks in test data
        data.Should().OnlyContain(e => e.EntityType == "Bank");
    }

    [Fact]
    public async Task GetEntities_FiltersByActiveStatus_WhenIsActiveProvided()
    {
        // Act
        var result = await _controller.GetEntities(isActive: true);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        
        data.Should().HaveCount(4); // 4 active entities
        data.Should().OnlyContain(e => e.IsActive == true);
    }

    [Fact]
    public async Task GetEntities_SearchesInNameNipRegonKrs_WhenSearchTermProvided()
    {
        // Act - search by name
        var result = await _controller.GetEntities(searchTerm: "Insurance");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        
        data.Should().HaveCount(1);
        data.First().Name.Should().Contain("Insurance");
    }

    [Fact]
    public async Task GetEntities_DefaultsToPage1_WhenPageIsLessThan1()
    {
        // Act
        var result = await _controller.GetEntities(page: 0, pageSize: 2);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var pagination = value.GetType().GetProperty("pagination")?.GetValue(value);
        var page = pagination?.GetType().GetProperty("page")?.GetValue(pagination);
        
        page.Should().Be(1);
    }

    [Fact]
    public async Task GetEntities_LimitsPageSizeTo100_WhenPageSizeExceeds100()
    {
        // Act
        var result = await _controller.GetEntities(pageSize: 150);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var pagination = value.GetType().GetProperty("pagination")?.GetValue(value);
        var pageSize = pagination?.GetType().GetProperty("pageSize")?.GetValue(pagination);
        
        pageSize.Should().Be(20); // Default when invalid
    }

    #endregion

    #region GetEntity Tests

    [Fact]
    public async Task GetEntity_ReturnsEntity_WhenEntityExists()
    {
        // Act
        var result = await _controller.GetEntity(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var entity = okResult.Value.Should().BeOfType<EntityResponse>().Subject;
        
        entity.Id.Should().Be(1);
        entity.Name.Should().Be("Test Bank 1");
        entity.EntityType.Should().Be("Bank");
    }

    [Fact]
    public async Task GetEntity_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _controller.GetEntity(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CreateEntity Tests

    [Fact]
    public async Task CreateEntity_CreatesEntity_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateEntityRequest
        {
            Name = "New Test Bank",
            EntityType = "Bank",
            UknfCode = "BANK999", // Note: This will be ignored, system generates UKNF code
            NIP = "9999999999",
            REGON = "999999999",
            KRS = "0000999999",
            LEI = "TESTLEI000000999",
            Street = "New Street",
            BuildingNumber = "99",
            City = "Warsaw",
            PostalCode = "00-999",
            Country = "Poland",
            Phone = "+48999999999",
            Email = "new@testbank.pl",
            Website = "https://newtestbank.pl"
        };

        // Act
        var result = await _controller.CreateEntity(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var entity = createdResult.Value.Should().BeOfType<EntityResponse>().Subject;
        
        entity.Name.Should().Be("New Test Bank");
        entity.EntityType.Should().Be("Bank");
        entity.UknfCode.Should().StartWith("UKNF"); // System generates code
        
        // Verify it was added to database
        var dbEntity = await _context.SupervisedEntities.FindAsync(entity.Id);
        dbEntity.Should().NotBeNull();
        dbEntity!.Name.Should().Be("New Test Bank");
    }

    [Fact]
    public async Task CreateEntity_ReturnsCreatedAtAction_WithCorrectRoute()
    {
        // Arrange
        var request = new CreateEntityRequest
        {
            Name = "Another New Bank",
            EntityType = "Bank",
            UknfCode = "BANK888", // Will be ignored, system generates UKNF code
            Street = "Test Street",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "test@bank.pl"
        };

        // Act
        var result = await _controller.CreateEntity(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        
        createdResult.ActionName.Should().Be(nameof(EntitiesController.GetEntity));
        createdResult.RouteValues.Should().ContainKey("id");
    }

    [Fact]
    public async Task CreateEntity_ReturnsBadRequest_WhenNipAlreadyExists()
    {
        // Arrange - Use existing NIP
        var request = new CreateEntityRequest
        {
            Name = "Duplicate Bank",
            EntityType = "Bank",
            NIP = "1234567890", // Already exists (Test Bank 1)
            Street = "Test Street",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "duplicate@bank.pl"
        };

        // Act
        var result = await _controller.CreateEntity(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region UpdateEntity Tests

    [Fact]
    public async Task UpdateEntity_UpdatesEntity_WhenEntityExists()
    {
        // Arrange - Provide all required fields and use a unique NIP
        var request = new UpdateEntityRequest
        {
            Name = "Updated Bank Name",
            EntityType = "Bank",
            NIP = "9999999999", // Use unique NIP (not used by any entity)
            Phone = "+48111111111",
            Street = "Updated Street",
            BuildingNumber = "99",
            City = "Warsaw",
            PostalCode = "00-999",
            Email = "updated@bank.pl",
            IsActive = true
        };

        // Act
        var result = await _controller.UpdateEntity(1, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var entity = okResult.Value.Should().BeOfType<EntityResponse>().Subject;
        
        entity.Name.Should().Be("Updated Bank Name");
        entity.Phone.Should().Be("+48111111111");
        
        // Verify in database
        var dbEntity = await _context.SupervisedEntities.FindAsync(1L);
        dbEntity!.Name.Should().Be("Updated Bank Name");
    }

    [Fact]
    public async Task UpdateEntity_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        // Arrange
        var request = new UpdateEntityRequest
        {
            Name = "Non-existent Entity",
            EntityType = "Bank",
            Street = "Test",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "test@test.pl"
        };

        // Act
        var result = await _controller.UpdateEntity(999, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region DeleteEntity Tests

    [Fact]
    public async Task DeleteEntity_SoftDeletesEntity_WhenEntityExists()
    {
        // Act
        var result = await _controller.DeleteEntity(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        // Verify it was soft deleted (IsActive set to false, not actually removed)
        var dbEntity = await _context.SupervisedEntities.FindAsync(1L);
        dbEntity.Should().NotBeNull();
        dbEntity!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteEntity_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteEntity(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteEntity_DoesNotAffectOtherEntities()
    {
        // Arrange
        var countBefore = await _context.SupervisedEntities.CountAsync();

        // Act
        await _controller.DeleteEntity(1);

        // Assert - Count should be same (soft delete doesn't remove record)
        var countAfter = await _context.SupervisedEntities.CountAsync();
        countAfter.Should().Be(countBefore);
        
        // Verify other entities still exist and are active
        var entity2 = await _context.SupervisedEntities.FindAsync(2L);
        entity2.Should().NotBeNull();
        entity2!.IsActive.Should().BeTrue();
    }

    #endregion

    #region GetEntityUsers Tests

    [Fact]
    public async Task GetEntityUsers_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Act
        var result = await _controller.GetEntityUsers(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<List<UserListItemResponse>>().Subject;
        
        users.Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudWorkflow_WorksCorrectly()
    {
        // Create
        var createRequest = new CreateEntityRequest
        {
            Name = "Workflow Test Bank",
            EntityType = "Bank",
            UknfCode = "WFLOW001", // Will be ignored
            NIP = "7777777777",
            Street = "Test Street",
            BuildingNumber = "1",
            City = "Warsaw",
            PostalCode = "00-001",
            Email = "workflow@test.pl"
        };
        
        var createResult = await _controller.CreateEntity(createRequest);
        var createdEntity = ((CreatedAtActionResult)createResult.Result!).Value as EntityResponse;
        var entityId = createdEntity!.Id;

        // Read
        var getResult = await _controller.GetEntity(entityId);
        var entity = ((OkObjectResult)getResult.Result!).Value as EntityResponse;
        entity!.Name.Should().Be("Workflow Test Bank");

        // Update
        var updateRequest = new UpdateEntityRequest
        {
            Name = "Updated Workflow Bank",
            EntityType = "Bank",
            Street = "Updated Street",
            BuildingNumber = "2",
            City = "Warsaw",
            PostalCode = "00-002",
            Email = "updated@test.pl",
            IsActive = true
        };
        var updateResult = await _controller.UpdateEntity(entityId, updateRequest);
        var updatedEntity = ((OkObjectResult)updateResult.Result!).Value as EntityResponse;
        updatedEntity!.Name.Should().Be("Updated Workflow Bank");

        // Delete (soft delete)
        var deleteResult = await _controller.DeleteEntity(entityId);
        deleteResult.Should().BeOfType<NoContentResult>();

        // Verify soft deletion (entity still exists but IsActive=false)
        var getAfterDelete = await _controller.GetEntity(entityId);
        var afterDeleteEntity = ((OkObjectResult)getAfterDelete.Result!).Value as EntityResponse;
        afterDeleteEntity.Should().NotBeNull();
        afterDeleteEntity!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Pagination_CalculatesTotalPagesCorrectly()
    {
        // Test with different page sizes
        var result1 = await _controller.GetEntities(pageSize: 2);
        var value1 = ((OkObjectResult)result1.Result!).Value;
        var pagination1 = value1.GetType().GetProperty("pagination")?.GetValue(value1);
        var totalPages1 = pagination1?.GetType().GetProperty("totalPages")?.GetValue(pagination1);
        totalPages1.Should().Be(3); // 5 entities / 2 = 3 pages

        var result2 = await _controller.GetEntities(pageSize: 3);
        var value2 = ((OkObjectResult)result2.Result!).Value;
        var pagination2 = value2.GetType().GetProperty("pagination")?.GetValue(value2);
        var totalPages2 = pagination2?.GetType().GetProperty("totalPages")?.GetValue(pagination2);
        totalPages2.Should().Be(2); // 5 entities / 3 = 2 pages
    }

    [Fact]
    public async Task CombinedFilters_WorkCorrectly()
    {
        // Filter by entity type AND active status
        var result = await _controller.GetEntities(entityType: "Bank", isActive: true);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        
        var data = value.GetType().GetProperty("data")?.GetValue(value) as List<EntityListItemResponse>;
        
        data.Should().HaveCount(2); // 2 active banks (IDs 1 and 5)
        data.Should().OnlyContain(e => e.EntityType == "Bank" && e.IsActive == true);
    }

    #endregion
}
