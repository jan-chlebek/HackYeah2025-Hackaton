namespace UknfCommunicationPlatform.Core.DTOs.Announcements;

/// <summary>
/// Announcement list item (with content preview)
/// </summary>
public class AnnouncementListItemResponse
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
    /// Content preview (first 200 characters)
    /// </summary>
    public string ContentPreview { get; set; } = string.Empty;

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
}
