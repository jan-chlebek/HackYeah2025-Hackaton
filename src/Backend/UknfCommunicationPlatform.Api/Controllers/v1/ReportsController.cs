using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.Requests;
using UknfCommunicationPlatform.Core.DTOs.Responses;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Manages report submissions and validation
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all reports with optional filtering
    /// </summary>
    /// <param name="entityId">Filter by supervised entity ID</param>
    /// <param name="status">Filter by report status</param>
    /// <param name="reportingPeriod">Filter by reporting period</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>List of reports</returns>
    /// <response code="200">Returns the list of reports</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReportResponse>>> GetReports(
        [FromQuery] long? entityId = null,
        [FromQuery] ReportStatus? status = null,
        [FromQuery] string? reportingPeriod = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Reports
            .Include(r => r.SupervisedEntity)
            .Include(r => r.SubmittedBy)
            .AsQueryable();

        if (entityId.HasValue)
            query = query.Where(r => r.SupervisedEntityId == entityId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (!string.IsNullOrEmpty(reportingPeriod))
            query = query.Where(r => r.ReportingPeriod == reportingPeriod);

        var reports = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReportResponse
            {
                Id = r.Id,
                ReportNumber = r.ReportNumber,
                FileName = r.FileName,
                ReportingPeriod = r.ReportingPeriod,
                ReportType = r.ReportType,
                Status = r.Status,
                StatusDescription = r.Status.ToString(),
                ErrorDescription = r.ErrorDescription,
                SubmittedAt = r.SubmittedAt,
                ValidatedAt = r.ValidatedAt,
                IsCorrection = r.IsCorrection,
                OriginalReportId = r.OriginalReportId,
                EntityName = r.SupervisedEntity.Name,
                SubmittedByName = $"{r.SubmittedBy.FirstName} {r.SubmittedBy.LastName}",
                SubmittedByEmail = r.SubmittedBy.Email
            })
            .ToListAsync();

        return Ok(reports);
    }

    /// <summary>
    /// Get a specific report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>Report details</returns>
    /// <response code="200">Returns the report</response>
    /// <response code="404">Report not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> GetReport(long id)
    {
        var report = await _context.Reports
            .Include(r => r.SupervisedEntity)
            .Include(r => r.SubmittedBy)
            .Where(r => r.Id == id)
            .Select(r => new ReportResponse
            {
                Id = r.Id,
                ReportNumber = r.ReportNumber,
                FileName = r.FileName,
                ReportingPeriod = r.ReportingPeriod,
                ReportType = r.ReportType,
                Status = r.Status,
                StatusDescription = r.Status.ToString(),
                ErrorDescription = r.ErrorDescription,
                SubmittedAt = r.SubmittedAt,
                ValidatedAt = r.ValidatedAt,
                IsCorrection = r.IsCorrection,
                OriginalReportId = r.OriginalReportId,
                EntityName = r.SupervisedEntity.Name,
                SubmittedByName = $"{r.SubmittedBy.FirstName} {r.SubmittedBy.LastName}",
                SubmittedByEmail = r.SubmittedBy.Email
            })
            .FirstOrDefaultAsync();

        if (report == null)
            return NotFound(new { message = $"Report with ID {id} not found" });

        return Ok(report);
    }

    /// <summary>
    /// Submit a new report
    /// </summary>
    /// <param name="request">Report submission data</param>
    /// <returns>Created report details</returns>
    /// <response code="201">Report submitted successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/reports
    ///     Content-Type: multipart/form-data
    ///
    ///     {
    ///         "file": "(Excel file)",
    ///         "reportingPeriod": "Q1 2025",
    ///         "reportType": "Quarterly Report",
    ///         "isCorrection": false
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportResponse>> SubmitReport([FromForm] SubmitReportRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate file type
        var allowedExtensions = new[] { ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            return BadRequest(new { message = "Only Excel files (.xlsx, .xls) are allowed" });

        // TODO: Get current user from JWT token
        // For now, using first user as placeholder
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user == null)
            return BadRequest(new { message = "User not found" });

        // Generate unique report number
        var reportNumber = $"RPT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        // TODO: Save file to storage
        var filePath = $"/reports/{reportNumber}/{request.File.FileName}";

        var report = new Report
        {
            ReportNumber = reportNumber,
            FileName = request.File.FileName,
            FilePath = filePath,
            ReportingPeriod = request.ReportingPeriod,
            ReportType = request.ReportType,
            Status = ReportStatus.Draft,
            SubmittedAt = DateTime.UtcNow,
            IsCorrection = request.IsCorrection,
            OriginalReportId = request.OriginalReportId,
            SupervisedEntityId = user.SupervisedEntityId ?? 1, // TODO: Fix when auth is implemented
            SubmittedByUserId = user.Id
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        // TODO: Trigger validation process

        _logger.LogInformation("Report {ReportNumber} submitted by user {UserId}", reportNumber, user.Id);

        var response = new ReportResponse
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            FileName = report.FileName,
            ReportingPeriod = report.ReportingPeriod,
            ReportType = report.ReportType,
            Status = report.Status,
            StatusDescription = report.Status.ToString(),
            SubmittedAt = report.SubmittedAt,
            IsCorrection = report.IsCorrection,
            OriginalReportId = report.OriginalReportId,
            EntityName = user.SupervisedEntity?.Name ?? "Unknown",
            SubmittedByName = $"{user.FirstName} {user.LastName}",
            SubmittedByEmail = user.Email
        };

        return CreatedAtAction(nameof(GetReport), new { id = report.Id }, response);
    }

    /// <summary>
    /// Update report status (UKNF employees only)
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <param name="status">New status</param>
    /// <param name="errorDescription">Error description (optional)</param>
    /// <returns>Updated report</returns>
    /// <response code="200">Status updated successfully</response>
    /// <response code="404">Report not found</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> UpdateReportStatus(
        long id,
        [FromQuery] ReportStatus status,
        [FromQuery] string? errorDescription = null)
    {
        var report = await _context.Reports
            .Include(r => r.SupervisedEntity)
            .Include(r => r.SubmittedBy)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
            return NotFound(new { message = $"Report with ID {id} not found" });

        report.Status = status;
        report.ErrorDescription = errorDescription;

        if (status == ReportStatus.ValidationSuccessful ||
            status == ReportStatus.ValidationErrors ||
            status == ReportStatus.TechnicalError)
        {
            report.ValidatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Report {ReportId} status updated to {Status}", id, status);

        var response = new ReportResponse
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            FileName = report.FileName,
            ReportingPeriod = report.ReportingPeriod,
            ReportType = report.ReportType,
            Status = report.Status,
            StatusDescription = report.Status.ToString(),
            ErrorDescription = report.ErrorDescription,
            SubmittedAt = report.SubmittedAt,
            ValidatedAt = report.ValidatedAt,
            IsCorrection = report.IsCorrection,
            OriginalReportId = report.OriginalReportId,
            EntityName = report.SupervisedEntity.Name,
            SubmittedByName = $"{report.SubmittedBy.FirstName} {report.SubmittedBy.LastName}",
            SubmittedByEmail = report.SubmittedBy.Email
        };

        return Ok(response);
    }
}
