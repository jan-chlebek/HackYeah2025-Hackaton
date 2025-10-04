using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a user in the system (internal or external)
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email address (used for login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// PESEL (Personal Identification Number) - masked, only last 4 digits visible
    /// </summary>
    public string? PESEL { get; set; }

    /// <summary>
    /// Password hash
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User role
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Is account active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Account creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last login date
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Supervised entity ID (for external users)
    /// </summary>
    public long? SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Associated supervised entity
    /// </summary>
    public SupervisedEntity? SupervisedEntity { get; set; }

    /// <summary>
    /// Navigation property - Reports submitted by this user
    /// </summary>
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
