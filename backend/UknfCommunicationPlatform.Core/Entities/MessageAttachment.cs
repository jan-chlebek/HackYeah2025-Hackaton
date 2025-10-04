namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a file attachment for a message
/// </summary>
public class MessageAttachment
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public long MessageId { get; set; }

    /// <summary>
    /// Navigation property - Parent message
    /// </summary>
    public Message Message { get; set; } = null!;

    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File path on storage
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

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

    /// <summary>
    /// Uploaded by user ID
    /// </summary>
    public long UploadedByUserId { get; set; }

    /// <summary>
    /// Navigation property - Uploader
    /// </summary>
    public User UploadedBy { get; set; } = null!;
}
