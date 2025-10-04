namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Defines who should receive an announcement (users, groups, podmiot types)
/// </summary>
public class AnnouncementRecipient
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Announcement ID
    /// </summary>
    public long AnnouncementId { get; set; }

    /// <summary>
    /// Navigation property - Announcement
    /// </summary>
    public Announcement Announcement { get; set; } = null!;

    /// <summary>
    /// Recipient type: "User", "SupervisedEntity", "PodmiotType", "AllExternal", "AllInternal"
    /// </summary>
    public string RecipientType { get; set; } = string.Empty;

    /// <summary>
    /// User ID (if RecipientType is "User")
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Supervised entity ID (if RecipientType is "SupervisedEntity")
    /// </summary>
    public long? SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Supervised entity
    /// </summary>
    public SupervisedEntity? SupervisedEntity { get; set; }

    /// <summary>
    /// Podmiot type filter (e.g., "Bank", "Insurance") if RecipientType is "PodmiotType"
    /// </summary>
    public string? PodmiotType { get; set; }
}
