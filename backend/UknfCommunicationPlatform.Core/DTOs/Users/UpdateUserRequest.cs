namespace UknfCommunicationPlatform.Core.DTOs.Users;

/// <summary>
/// Request to update an existing user account
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Is account active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Role IDs to assign to this user (replaces existing roles)
    /// </summary>
    public List<int> RoleIds { get; set; } = new();

    /// <summary>
    /// Supervised entity ID (null for UKNF staff)
    /// </summary>
    public long? SupervisedEntityId { get; set; }
}
