namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a contact in the address book
/// </summary>
public class Contact
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Contact name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact position/role
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Mobile phone number
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// Related supervised entity ID
    /// </summary>
    public long? SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Supervised entity
    /// </summary>
    public SupervisedEntity? SupervisedEntity { get; set; }

    /// <summary>
    /// Department/division
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Is primary contact
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Created by user ID
    /// </summary>
    public long CreatedByUserId { get; set; }

    /// <summary>
    /// Navigation property - Creator
    /// </summary>
    public User CreatedBy { get; set; } = null!;

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property - Contact group memberships
    /// </summary>
    public ICollection<ContactGroupMember> ContactGroupMemberships { get; set; } = new List<ContactGroupMember>();
}
