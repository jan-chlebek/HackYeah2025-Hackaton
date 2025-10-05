namespace UknfCommunicationPlatform.Core.DTOs.Cases;

/// <summary>
/// Request to update an existing case
/// </summary>
public class UpdateCaseRequest
{
    /// <summary>
    /// Case title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Case description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Case category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Handler ID
    /// </summary>
    public long? HandlerId { get; set; }
}
