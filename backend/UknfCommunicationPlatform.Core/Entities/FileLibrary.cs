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
    /// File category (e.g., "Templates", "Guidelines", "Legal Documents")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Tags for search
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Version number
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Is this the current version
    /// </summary>
    public bool IsCurrentVersion { get; set; } = true;

    /// <summary>
    /// Parent file ID (for versioning)
    /// </summary>
    public long? ParentFileId { get; set; }

    /// <summary>
    /// Navigation property - Parent file
    /// </summary>
    public FileLibrary? ParentFile { get; set; }

    /// <summary>
    /// Navigation property - File versions
    /// </summary>
    public ICollection<FileLibrary> Versions { get; set; } = new List<FileLibrary>();

    /// <summary>
    /// Is file public (visible to all)
    /// </summary>
    public bool IsPublic { get; set; }

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
    /// Download count
    /// </summary>
    public int DownloadCount { get; set; }

    /// <summary>
    /// Navigation property - Access permissions
    /// </summary>
    public ICollection<FileLibraryPermission> Permissions { get; set; } = new List<FileLibraryPermission>();
}
