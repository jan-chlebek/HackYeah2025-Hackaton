namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a file attachment for an announcement
/// </summary>
public class AnnouncementAttachment
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
    /// Navigation property - Parent announcement
    /// </summary>
    public Announcement Announcement { get; set; } = null!;

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File path on storage
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Upload date
    /// </summary>
    public DateTime UploadedAt { get; set; }
}
