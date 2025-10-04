using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Responses;

/// <summary>
/// Response containing report details
/// </summary>
public class ReportResponse
{
    /// <summary>
    /// Report unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Unique report number assigned after submission
    /// </summary>
    public string ReportNumber { get; set; } = string.Empty;

    /// <summary>
    /// Report file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Reporting period
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
    /// Status description
    /// </summary>
    public string StatusDescription { get; set; } = string.Empty;

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
    /// Is this a correction
    /// </summary>
    public bool IsCorrection { get; set; }

    /// <summary>
    /// Original report ID (if correction)
    /// </summary>
    public long? OriginalReportId { get; set; }

    /// <summary>
    /// Supervised entity name
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// User who submitted (name)
    /// </summary>
    public string SubmittedByName { get; set; } = string.Empty;

    /// <summary>
    /// User who submitted (email)
    /// </summary>
    public string SubmittedByEmail { get; set; } = string.Empty;
}
