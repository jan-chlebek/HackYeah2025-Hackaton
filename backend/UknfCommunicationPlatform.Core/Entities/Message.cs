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

    // Polish UI fields
    /// <summary>
    /// Identyfikator wiadomości (e.g., "2024/System14/5")
    /// </summary>
    public string? Identyfikator { get; set; }

    /// <summary>
    /// Sygnatura sprawy (e.g., "001/2025")
    /// </summary>
    public string? SygnaturaSprawy { get; set; }

    /// <summary>
    /// Nazwa podmiotu
    /// </summary>
    public string? Podmiot { get; set; }

    /// <summary>
    /// Status wiadomości w języku polskim
    /// </summary>
    public string? StatusWiadomosci { get; set; }

    /// <summary>
    /// Priorytet wiadomości (Wysoki, Średni, Niski)
    /// </summary>
    public string? Priorytet { get; set; }

    /// <summary>
    /// Data przesłania przez podmiot
    /// </summary>
    public DateTime? DataPrzeslaniaPodmiotu { get; set; }

    /// <summary>
    /// Nazwa użytkownika (podmiotu)
    /// </summary>
    public string? Uzytkownik { get; set; }

    /// <summary>
    /// Wiadomość od użytkownika
    /// </summary>
    public string? WiadomoscUzytkownika { get; set; }

    /// <summary>
    /// Data przesłania odpowiedzi UKNF
    /// </summary>
    public DateTime? DataPrzeslaniaUKNF { get; set; }

    /// <summary>
    /// Pracownik UKNF obsługujący sprawę
    /// </summary>
    public string? PracownikUKNF { get; set; }

    /// <summary>
    /// Wiadomość od pracownika UKNF
    /// </summary>
    public string? WiadomoscPracownikaUKNF { get; set; }
}
