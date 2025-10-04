namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Priority level for bulletin board announcements
/// </summary>
public enum AnnouncementPriority
{
    /// <summary>
    /// Low priority - Regular information
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium priority - Important information
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High priority - Urgent information, requires read confirmation
    /// </summary>
    High = 2
}
