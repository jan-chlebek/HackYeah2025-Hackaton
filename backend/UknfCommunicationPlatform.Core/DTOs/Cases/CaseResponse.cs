using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Cases;

/// <summary>
/// Case response DTO
/// </summary>
public class CaseResponse
{
    public long Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public CaseStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public int Priority { get; set; }

    // Entity info
    public long SupervisedEntityId { get; set; }
    public string SupervisedEntityName { get; set; } = string.Empty;

    // Handler info
    public long? HandlerId { get; set; }
    public string? HandlerName { get; set; }

    // Creator info
    public long CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;

    // Dates
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Cancellation
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}
