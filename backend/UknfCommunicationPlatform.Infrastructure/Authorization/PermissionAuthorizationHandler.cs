using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <summary>
    /// Handle the permission requirement
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get all permission claims from the user's token
        var userPermissions = context.User
            .FindAll("permission")
            .Select(c => c.Value)
            .ToList();

        // Check if user has the required permission
        if (userPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
