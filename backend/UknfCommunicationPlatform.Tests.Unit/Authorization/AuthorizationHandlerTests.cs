using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.Authorization;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Authorization;

/// <summary>
/// Unit tests for custom authorization handlers
/// Tests permission-based, role-based, and entity ownership authorization
/// </summary>
public class AuthorizationHandlerTests
{
    #region Permission Authorization Tests

    [Fact]
    public async Task PermissionAuthorizationHandler_WithRequiredPermission_ShouldSucceed()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("users.read");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task PermissionAuthorizationHandler_WithoutRequiredPermission_ShouldFail()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("users.delete");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task PermissionAuthorizationHandler_WithMultiplePermissions_ShouldCheckCorrectOne()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("messages.write");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", "messages.read"),
            new Claim("permission", "messages.write"),
            new Claim("permission", "reports.read")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    #endregion

    #region Role Authorization Tests

    [Fact]
    public async Task RoleAuthorizationHandler_WithRequiredRole_ShouldSucceed()
    {
        // Arrange
        var handler = new RoleAuthorizationHandler();
        var requirement = new RoleRequirement("Administrator");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task RoleAuthorizationHandler_WithoutRequiredRole_ShouldFail()
    {
        // Arrange
        var handler = new RoleAuthorizationHandler();
        var requirement = new RoleRequirement("Administrator");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "InternalUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task RoleAuthorizationHandler_WithMultipleRoles_ShouldCheckCorrectOne()
    {
        // Arrange
        var handler = new RoleAuthorizationHandler();
        var requirement = new RoleRequirement("Supervisor");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "InternalUser"),
            new Claim(ClaimTypes.Role, "Supervisor")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    #endregion

    #region Entity Ownership Authorization Tests

    [Fact]
    public async Task EntityOwnershipHandler_InternalUser_ShouldAlwaysSucceedWhenAllowed()
    {
        // Arrange
        var handler = new EntityOwnershipAuthorizationHandler();
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: true);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "InternalUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task EntityOwnershipHandler_AdminUser_ShouldAlwaysSucceed()
    {
        // Arrange
        var handler = new EntityOwnershipAuthorizationHandler();
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task EntityOwnershipHandler_ExternalUserWithMatchingEntity_ShouldSucceed()
    {
        // Arrange
        var handler = new EntityOwnershipAuthorizationHandler();
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "ExternalUser"),
            new Claim("supervised_entity_id", "123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Resource represents an entity with ID 123
        var resource = new { SupervisedEntityId = 123L };

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            resource);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task EntityOwnershipHandler_ExternalUserWithDifferentEntity_ShouldFail()
    {
        // Arrange
        var handler = new EntityOwnershipAuthorizationHandler();
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "ExternalUser"),
            new Claim("supervised_entity_id", "123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Resource represents an entity with ID 456 (different from user's entity)
        var resource = new { SupervisedEntityId = 456L };

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            resource);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task EntityOwnershipHandler_ExternalUserWithoutEntityId_ShouldFail()
    {
        // Arrange
        var handler = new EntityOwnershipAuthorizationHandler();
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "ExternalUser")
            // No supervised_entity_id claim
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var resource = new { SupervisedEntityId = 123L };

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            resource);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    #endregion

    #region Combined Requirements Tests

    [Fact]
    public async Task MultipleRequirements_AllMustBeSatisfied()
    {
        // Arrange
        var permissionHandler = new PermissionAuthorizationHandler();
        var roleHandler = new RoleAuthorizationHandler();

        var permissionReq = new PermissionRequirement("users.read");
        var roleReq = new RoleRequirement("Administrator");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new IAuthorizationRequirement[] { permissionReq, roleReq },
            user,
            null);

        // Act
        await permissionHandler.HandleAsync(context);
        await roleHandler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleRequirements_IfOneFails_OverallFails()
    {
        // Arrange
        var permissionHandler = new PermissionAuthorizationHandler();
        var roleHandler = new RoleAuthorizationHandler();

        var permissionReq = new PermissionRequirement("users.read");
        var roleReq = new RoleRequirement("Administrator");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "InternalUser"), // Wrong role
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new IAuthorizationRequirement[] { permissionReq, roleReq },
            user,
            null);

        // Act
        await permissionHandler.HandleAsync(context);
        await roleHandler.HandleAsync(context);

        // Assert - Should fail because role requirement is not met
        context.HasSucceeded.Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task AuthorizationHandler_WithNullUser_ShouldFail()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("users.read");

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            null!,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task AuthorizationHandler_WithUnauthenticatedUser_ShouldFail()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("users.read");

        var identity = new ClaimsIdentity(); // Not authenticated
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task PermissionHandler_WithEmptyPermissionName_ShouldFail()
    {
        // Arrange
        var handler = new PermissionAuthorizationHandler();
        var requirement = new PermissionRequirement("");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", "users.read")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task RoleHandler_WithCaseInsensitiveRole_ShouldSucceed()
    {
        // Arrange
        var handler = new RoleAuthorizationHandler();
        var requirement = new RoleRequirement("administrator"); // lowercase
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Administrator") // PascalCase
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            user,
            null);

        // Act
        await handler.HandleAsync(context);

        // Assert - Role comparison should be case-insensitive
        context.HasSucceeded.Should().BeTrue();
    }

    #endregion
}
