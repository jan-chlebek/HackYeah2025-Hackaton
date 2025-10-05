using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Core.DTOs.Reports;
using UknfCommunicationPlatform.Core.DTOs.Requests;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing reports
/// </summary>
public class ReportsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportsService> _logger;
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB

    public ReportsService(ApplicationDbContext context, ILogger<ReportsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all reports with optional filtering by reporting period
    /// </summary>
    /// <param name="reportingPeriod">Optional filter by reporting period (Q1, Q2, Q3, Q4)</param>
    /// <returns>List of reports</returns>
    public async Task<List<ReportDto>> GetReportsAsync(string? reportingPeriod = null)
    {
        var query = _context.Reports
            .Include(r => r.SubmittedBy)
            .AsQueryable();

        // Apply filtering if reportingPeriod is provided
        if (!string.IsNullOrWhiteSpace(reportingPeriod))
        {
            query = query.Where(r => r.ReportingPeriod.ToString() == reportingPeriod);
        }

        var reports = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                ReportNumber = r.ReportNumber,
                ReportingPeriod = r.ReportingPeriod.ToString(),
                // EntityName from the user's full name (since reports don't have direct entity relationship)
                EntityName = r.SubmittedBy.FirstName + " " + r.SubmittedBy.LastName,
                FileName = r.FileName
            })
            .ToListAsync();

        return reports;
    }

    /// <summary>
    /// Get a specific report by ID
    /// </summary>
    /// <param name="id">Report ID</param>
    /// <returns>Report details or null if not found</returns>
    public async Task<ReportDto?> GetReportByIdAsync(long id)
    {
        var report = await _context.Reports
            .Include(r => r.SubmittedBy)
            .Where(r => r.Id == id)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                ReportNumber = r.ReportNumber,
                ReportingPeriod = r.ReportingPeriod.ToString(),
                EntityName = r.SubmittedBy.FirstName + " " + r.SubmittedBy.LastName,
                FileName = r.FileName
            })
            .FirstOrDefaultAsync();

        return report;
    }

    /// <summary>
    /// Submit a new report with XLSX file attachment
    /// </summary>
    /// <param name="userId">User submitting the report</param>
    /// <param name="file">XLSX file</param>
    /// <param name="request">Report metadata</param>
    /// <returns>Created report DTO</returns>
    /// <exception cref="ArgumentException">Thrown when file validation fails</exception>
    public async Task<ReportDto> SubmitReportAsync(long userId, IFormFile file, SubmitReportRequest request)
    {
        // Validate file is provided
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Report file is required");
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (extension != ".xlsx")
        {
            throw new ArgumentException($"Invalid file type. Only .xlsx files are allowed. Received: {extension}");
        }

        // Validate MIME type
        var allowedMimeTypes = new[]
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/octet-stream" // Some browsers send this for xlsx
        };

        if (!allowedMimeTypes.Contains(file.ContentType))
        {
            throw new ArgumentException($"Invalid file content type. Expected XLSX file. Received: {file.ContentType}");
        }

        // Validate file size
        if (file.Length > MaxFileSizeBytes)
        {
            var maxSizeMB = MaxFileSizeBytes / (1024 * 1024);
            var fileSizeMB = file.Length / (1024.0 * 1024.0);
            throw new ArgumentException($"File size ({fileSizeMB:F2} MB) exceeds maximum allowed size ({maxSizeMB} MB)");
        }

        // Read file content
        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileContent = memoryStream.ToArray();
        }

        // Generate unique report number
        var year = DateTime.UtcNow.Year;
        var lastReport = await _context.Reports
            .Where(r => r.ReportNumber.StartsWith($"RPT-{year}-"))
            .OrderByDescending(r => r.ReportNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastReport != null)
        {
            var lastNumberStr = lastReport.ReportNumber.Split('-').Last();
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        var reportNumber = $"RPT-{year}-{nextNumber:D4}";

        // Create report entity
        var report = new Report
        {
            ReportNumber = reportNumber,
            FileName = file.FileName,
            FileContent = fileContent,
            ReportingPeriod = request.ReportingPeriod,
            Status = ReportStatus.Draft,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Report {ReportNumber} submitted by user {UserId} with file {FileName}",
            reportNumber, userId, file.FileName);

        // Load the user for response
        await _context.Entry(report).Reference(r => r.SubmittedBy).LoadAsync();

        return new ReportDto
        {
            Id = report.Id,
            ReportNumber = report.ReportNumber,
            ReportingPeriod = report.ReportingPeriod.ToString(),
            EntityName = report.SubmittedBy.FirstName + " " + report.SubmittedBy.LastName,
            FileName = report.FileName
        };
    }

    /// <summary>
    /// Get report file for download
    /// </summary>
    /// <param name="reportId">Report ID</param>
    /// <returns>File content and metadata</returns>
    public async Task<(byte[] FileContent, string FileName)?> GetReportFileAsync(long reportId)
    {
        var report = await _context.Reports
            .Where(r => r.Id == reportId)
            .Select(r => new { r.FileContent, r.FileName })
            .FirstOrDefaultAsync();

        if (report == null || report.FileContent == null || report.FileContent.Length == 0)
        {
            return null;
        }

        return (report.FileContent, report.FileName);
    }
}

