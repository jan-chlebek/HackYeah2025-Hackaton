namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a granular permission that can be assigned to roles.
/// Permissions follow a resource.action pattern (e.g., "users.create", "reports.view")
/// </summary>
public class Permission
{
    /// <summary>
    /// Unique identifier for the permission
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Full permission name (e.g., "users.create", "reports.view")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Resource this permission applies to (e.g., "users", "reports", "entities")
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// Action allowed on the resource (e.g., "create", "read", "update", "delete")
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this permission allows
    /// </summary>
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    /// <summary>
    /// Roles that have this permission
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
