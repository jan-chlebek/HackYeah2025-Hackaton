namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Junction table linking users to roles (many-to-many relationship)
/// Tracks when a role was assigned to a user
/// </summary>
public class UserRole
{
    /// <summary>
    /// Foreign key to User
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Foreign key to Role
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// When this role was assigned to the user
    /// </summary>
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// The user who has this role
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The role assigned to the user
    /// </summary>
    public Role Role { get; set; } = null!;
}
