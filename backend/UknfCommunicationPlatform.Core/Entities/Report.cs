using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a quarterly report submission
/// </summary>
public class Report
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Auto-generated unique report number (e.g., RPT-2025-0001)
    /// </summary>
    public string ReportNumber { get; set; } = string.Empty;

    /// <summary>
    /// Original uploaded file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Quarterly reporting period
    /// </summary>
    public ReportingPeriod ReportingPeriod { get; set; }

    /// <summary>
    /// Current report validation/processing status
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Draft;

    /// <summary>
    /// Binary file content stored in database (XLSX format)
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// When the report was submitted/uploaded
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// User who submitted the report
    /// </summary>
    public long SubmittedByUserId { get; set; }

    /// <summary>
    /// Navigation property - User who submitted
    /// </summary>
    public User SubmittedBy { get; set; } = null!;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
