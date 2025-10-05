namespace UknfCommunicationPlatform.Core.Enums;

/// <summary>
/// Status of a message in the communication system
/// </summary>
public enum MessageStatus
{
    /// <summary>
    /// Sent - Delivered to recipient
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Read - Opened by recipient
    /// </summary>
    Read = 2,

    /// <summary>
    /// Awaiting UKNF Response
    /// </summary>
    AwaitingUknfResponse = 3,

    /// <summary>
    /// Awaiting User Response
    /// </summary>
    AwaitingUserResponse = 4,

    /// <summary>
    /// Closed - Conversation complete
    /// </summary>
    Closed = 5
}
