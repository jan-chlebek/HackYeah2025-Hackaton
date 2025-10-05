namespace UknfCommunicationPlatform.Core.DTOs.Cases;

/// <summary>
/// Request to create a new case
/// </summary>
public class CreateCaseRequest
{
    /// <summary>
    /// Case title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Case description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Case category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Supervised entity ID
    /// </summary>
    public long SupervisedEntityId { get; set; }

    /// <summary>
    /// Priority level (1=Low, 2=Medium, 3=High)
    /// </summary>
    public int Priority { get; set; } = 2;
}
