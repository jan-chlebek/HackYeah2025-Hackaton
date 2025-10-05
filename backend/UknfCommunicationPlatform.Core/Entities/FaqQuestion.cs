namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// FAQ Question with Answer (simplified, editable structure)
/// </summary>
public class FaqQuestion
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Question text
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Answer text (can be HTML from WYSIWYG editor)
    /// </summary>
    public string Answer { get; set; } = string.Empty;
}
