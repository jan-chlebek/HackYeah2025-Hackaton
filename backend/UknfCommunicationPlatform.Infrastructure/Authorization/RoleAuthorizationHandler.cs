using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization handler for role-based access control
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    /// <summary>
    /// Handle the role requirement
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        // Get all role claims from the user's token
        var userRoles = context.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check if user has the required role (case-insensitive)
        if (userRoles.Any(role => role.Equals(requirement.Role, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
