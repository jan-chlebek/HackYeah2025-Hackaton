namespace UknfCommunicationPlatform.Core.DTOs.FileLibrary;

/// <summary>
/// Response DTO for file library item
/// </summary>
public class FileLibraryResponse
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
    /// Human-readable file size (e.g., "2.5 MB")
    /// </summary>
    public string FileSizeFormatted { get; set; } = string.Empty;

    /// <summary>
    /// MIME type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Upload date
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Uploader user ID
    /// </summary>
    public long UploadedByUserId { get; set; }

    /// <summary>
    /// Uploader name
    /// </summary>
    public string UploadedByName { get; set; } = string.Empty;

    /// <summary>
    /// Uploader email
    /// </summary>
    public string UploadedByEmail { get; set; } = string.Empty;

    /// <summary>
    /// Number of permissions assigned to this file
    /// </summary>
    public int PermissionCount { get; set; }
}
