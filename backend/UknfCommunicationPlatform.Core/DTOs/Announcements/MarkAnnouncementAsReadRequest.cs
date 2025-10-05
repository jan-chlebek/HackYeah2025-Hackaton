namespace UknfCommunicationPlatform.Core.DTOs.Announcements;

/// <summary>
/// Request to mark an announcement as read (optional IP tracking)
/// </summary>
public class MarkAnnouncementAsReadRequest
{
    /// <summary>
    /// Optional: Client IP address for tracking
    /// </summary>
    public string? IpAddress { get; set; }
}
