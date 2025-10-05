using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.Announcements;

/// <summary>
/// Request to create a new announcement
/// </summary>
public class CreateAnnouncementRequest
{
    /// <summary>
    /// Announcement title
    /// </summary>
    [Required(ErrorMessage = "Tytuł jest wymagany")]
    [StringLength(500, ErrorMessage = "Tytuł nie może przekraczać 500 znaków")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Announcement content
    /// </summary>
    [Required(ErrorMessage = "Treść jest wymagana")]
    public string Content { get; set; } = string.Empty;
}
