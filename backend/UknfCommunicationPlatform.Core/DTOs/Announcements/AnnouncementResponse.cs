namespace UknfCommunicationPlatform.Core.DTOs.Announcements;

/// <summary>
/// Full announcement details response
/// </summary>
public class AnnouncementResponse
{
    /// <summary>
    /// Announcement ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Announcement title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Full announcement content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Creator user ID
    /// </summary>
    public long CreatedByUserId { get; set; }

    /// <summary>
    /// Creator full name
    /// </summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Whether current user has read this announcement
    /// </summary>
    public bool IsReadByCurrentUser { get; set; }

    /// <summary>
    /// When current user read this announcement (if they have)
    /// </summary>
    public DateTime? ReadAt { get; set; }
}
