namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a document attached to a case
/// </summary>
public class CaseDocument
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Case ID
    /// </summary>
    public long CaseId { get; set; }

    /// <summary>
    /// Navigation property - Parent case
    /// </summary>
    public Case Case { get; set; } = null!;

    /// <summary>
    /// Document name
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// File name
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
    /// Document description
    /// </summary>
    public string? Description { get; set; }

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
