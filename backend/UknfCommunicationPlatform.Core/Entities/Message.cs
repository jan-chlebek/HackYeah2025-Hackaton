using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a message between users in the communication system
/// </summary>
public class Message
{
    /// <summary>
    /// Unique identifier
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
    /// Sender user ID
    /// </summary>
    public long SenderId { get; set; }

    /// <summary>
    /// Navigation property - Sender
    /// </summary>
    public User Sender { get; set; } = null!;

    /// <summary>
    /// Recipient user ID (nullable for group messages)
    /// </summary>
    public long? RecipientId { get; set; }

    /// <summary>
    /// Navigation property - Recipient
    /// </summary>
    public User? Recipient { get; set; }

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
    /// Navigation property - Parent message
    /// </summary>
    public Message? ParentMessage { get; set; }

    /// <summary>
    /// Navigation property - Child messages (replies)
    /// </summary>
    public ICollection<Message> Replies { get; set; } = new List<Message>();

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
    /// Related supervised entity ID (optional)
    /// </summary>
    public long? RelatedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Related supervised entity
    /// </summary>
    public SupervisedEntity? RelatedEntity { get; set; }

    /// <summary>
    /// Related report ID (optional)
    /// </summary>
    public long? RelatedReportId { get; set; }

    /// <summary>
    /// Navigation property - Related report
    /// </summary>
    public Report? RelatedReport { get; set; }

    /// <summary>
    /// Related case ID (optional)
    /// </summary>
    public long? RelatedCaseId { get; set; }

    /// <summary>
    /// Navigation property - Related case
    /// </summary>
    public Case? RelatedCase { get; set; }

    /// <summary>
    /// Navigation property - Message attachments
    /// </summary>
    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();

    /// <summary>
    /// Is this message cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Cancellation date
    /// </summary>
    public DateTime? CancelledAt { get; set; }
}
