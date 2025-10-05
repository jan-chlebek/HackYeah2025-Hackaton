using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents an administrative case concerning a supervised entity
/// </summary>
public class Case
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Case number
    /// </summary>
    public string CaseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Case title/name
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Case description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Case category (e.g., compliance, audit, investigation)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public CaseStatus Status { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Supervised entity ID
    /// </summary>
    public long SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Supervised entity
    /// </summary>
    public SupervisedEntity SupervisedEntity { get; set; } = null!;

    /// <summary>
    /// Case handler (UKNF employee) ID
    /// </summary>
    public long? HandlerId { get; set; }

    /// <summary>
    /// Navigation property - Case handler
    /// </summary>
    public User? Handler { get; set; }

    /// <summary>
    /// Created by user ID
    /// </summary>
    public long CreatedByUserId { get; set; }

    /// <summary>
    /// Navigation property - Creator
    /// </summary>
    public User CreatedBy { get; set; } = null!;

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Closed date
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// Is case cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Cancellation date
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Cancellation reason
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Navigation property - Case documents
    /// </summary>
    public ICollection<CaseDocument> Documents { get; set; } = new List<CaseDocument>();

    /// <summary>
    /// Navigation property - Case history
    /// </summary>
    public ICollection<CaseHistory> History { get; set; } = new List<CaseHistory>();
}
