using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.FileLibrary;

/// <summary>
/// Request DTO for uploading a file to the library
/// </summary>
public class FileLibraryUploadRequest
{
    /// <summary>
    /// File name/title for display
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// File description (optional)
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// File category (e.g., "Templates", "Guidelines", "Legal Documents")
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The file to upload
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;
}
