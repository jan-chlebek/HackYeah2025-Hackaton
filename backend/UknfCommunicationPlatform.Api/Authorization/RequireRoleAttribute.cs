using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Api.Authorization;

/// <summary>
/// Attribute to require a specific role for endpoint access
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a new role requirement attribute
    /// </summary>
    /// <param name="role">Required role (e.g., "Administrator")</param>
    public RequireRoleAttribute(string role)
    {
        Roles = role;
    }
}
