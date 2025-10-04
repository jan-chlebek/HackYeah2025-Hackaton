namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a contact group for organizing recipients
/// </summary>
public class ContactGroup
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Group name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Group description
    /// </summary>
    public string? Description { get; set; }

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
    /// Navigation property - Group members
    /// </summary>
    public ICollection<ContactGroupMember> Members { get; set; } = new List<ContactGroupMember>();
}
