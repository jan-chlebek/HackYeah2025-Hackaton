using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.Reports;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing reports
/// </summary>
public class ReportsService
{
    private readonly ApplicationDbContext _context;

    public ReportsService(ApplicationDbContext context)
    {
        _context = context;
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
                EntityName = r.SubmittedBy.FirstName + " " + r.SubmittedBy.LastName
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
                EntityName = r.SubmittedBy.FirstName + " " + r.SubmittedBy.LastName
            })
            .FirstOrDefaultAsync();

        return report;
    }
}
