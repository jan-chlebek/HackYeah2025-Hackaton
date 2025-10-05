namespace UknfCommunicationPlatform.Core.DTOs.Reports;

/// <summary>
/// Report summary for list view
/// </summary>
public class ReportDto
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
    /// Quarterly reporting period (Q1, Q2, Q3, Q4)
    /// </summary>
    public string ReportingPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Name of the entity that submitted the report (from user's associated entity)
    /// </summary>
    public string EntityName { get; set; } = string.Empty;
}
