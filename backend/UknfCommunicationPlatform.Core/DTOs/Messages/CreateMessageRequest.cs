using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Request DTO for creating a new message
/// </summary>
public class CreateMessageRequest
{
    /// <summary>
    /// Message subject
    /// </summary>
    [Required(ErrorMessage = "Subject is required")]
    [StringLength(500, ErrorMessage = "Subject cannot exceed 500 characters")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Message body/content
    /// </summary>
    [Required(ErrorMessage = "Body is required")]
    [StringLength(50000, ErrorMessage = "Body cannot exceed 50000 characters")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Recipient user ID (optional for drafts)
    /// </summary>
    public long? RecipientId { get; set; }

    /// <summary>
    /// Message folder classification
    /// </summary>
    public MessageFolder Folder { get; set; } = MessageFolder.Inbox;

    /// <summary>
    /// Thread ID for conversation grouping
    /// </summary>
    public long? ThreadId { get; set; }

    /// <summary>
    /// Parent message ID (if this is a reply)
    /// </summary>
    public long? ParentMessageId { get; set; }

    /// <summary>
    /// Related supervised entity ID (optional)
    /// </summary>
    public long? RelatedEntityId { get; set; }

    /// <summary>
    /// Related report ID (optional)
    /// </summary>
    public long? RelatedReportId { get; set; }

    /// <summary>
    /// Related case ID (optional)
    /// </summary>
    public long? RelatedCaseId { get; set; }

    /// <summary>
    /// Whether to send immediately (true) or save as draft (false)
    /// </summary>
    public bool SendImmediately { get; set; } = true;

    /// <summary>
    /// Optional file attachments (multiple files allowed, can be empty)
    /// </summary>
    public List<IFormFile>? Attachments { get; set; }
}
