using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.Authorization;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Authorization;

public class RoleAuthorizationHandlerTests
{
    private readonly RoleAuthorizationHandler _handler;

    public RoleAuthorizationHandlerTests()
    {
        _handler = new RoleAuthorizationHandler();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMatchingRole_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new RoleRequirement("Administrator");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithoutMatchingRole_ShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "User")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new RoleRequirement("Administrator");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMultipleRoles_ShouldCheckCorrectly()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Moderator"),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new RoleRequirement("Moderator");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithNoRoles_ShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new RoleRequirement("Administrator");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }
}
