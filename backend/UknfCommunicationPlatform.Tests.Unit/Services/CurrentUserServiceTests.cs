using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UknfCommunicationPlatform.Infrastructure.Services;
using NSubstitute;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

public class CurrentUserServiceTests
{
    private IHttpContextAccessor CreateHttpContextAccessor(ClaimsPrincipal? user)
    {
        var httpContext = new DefaultHttpContext
        {
            User = user ?? new ClaimsPrincipal()
        };

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(httpContext);
        return httpContextAccessor;
    }

    [Fact]
    public void UserId_WithValidClaim_ShouldReturnUserId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.UserId;

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public void UserId_WithoutClaim_ShouldReturnNull()
    {
        // Arrange
        var httpContextAccessor = CreateHttpContextAccessor(new ClaimsPrincipal());
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.UserId;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Email_WithValidClaim_ShouldReturnEmail()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.Email;

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void SupervisedEntityId_WithValidClaim_ShouldReturnEntityId()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("supervised_entity_id", "456")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.SupervisedEntityId;

        // Assert
        result.Should().Be(456);
    }

    [Fact]
    public void Roles_WithMultipleRoles_ShouldReturnAllRoles()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim(ClaimTypes.Role, "InternalUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.Roles;

        // Assert
        result.Should().Contain(new[] { "Administrator", "InternalUser" });
    }

    [Fact]
    public void Permissions_WithMultiplePermissions_ShouldReturnAllPermissions()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.Permissions;

        // Assert
        result.Should().Contain(new[] { "users.read", "users.write" });
    }

    [Fact]
    public void IsAuthenticated_WithAuthenticatedUser_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsAuthenticated;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_WithUnauthenticatedUser_ShouldReturnFalse()
    {
        // Arrange
        var httpContextAccessor = CreateHttpContextAccessor(new ClaimsPrincipal());
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsAuthenticated;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasPermission_WithMatchingPermission_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[] { new Claim("permission", "users.read") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.HasPermission("users.read");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasPermission_WithoutMatchingPermission_ShouldReturnFalse()
    {
        // Arrange
        var claims = new[] { new Claim("permission", "users.read") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.HasPermission("users.write");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasRole_WithMatchingRole_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Administrator") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.HasRole("Administrator");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalUser_WithAdministratorRole_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "Administrator") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsInternalUser;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalUser_WithInternalUserRole_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "InternalUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsInternalUser;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInternalUser_WithExternalUserRole_ShouldReturnFalse()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "ExternalUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsInternalUser;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsExternalUser_WithExternalRoleAndEntityId_ShouldReturnTrue()
    {
        // Arrange
        var claims = new[]
        {
            new Claim(ClaimTypes.Role, "ExternalUser"),
            new Claim("supervised_entity_id", "123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsExternalUser;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsExternalUser_WithExternalRoleButNoEntityId_ShouldReturnFalse()
    {
        // Arrange
        var claims = new[] { new Claim(ClaimTypes.Role, "ExternalUser") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContextAccessor = CreateHttpContextAccessor(user);
        var service = new CurrentUserService(httpContextAccessor);

        // Act
        var result = service.IsExternalUser;

        // Assert
        result.Should().BeFalse();
    }
}
