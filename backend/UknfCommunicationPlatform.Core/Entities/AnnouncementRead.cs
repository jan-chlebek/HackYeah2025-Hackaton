namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Tracks when users read announcements (for high-priority confirmations)
/// </summary>
public class AnnouncementRead
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
    /// User ID who read the announcement
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Read timestamp
    /// </summary>
    public DateTime ReadAt { get; set; }

    /// <summary>
    /// IP address of the reader
    /// </summary>
    public string? IpAddress { get; set; }
}
