using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a history entry for a case tracking changes
/// </summary>
public class CaseHistory
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Case ID
    /// </summary>
    public long CaseId { get; set; }

    /// <summary>
    /// Navigation property - Parent case
    /// </summary>
    public Case Case { get; set; } = null!;

    /// <summary>
    /// Change type/action (e.g., "Status Changed", "Document Added", "Message Sent")
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;

    /// <summary>
    /// Old status (if status change)
    /// </summary>
    public CaseStatus? OldStatus { get; set; }

    /// <summary>
    /// New status (if status change)
    /// </summary>
    public CaseStatus? NewStatus { get; set; }

    /// <summary>
    /// Description of the change
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Changed by user ID
    /// </summary>
    public long ChangedByUserId { get; set; }

    /// <summary>
    /// Navigation property - User who made the change
    /// </summary>
    public User ChangedBy { get; set; } = null!;

    /// <summary>
    /// Change timestamp
    /// </summary>
    public DateTime ChangedAt { get; set; }
}
