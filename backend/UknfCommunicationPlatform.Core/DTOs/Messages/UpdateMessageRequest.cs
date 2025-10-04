using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Request DTO for updating an existing message (typically drafts)
/// </summary>
public class UpdateMessageRequest
{
    /// <summary>
    /// Message subject
    /// </summary>
    [StringLength(500, ErrorMessage = "Subject cannot exceed 500 characters")]
    public string? Subject { get; set; }

    /// <summary>
    /// Message body/content
    /// </summary>
    [StringLength(50000, ErrorMessage = "Body cannot exceed 50000 characters")]
    public string? Body { get; set; }

    /// <summary>
    /// Recipient user ID
    /// </summary>
    public long? RecipientId { get; set; }
}
