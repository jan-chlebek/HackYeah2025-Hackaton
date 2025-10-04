namespace UknfCommunicationPlatform.Core.DTOs.Users;

/// <summary>
/// Request to create a new user account
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Email address (will be used for login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

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
    /// Initial password (will be hashed before storage)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Role IDs to assign to this user
    /// </summary>
    public List<int> RoleIds { get; set; } = new();

    /// <summary>
    /// Supervised entity ID (required for external users, null for UKNF staff)
    /// </summary>
    public long? SupervisedEntityId { get; set; }

    /// <summary>
    /// If true, user must change password on first login
    /// </summary>
    public bool RequirePasswordChange { get; set; } = true;
}
