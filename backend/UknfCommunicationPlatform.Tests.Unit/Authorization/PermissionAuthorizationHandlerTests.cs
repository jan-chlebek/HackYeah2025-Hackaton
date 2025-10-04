using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.Authorization;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Authorization;

public class PermissionAuthorizationHandlerTests
{
    private readonly PermissionAuthorizationHandler _handler;

    public PermissionAuthorizationHandlerTests()
    {
        _handler = new PermissionAuthorizationHandler();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMatchingPermission_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("permission", "users.read"),
            new Claim("permission", "users.write")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new PermissionRequirement("users.read");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithoutMatchingPermission_ShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("permission", "users.read")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new PermissionRequirement("users.delete");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithMultiplePermissions_ShouldCheckCorrectly()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("permission", "users.read"),
            new Claim("permission", "reports.write"),
            new Claim("permission", "messages.send")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new PermissionRequirement("reports.write");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleRequirementAsync_WithNoPermissions_ShouldNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        var requirement = new PermissionRequirement("users.read");
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }
}
