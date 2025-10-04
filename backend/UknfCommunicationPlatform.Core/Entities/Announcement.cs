using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a bulletin board announcement
/// </summary>
public class Announcement
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Announcement title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Announcement content (WYSIWYG HTML)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Category (e.g., "General Information", "Events", "Regulatory Changes")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Priority level
    /// </summary>
    public AnnouncementPriority Priority { get; set; }

    /// <summary>
    /// Is announcement published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Publication date
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Expiration date (announcement hidden after this date)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Created by user ID (UKNF staff)
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
    /// Navigation property - Attachments
    /// </summary>
    public ICollection<AnnouncementAttachment> Attachments { get; set; } = new List<AnnouncementAttachment>();

    /// <summary>
    /// Navigation property - Read confirmations
    /// </summary>
    public ICollection<AnnouncementRead> ReadConfirmations { get; set; } = new List<AnnouncementRead>();

    /// <summary>
    /// Navigation property - Recipient assignments
    /// </summary>
    public ICollection<AnnouncementRecipient> Recipients { get; set; } = new List<AnnouncementRecipient>();

    /// <summary>
    /// Navigation property - Change history
    /// </summary>
    public ICollection<AnnouncementHistory> History { get; set; } = new List<AnnouncementHistory>();
}
