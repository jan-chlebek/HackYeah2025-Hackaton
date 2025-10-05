using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Api.Models;

/// <summary>
/// Form model for submitting a report with file upload
/// </summary>
public class SubmitReportFormModel
{
    /// <summary>
    /// XLSX file containing the report data (required, max 50MB)
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// Quarterly reporting period (Q1, Q2, Q3, Q4)
    /// </summary>
    [Required]
    public string ReportingPeriod { get; set; } = null!;
}
