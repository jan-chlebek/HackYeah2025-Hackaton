using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization requirement for checking if user has a specific permission
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Required permission name (e.g., "users.read", "reports.write")
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// Creates a new permission requirement
    /// </summary>
    /// <param name="permission">Permission name</param>
    public PermissionRequirement(string permission)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}
