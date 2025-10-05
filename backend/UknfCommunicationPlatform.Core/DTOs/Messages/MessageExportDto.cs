namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Response DTO for CSV export of messages
/// </summary>
public class MessageExportDto
{
    public long Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? RelatedEntityName { get; set; }
    public int AttachmentCount { get; set; }
    public bool IsReply { get; set; }
}
