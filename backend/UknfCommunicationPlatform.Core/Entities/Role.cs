namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a role in the system with associated permissions.
/// Roles are assigned to users to control access to resources.
/// </summary>
public class Role
{
    /// <summary>
    /// Unique identifier for the role
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the role (e.g., "SystemAdmin", "EntityAdmin")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the role's purpose and responsibilities
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this is a system-defined role that cannot be deleted
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// When the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Permissions assigned to this role
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
