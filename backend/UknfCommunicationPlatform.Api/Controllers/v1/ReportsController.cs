using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UknfCommunicationPlatform.Core.DTOs.Reports;
using UknfCommunicationPlatform.Core.DTOs.Requests;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Reports management operations
/// </summary>
[ApiController]
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
[Route("api/v1/reports")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reportsService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ReportsService reportsService, ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of reports with optional filtering
    /// </summary>
    /// <param name="reportingPeriod">Filter by reporting period (Q1, Q2, Q3, Q4)</param>
    /// <returns>List of reports</returns>
    [HttpGet]
    // TODO: RE-ENABLE PERMISSION CHECK - Temporarily disabled for testing
    // [RequirePermission("reports.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<ReportDto>>> GetReports(
        [FromQuery] string? reportingPeriod = null)
    {
        try
        {
            var reports = await _reportsService.GetReportsAsync(reportingPeriod);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reports");
            return StatusCode(500, new { message = "An error occurred while retrieving reports" });
        }
    }

    /// <summary>
    /// Get a specific report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>Report details</returns>
    [HttpGet("{id}")]
    // TODO: RE-ENABLE PERMISSION CHECK - Temporarily disabled for testing
    // [RequirePermission("reports.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDto>> GetReport(long id)
    {
        try
        {
            var report = await _reportsService.GetReportByIdAsync(id);

            if (report == null)
            {
                return NotFound(new { message = $"Report with ID {id} not found" });
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report {ReportId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the report" });
        }
    }

    /// <summary>
    /// Submit a new report with XLSX file attachment
    /// </summary>
    /// <param name="model">Form data containing file and reporting period</param>
    /// <returns>Created report details</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    // TODO: RE-ENABLE PERMISSION CHECK - Temporarily disabled for testing
    // [RequirePermission("reports.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReportDto>> SubmitReport(
        [FromForm] Models.SubmitReportFormModel model)
    {
        try
        {
            // TODO: Get actual user ID from authentication context
            const long currentUserId = 2; // Hardcoded for testing

            // Parse reporting period
            if (!Enum.TryParse<Core.Enums.ReportingPeriod>(model.ReportingPeriod, out var period))
            {
                return BadRequest(new { message = $"Invalid reporting period. Must be one of: Q1, Q2, Q3, Q4. Received: {model.ReportingPeriod}" });
            }

            var request = new SubmitReportRequest
            {
                ReportingPeriod = period
            };

            var report = await _reportsService.SubmitReportAsync(currentUserId, model.File, request);

            _logger.LogInformation("Report {ReportNumber} submitted successfully", report.ReportNumber);

            return CreatedAtAction(
                nameof(GetReport),
                new { id = report.Id },
                report);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid report submission");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting report");
            return StatusCode(500, new { message = "An error occurred while submitting the report" });
        }
    }

    /// <summary>
    /// Download report file
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>XLSX file</returns>
    [HttpGet("{id}/download")]
    // TODO: RE-ENABLE PERMISSION CHECK - Temporarily disabled for testing
    // [RequirePermission("reports.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DownloadReport(long id)
    {
        try
        {
            var fileData = await _reportsService.GetReportFileAsync(id);

            if (fileData == null)
            {
                return NotFound(new { message = $"Report file with ID {id} not found" });
            }

            var (fileContent, fileName) = fileData.Value;

            _logger.LogInformation("Report {ReportId} file downloaded: {FileName}", id, fileName);

            return File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report {ReportId}", id);
            return StatusCode(500, new { message = "An error occurred while downloading the report file" });
        }
    }
}

