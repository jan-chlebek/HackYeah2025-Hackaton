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
    /// Navigation property - Message attachments
    /// </summary>
    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();

    /// <summary>
    /// Is this message cancelled
    /// </summary>
    public bool IsCancelled { get; set; }
}
