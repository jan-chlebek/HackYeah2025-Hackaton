using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UknfCommunicationPlatform.Core.DTOs.Requests;

/// <summary>
/// Request to submit a new report
/// </summary>
public class SubmitReportRequest
{
    /// <summary>
    /// Report file (Excel XLSX format)
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Reporting period (e.g., "Q1 2025")
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ReportingPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Report type/category
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Is this a correction of another report
    /// </summary>
    public bool IsCorrection { get; set; }

    /// <summary>
    /// Original report ID (required if IsCorrection = true)
    /// </summary>
    public long? OriginalReportId { get; set; }
}
