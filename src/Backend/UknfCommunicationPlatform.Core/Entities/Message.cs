namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a message between users
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
    /// Message body
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
    /// Recipient user ID
    /// </summary>
    public long RecipientId { get; set; }

    /// <summary>
    /// Navigation property - Recipient
    /// </summary>
    public User Recipient { get; set; } = null!;

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
    /// Related entity ID (optional)
    /// </summary>
    public long? RelatedEntityId { get; set; }

    /// <summary>
    /// Related report ID (optional)
    /// </summary>
    public long? RelatedReportId { get; set; }
}
