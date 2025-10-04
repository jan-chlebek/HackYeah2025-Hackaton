using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using UknfCommunicationPlatform.Core.DTOs.Users;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;
using UserRoleEnum = UknfCommunicationPlatform.Core.Enums.UserRole;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for UserManagementService
/// </summary>
public class UserManagementServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IPasswordHashingService> _mockPasswordService;
    private readonly UserManagementService _sut;

    public UserManagementServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockPasswordService = new Mock<IPasswordHashingService>();
        _sut = new UserManagementService(_context, _mockPasswordService.Object);

        // Setup default mock behavior
        _mockPasswordService
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns((string pwd) => $"hashed_{pwd}");
        
        _mockPasswordService
            .Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string pwd, string hash) => hash == $"hashed_{pwd}");
    }

    [Fact]
    public async Task GetUsersAsync_WithoutFilters_ShouldReturnAllUsers()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var (users, totalCount) = await _sut.GetUsersAsync(page: 1, pageSize: 10);

        // Assert
        users.Should().HaveCount(3);
        totalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetUsersAsync_WithSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var (users, totalCount) = await _sut.GetUsersAsync(
            page: 1, 
            pageSize: 10, 
            searchTerm: "john");

        // Assert
        users.Should().HaveCount(1);
        users.First().FullName.Should().Contain("John");
        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetUsersAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedUsersAsync();

        // Act
        var (users, totalCount) = await _sut.GetUsersAsync(page: 2, pageSize: 1);

        // Assert
        users.Should().HaveCount(1);
        totalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ShouldReturnUser()
    {
        // Arrange
        var user = await SeedSingleUserAsync();

        // Act
        var result = await _sut.GetUserByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_ValidData_ShouldCreateUser()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!",
            RoleIds = new List<int>()
        };

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        
        _mockPasswordService.Verify(x => x.HashPassword("Password123!"), Times.Once);
        
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateEmail_ShouldThrowException()
    {
        // Arrange
        await SeedSingleUserAsync();
        
        var request = new CreateUserRequest
        {
            Email = "john.doe@example.com",
            FirstName = "Another",
            LastName = "John",
            Password = "Password123!",
            RoleIds = new List<int>()
        };

        // Act
        var act = async () => await _sut.CreateUserAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_ShouldSoftDelete()
    {
        // Arrange
        var user = await SeedSingleUserAsync();

        // Act
        var result = await _sut.DeleteUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedUser = await _context.Users.FindAsync(user.Id);
        deletedUser!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ActivateUserAsync_InactiveUser_ShouldActivate()
    {
        // Arrange
        var user = await SeedSingleUserAsync();
        user.IsActive = false;
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ActivateUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        
        var activatedUser = await _context.Users.FindAsync(user.Id);
        activatedUser!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateUserAsync_ActiveUser_ShouldDeactivate()
    {
        // Arrange
        var user = await SeedSingleUserAsync();

        // Act
        var result = await _sut.DeactivateUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        
        var deactivatedUser = await _context.Users.FindAsync(user.Id);
        deactivatedUser!.IsActive.Should().BeFalse();
    }

    private async Task<User> SeedSingleUserAsync()
    {
        var user = new User
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "hashed_password",
            Phone = "123456789",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = "hashed_password",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                PasswordHash = "hashed_password",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Email = "inactive@example.com",
                FirstName = "Inactive",
                LastName = "User",
                PasswordHash = "hashed_password",
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
