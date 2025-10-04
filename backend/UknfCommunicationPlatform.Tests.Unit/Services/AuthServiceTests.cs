using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Interfaces;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _mockJwtService = new Mock<IJwtService>();
        _mockPasswordHashingService = new Mock<IPasswordHashingService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _context,
            _mockJwtService.Object,
            _mockPasswordHashingService.Object,
            _mockLogger.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create password policy
        var passwordPolicy = new PasswordPolicy
        {
            Id = 1,
            MinLength = 8,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecialChar = true,
            ExpirationDays = 90,
            HistoryCount = 5,
            MaxFailedAttempts = 5,
            LockoutDurationMinutes = 15
        };
        _context.PasswordPolicies.Add(passwordPolicy);

        // Create roles
        var adminRole = new Role { Id = 1, Name = "Administrator", Description = "Admin role" };
        var userRole = new Role { Id = 2, Name = "User", Description = "User role" };
        _context.Roles.AddRange(adminRole, userRole);

        // Create permissions
        var readPermission = new Permission { Id = 1, Name = "users.read", Resource = "users", Action = "read" };
        var writePermission = new Permission { Id = 2, Name = "users.write", Resource = "users", Action = "write" };
        _context.Permissions.AddRange(readPermission, writePermission);

        // Link permissions to admin role
        _context.RolePermissions.AddRange(
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 }
        );

        // Create test user
        var testUser = new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            FailedLoginAttempts = 0
        };
        _context.Users.Add(testUser);

        // Assign role to user
        _context.UserRoles.Add(new UserRole { UserId = 1, RoleId = 1, AssignedAt = DateTime.UtcNow });

        _context.SaveChanges();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Test123!";
        var ipAddress = "127.0.0.1";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(password, "hashed_password"))
            .Returns(true);

        _mockJwtService
            .Setup(x => x.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<long?>()))
            .Returns("mock_access_token");

        _mockJwtService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("mock_refresh_token");

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be("mock_access_token");
        result.RefreshToken.Should().Be("mock_refresh_token");
        result.User.Email.Should().Be(email);
        result.User.Roles.Should().Contain("Administrator");
        result.User.Permissions.Should().Contain("users.read");
        result.User.Permissions.Should().Contain("users.write");

        // Verify refresh token was saved
        var savedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync();
        savedRefreshToken.Should().NotBeNull();
        savedRefreshToken!.Token.Should().Be("mock_refresh_token");
        savedRefreshToken.UserId.Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        var ipAddress = "127.0.0.1";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(password, "hashed_password"))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().BeNull();

        // Verify failed login attempt was recorded
        var user = await _context.Users.FindAsync(1L);
        user!.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Test123!";
        var ipAddress = "127.0.0.1";

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var user = await _context.Users.FindAsync(1L);
        user!.IsActive = false;
        await _context.SaveChangesAsync();

        var email = "test@example.com";
        var password = "Test123!";
        var ipAddress = "127.0.0.1";

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithLockedAccount_ShouldReturnNull()
    {
        // Arrange
        var user = await _context.Users.FindAsync(1L);
        user!.LockedUntil = DateTime.UtcNow.AddMinutes(10);
        await _context.SaveChangesAsync();

        var email = "test@example.com";
        var password = "Test123!";
        var ipAddress = "127.0.0.1";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(password, "hashed_password"))
            .Returns(true);

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_AfterMaxFailedAttempts_ShouldLockAccount()
    {
        // Arrange
        var user = await _context.Users.FindAsync(1L);
        user!.FailedLoginAttempts = 4; // One more will lock the account
        await _context.SaveChangesAsync();

        var email = "test@example.com";
        var password = "WrongPassword";
        var ipAddress = "127.0.0.1";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(password, "hashed_password"))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(email, password, ipAddress);

        // Assert
        result.Should().BeNull();

        var lockedUser = await _context.Users.FindAsync(1L);
        lockedUser!.FailedLoginAttempts.Should().Be(5);
        lockedUser.LockedUntil.Should().NotBeNull();
        lockedUser.LockedUntil.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LogoutAsync_ShouldRevokeAllRefreshTokens()
    {
        // Arrange
        var userId = 1L;
        var ipAddress = "127.0.0.1";

        // Create some refresh tokens
        _context.RefreshTokens.AddRange(
            new RefreshToken
            {
                Id = 1,
                UserId = userId,
                Token = "token1",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            },
            new RefreshToken
            {
                Id = 2,
                UserId = userId,
                Token = "token2",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LogoutAsync(userId, ipAddress);

        // Assert
        result.Should().BeTrue();

        var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        tokens.Should().AllSatisfy(token =>
        {
            token.RevokedAt.Should().NotBeNull();
            token.RevokedByIp.Should().Be(ipAddress);
            token.RevocationReason.Should().Be("User logout");
        });
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ShouldUpdatePassword()
    {
        // Arrange
        var userId = 1L;
        var currentPassword = "OldPassword123!";
        var newPassword = "NewPassword123!";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(currentPassword, "hashed_password"))
            .Returns(true);

        _mockPasswordHashingService
            .Setup(x => x.HashPassword(newPassword))
            .Returns("new_hashed_password");

        // Act
        var result = await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);

        // Assert
        result.Should().BeTrue();

        var user = await _context.Users.FindAsync(userId);
        user!.PasswordHash.Should().Be("new_hashed_password");
        user.LastPasswordChangeAt.Should().NotBeNull();
        user.RequirePasswordChange.Should().BeFalse();

        // Verify old password was added to history
        var passwordHistory = await _context.PasswordHistories
            .Where(ph => ph.UserId == userId)
            .FirstOrDefaultAsync();
        passwordHistory.Should().NotBeNull();
        passwordHistory!.PasswordHash.Should().Be("hashed_password");
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1L;
        var currentPassword = "WrongPassword";
        var newPassword = "NewPassword123!";

        _mockPasswordHashingService
            .Setup(x => x.VerifyPassword(currentPassword, "hashed_password"))
            .Returns(false);

        // Act
        var result = await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAccountLockedAsync_WithLockedAccount_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1L;
        var user = await _context.Users.FindAsync(userId);
        user!.LockedUntil = DateTime.UtcNow.AddMinutes(10);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.IsAccountLockedAsync(userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAccountLockedAsync_WithUnlockedAccount_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1L;

        // Act
        var result = await _authService.IsAccountLockedAsync(userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UnlockAccountAsync_ShouldResetLockoutFields()
    {
        // Arrange
        var userId = 1L;
        var user = await _context.Users.FindAsync(userId);
        user!.LockedUntil = DateTime.UtcNow.AddMinutes(10);
        user.FailedLoginAttempts = 5;
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.UnlockAccountAsync(userId);

        // Assert
        result.Should().BeTrue();

        var unlockedUser = await _context.Users.FindAsync(userId);
        unlockedUser!.LockedUntil.Should().BeNull();
        unlockedUser.FailedLoginAttempts.Should().Be(0);
    }
}
