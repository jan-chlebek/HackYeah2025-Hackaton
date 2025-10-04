using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization handler for entity ownership validation
/// Ensures external users can only access their own entity's data
/// </summary>
public class EntityOwnershipAuthorizationHandler : AuthorizationHandler<EntityOwnershipRequirement>
{
    /// <summary>
    /// Handle the entity ownership requirement
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EntityOwnershipRequirement requirement)
    {
        // Check if user is an internal user (UKNF staff)
        if (requirement.AllowInternalUsers)
        {
            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Internal users bypass entity ownership checks
            if (userRoles.Contains("Administrator") ||
                userRoles.Contains("InternalUser") ||
                userRoles.Contains("Supervisor"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        // For external users, they must have a supervised entity ID in their claims
        var entityIdClaim = context.User.FindFirst("supervised_entity_id")?.Value;

        if (!string.IsNullOrEmpty(entityIdClaim))
        {
            // User has an entity context - they can access entity-specific resources
            // The actual entity ID matching will be done in the controller/service layer
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
