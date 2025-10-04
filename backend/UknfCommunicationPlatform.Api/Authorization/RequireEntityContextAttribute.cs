using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Api.Authorization;

/// <summary>
/// Attribute to require entity context (external users must have supervised entity)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireEntityContextAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a new entity context requirement attribute
    /// </summary>
    /// <param name="allowInternalUsers">Whether internal users bypass this check (default: true)</param>
    public RequireEntityContextAttribute(bool allowInternalUsers = true)
    {
        Policy = allowInternalUsers ? "EntityContext_AllowInternal" : "EntityContext_Strict";
    }
}
