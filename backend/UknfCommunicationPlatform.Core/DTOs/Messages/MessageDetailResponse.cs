namespace UknfCommunicationPlatform.Core.DTOs.Messages;

/// <summary>
/// Detailed message response with full conversation thread
/// </summary>
public class MessageDetailResponse : MessageResponse
{
    /// <summary>
    /// List of replies to this message
    /// </summary>
    public List<MessageResponse> Replies { get; set; } = new();

    /// <summary>
    /// List of attachments
    /// </summary>
    public List<MessageAttachmentInfo> Attachments { get; set; } = new();
}

/// <summary>
/// Attachment information
/// </summary>
public class MessageAttachmentInfo
{
    /// <summary>
    /// Attachment ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MIME type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Upload date
    /// </summary>
    public DateTime UploadedAt { get; set; }
}
