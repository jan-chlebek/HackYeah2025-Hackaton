using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Response DTO for message data
/// </summary>
public class MessageResponse
{
    /// <summary>
    /// Message ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Message subject
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Message body/content
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Sender information
    /// </summary>
    public MessageUserInfo Sender { get; set; } = null!;

    /// <summary>
    /// Recipient information
    /// </summary>
    public MessageUserInfo? Recipient { get; set; }

    /// <summary>
    /// Current status of the message
    /// </summary>
    public MessageStatus Status { get; set; }

    /// <summary>
    /// Message folder classification
    /// </summary>
    public MessageFolder Folder { get; set; }

    /// <summary>
    /// Thread ID for conversation grouping
    /// </summary>
    public long? ThreadId { get; set; }

    /// <summary>
    /// Parent message ID (if this is a reply)
    /// </summary>
    public long? ParentMessageId { get; set; }

    /// <summary>
    /// Has the message been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Message sent date
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Message read date
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Related supervised entity ID
    /// </summary>
    public long? RelatedEntityId { get; set; }

    /// <summary>
    /// Related supervised entity name
    /// </summary>
    public string? RelatedEntityName { get; set; }

    /// <summary>
    /// Related report ID
    /// </summary>
    public long? RelatedReportId { get; set; }

    /// <summary>
    /// Related case ID
    /// </summary>
    public long? RelatedCaseId { get; set; }

    /// <summary>
    /// Number of attachments
    /// </summary>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// Number of replies
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Is this message cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Cancellation date
    /// </summary>
    public DateTime? CancelledAt { get; set; }
}

/// <summary>
/// Simplified user information for messages
/// </summary>
public class MessageUserInfo
{
    /// <summary>
    /// User ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
