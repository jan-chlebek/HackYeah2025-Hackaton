using Microsoft.AspNetCore.Authorization;

namespace UknfCommunicationPlatform.Core.Authorization;

/// <summary>
/// Authorization requirement for checking if user has a specific role
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Required role name (e.g., "Administrator", "ExternalUser")
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Creates a new role requirement
    /// </summary>
    /// <param name="role">Role name</param>
    public RoleRequirement(string role)
    {
        Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}
