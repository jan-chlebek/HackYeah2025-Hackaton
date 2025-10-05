using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Request DTO for replying to a message
/// </summary>
public class ReplyMessageRequest
{
    /// <summary>
    /// Reply message body/content
    /// </summary>
    [Required(ErrorMessage = "Body is required")]
    [StringLength(50000, ErrorMessage = "Body cannot exceed 50000 characters")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Priority level of the reply (default: inherits from parent)
    /// </summary>
    public MessagePriority? Priority { get; set; }

    /// <summary>
    /// Optional file attachments for the reply
    /// </summary>
    public List<IFormFile>? Attachments { get; set; }
}
