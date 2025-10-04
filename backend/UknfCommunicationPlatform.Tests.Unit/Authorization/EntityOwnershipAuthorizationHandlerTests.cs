using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.Authorization;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Authorization;

public class EntityOwnershipAuthorizationHandlerTests
{
    private readonly EntityOwnershipAuthorizationHandler _handler;

    public EntityOwnershipAuthorizationHandlerTests()
    {
        _handler = new EntityOwnershipAuthorizationHandler();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithAdministratorRole_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: true);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithInternalUserRole_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "InternalUser")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: true);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithSupervisedEntityId_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "ExternalUser"),
            new Claim("supervised_entity_id", "123")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: true);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithoutSupervisedEntityId_ShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "ExternalUser")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: true);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_StrictMode_InternalUserShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "InternalUser")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_StrictMode_WithSupervisedEntityId_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "ExternalUser"),
            new Claim("supervised_entity_id", "456")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new EntityOwnershipRequirement(allowInternalUsers: false);
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }
}
