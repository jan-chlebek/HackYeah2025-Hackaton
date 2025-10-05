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
        var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        // Administrators always bypass entity ownership checks
        if (userRoles.Contains("Administrator"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if user is an internal user (UKNF staff)
        if (requirement.AllowInternalUsers)
        {
            // Internal users bypass entity ownership checks
            if (userRoles.Contains("InternalUser") || userRoles.Contains("Supervisor"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        // For external users, they must have a supervised entity ID in their claims
        var entityIdClaim = context.User.FindFirst("supervised_entity_id")?.Value;

        if (!string.IsNullOrEmpty(entityIdClaim) && int.TryParse(entityIdClaim, out var userEntityId) && userEntityId > 0)
        {
            // If there's a resource with an entity ID, validate it matches the user's entity
            if (context.Resource != null)
            {
                var resourceType = context.Resource.GetType();
                var entityIdProperty = resourceType.GetProperty("SupervisedEntityId");
                
                if (entityIdProperty != null)
                {
                    var resourceEntityId = entityIdProperty.GetValue(context.Resource);
                    
                    // Convert to long for comparison
                    long resourceEntityIdValue = 0;
                    if (resourceEntityId is long longValue)
                    {
                        resourceEntityIdValue = longValue;
                    }
                    else if (resourceEntityId is int intValue)
                    {
                        resourceEntityIdValue = intValue;
                    }
                    
                    // Only succeed if the entity IDs match
                    if (resourceEntityIdValue == userEntityId)
                    {
                        context.Succeed(requirement);
                    }
                    // If IDs don't match, don't succeed (authorization fails)
                    return Task.CompletedTask;
                }
            }
            
            // If no resource or no entity ID property, succeed (basic entity context validation)
            // The actual entity ID matching will be done in the controller/service layer
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
