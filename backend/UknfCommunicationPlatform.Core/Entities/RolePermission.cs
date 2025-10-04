namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Junction table linking roles to permissions (many-to-many relationship)
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Foreign key to Role
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Foreign key to Permission
    /// </summary>
    public int PermissionId { get; set; }

    // Navigation properties
    /// <summary>
    /// The role that has this permission
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// The permission granted to the role
    /// </summary>
    public Permission Permission { get; set; } = null!;
}
