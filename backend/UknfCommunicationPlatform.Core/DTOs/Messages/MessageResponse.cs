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
    /// Priority level of the message
    /// </summary>
    public MessagePriority Priority { get; set; }

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
    /// Number of attachments
    /// </summary>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// Parent message ID (if this is a reply)
    /// </summary>
    public long? ParentMessageId { get; set; }

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
