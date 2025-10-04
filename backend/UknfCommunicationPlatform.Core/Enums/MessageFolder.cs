namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Folder categories for message organization
/// </summary>
public enum MessageFolder
{
    /// <summary>
    /// Inbox - Received messages
    /// </summary>
    Inbox = 0,

    /// <summary>
    /// Sent - Sent messages
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Drafts - Unsent messages
    /// </summary>
    Drafts = 2,

    /// <summary>
    /// Reports - Messages related to reports (Sprawozdania)
    /// </summary>
    Reports = 3,

    /// <summary>
    /// Cases - Messages related to administrative cases (Sprawy)
    /// </summary>
    Cases = 4,

    /// <summary>
    /// Applications - Messages related to access requests (Wnioski)
    /// </summary>
    Applications = 5
}
