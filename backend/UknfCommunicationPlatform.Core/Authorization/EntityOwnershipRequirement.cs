using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization requirement for checking if external user belongs to the entity they're accessing
/// </summary>
public class EntityOwnershipRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// If true, internal users (UKNF staff) bypass this check
    /// </summary>
    public bool AllowInternalUsers { get; }

    /// <summary>
    /// Creates a new entity ownership requirement
    /// </summary>
    /// <param name="allowInternalUsers">Whether internal users bypass this check</param>
    public EntityOwnershipRequirement(bool allowInternalUsers = true)
    {
        AllowInternalUsers = allowInternalUsers;
    }
}
