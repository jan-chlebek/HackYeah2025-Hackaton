namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a file in the library/repository
/// </summary>
public class FileLibrary
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// File name/title
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// File description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Original file name
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
    /// File category (e.g., "Templates", "Guidelines", "Legal Documents")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Binary content of the file stored as BLOB
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Uploaded by user ID
    /// </summary>
    public long UploadedByUserId { get; set; }

    /// <summary>
    /// Navigation property - Uploader
    /// </summary>
    public User UploadedBy { get; set; } = null!;

    /// <summary>
    /// Upload date
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Navigation property - Access permissions
    /// </summary>
    public ICollection<FileLibraryPermission> Permissions { get; set; } = new List<FileLibraryPermission>();
}
