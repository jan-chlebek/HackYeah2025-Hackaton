using System.ComponentModel.DataAnnotations;
using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.DTOs.Requests;

/// <summary>
/// Request to submit a new report (form data fields, file sent separately)
/// </summary>
public class SubmitReportRequest
{
    /// <summary>
    /// Quarterly reporting period (Q1, Q2, Q3, Q4)
    /// </summary>
    [Required]
    public ReportingPeriod ReportingPeriod { get; set; }
}
