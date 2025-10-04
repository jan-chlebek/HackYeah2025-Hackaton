using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a report submitted by a supervised entity
/// </summary>
public class Report
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Unique report identifier assigned after submission
    /// </summary>
    public string ReportNumber { get; set; } = string.Empty;

    /// <summary>
    /// Report file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Report file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Reporting period (e.g., Q1 2025)
    /// </summary>
    public string ReportingPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Report type/category
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Current validation status
    /// </summary>
    public ReportStatus Status { get; set; }

    /// <summary>
    /// Validation result file path
    /// </summary>
    public string? ValidationResultPath { get; set; }

    /// <summary>
    /// Error description (if any)
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Submission date
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Validation completion date
    /// </summary>
    public DateTime? ValidatedAt { get; set; }

    /// <summary>
    /// Is this a correction of another report
    /// </summary>
    public bool IsCorrection { get; set; }

    /// <summary>
    /// Original report ID (if this is a correction)
    /// </summary>
    public long? OriginalReportId { get; set; }

    /// <summary>
    /// Supervised entity that submitted the report
    /// </summary>
    public long SupervisedEntityId { get; set; }

    /// <summary>
    /// Navigation property - Supervised entity
    /// </summary>
    public SupervisedEntity SupervisedEntity { get; set; } = null!;

    /// <summary>
    /// User who submitted the report
    /// </summary>
    public long SubmittedByUserId { get; set; }

    /// <summary>
    /// Navigation property - User who submitted
    /// </summary>
    public User SubmittedBy { get; set; } = null!;

    /// <summary>
    /// Navigation property - Original report (if this is a correction)
    /// </summary>
    public Report? OriginalReport { get; set; }

    /// <summary>
    /// Navigation property - Corrections to this report
    /// </summary>
    public ICollection<Report> Corrections { get; set; } = new List<Report>();
}
