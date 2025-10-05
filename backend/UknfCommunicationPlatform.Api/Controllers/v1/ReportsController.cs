using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UknfCommunicationPlatform.Core.DTOs.Reports;
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
}
