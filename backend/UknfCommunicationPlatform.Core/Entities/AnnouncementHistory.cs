namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Tracks changes to announcements
/// </summary>
public class AnnouncementHistory
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
    /// Change type (e.g., "Created", "Published", "Updated", "Unpublished", "Deleted")
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the change
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Changed by user ID
    /// </summary>
    public long ChangedByUserId { get; set; }

    /// <summary>
    /// Navigation property - User who made the change
    /// </summary>
    public User ChangedBy { get; set; } = null!;

    /// <summary>
    /// Change timestamp
    /// </summary>
    public DateTime ChangedAt { get; set; }
}
