using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.FileLibrary;

/// <summary>
/// Request DTO for replacing a file in the library (keeps same ID, updates content and metadata)
/// </summary>
public class FileLibraryReplaceRequest
{
    /// <summary>
    /// New file name/title for display (optional - keeps existing if not provided)
    /// </summary>
    [MaxLength(500)]
    public string? Name { get; set; }

    /// <summary>
    /// New file description (optional - keeps existing if not provided)
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// New file category (optional - keeps existing if not provided)
    /// </summary>
    [MaxLength(250)]
    public string? Category { get; set; }

    /// <summary>
    /// The new file to replace the existing one
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;
}
