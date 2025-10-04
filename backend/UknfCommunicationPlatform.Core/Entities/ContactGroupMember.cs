namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a contact's membership in a contact group
/// </summary>
public class ContactGroupMember
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Contact group ID
    /// </summary>
    public long ContactGroupId { get; set; }

    /// <summary>
    /// Navigation property - Contact group
    /// </summary>
    public ContactGroup ContactGroup { get; set; } = null!;

    /// <summary>
    /// Contact ID
    /// </summary>
    public long ContactId { get; set; }

    /// <summary>
    /// Navigation property - Contact
    /// </summary>
    public Contact Contact { get; set; } = null!;

    /// <summary>
    /// Added to group date
    /// </summary>
    public DateTime AddedAt { get; set; }
}
