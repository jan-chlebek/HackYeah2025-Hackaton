using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.FileLibrary;

/// <summary>
/// Request DTO for updating file library metadata (without replacing the file)
/// </summary>
public class FileLibraryUpdateMetadataRequest
{
    /// <summary>
    /// File name/title for display
    /// </summary>
    [MaxLength(500)]
    public string? Name { get; set; }

    /// <summary>
    /// File description
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// File category
    /// </summary>
    [MaxLength(250)]
    public string? Category { get; set; }
}
