using Microsoft.AspNetCore.Authorization;
using UknfCommunicationPlatform.Core.Authorization;

namespace UknfCommunicationPlatform.Api.Authorization;

/// <summary>
/// Attribute to require a specific permission for endpoint access
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a new permission requirement attribute
    /// </summary>
    /// <param name="permission">Required permission (e.g., "users.read")</param>
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission_{permission}";
    }
}
