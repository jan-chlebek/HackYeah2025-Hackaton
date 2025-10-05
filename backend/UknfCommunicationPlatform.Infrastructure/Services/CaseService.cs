using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using UknfCommunicationPlatform.Core.DTOs.Cases;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing cases
/// </summary>
public class CaseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CaseService> _logger;

    // Pure projection expression so EF Core can translate entirely server-side.
    private static readonly Expression<Func<Case, CaseResponse>> CaseProjection = c => new CaseResponse
    {
        Id = c.Id,
        CaseNumber = c.CaseNumber,
        Title = c.Title,
        Description = c.Description,
        Category = c.Category,
        Status = c.Status,
        StatusDisplay = c.Status.ToString(),
        Priority = c.Priority,
        SupervisedEntityId = c.SupervisedEntityId,
        SupervisedEntityName = c.SupervisedEntity != null ? c.SupervisedEntity.Name : string.Empty,
        HandlerId = c.HandlerId,
        HandlerName = c.Handler != null ? c.Handler.FirstName + " " + c.Handler.LastName : null,
        CreatedByUserId = c.CreatedByUserId,
        CreatedByName = c.CreatedBy.FirstName + " " + c.CreatedBy.LastName,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        ResolvedAt = c.ResolvedAt,
        ClosedAt = c.ClosedAt,
        IsCancelled = c.IsCancelled,
        CancelledAt = c.CancelledAt,
        CancellationReason = c.CancellationReason
    };

    public CaseService(ApplicationDbContext context, ILogger<CaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of cases with optional filtering
    /// </summary>
    public async Task<(List<CaseResponse> Cases, int TotalCount)> GetCasesAsync(
        int page,
        int pageSize,
        CaseStatus? status = null,
        long? supervisedEntityId = null,
        long? handlerId = null,
        string? category = null)
    {
        var query = _context.Cases
            .Include(c => c.SupervisedEntity)
            .Include(c => c.Handler)
            .Include(c => c.CreatedBy)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (supervisedEntityId.HasValue)
            query = query.Where(c => c.SupervisedEntityId == supervisedEntityId.Value);

        if (handlerId.HasValue)
            query = query.Where(c => c.HandlerId == handlerId.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category.Contains(category));

        var totalCount = await query.CountAsync();

        var cases = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(CaseProjection)
            .ToListAsync();

        return (cases, totalCount);
    }

    /// <summary>
    /// Get case by ID
    /// </summary>
    public async Task<CaseResponse?> GetCaseByIdAsync(long id)
    {
        var response = await _context.Cases
            .Include(c => c.SupervisedEntity)
            .Include(c => c.Handler)
            .Include(c => c.CreatedBy)
            .Where(c => c.Id == id)
            .Select(CaseProjection)
            .FirstOrDefaultAsync();

        return response;
    }

    /// <summary>
    /// Create a new case
    /// </summary>
    public async Task<CaseResponse> CreateCaseAsync(CreateCaseRequest request, long userId)
    {
        // Generate case number
        var caseCount = await _context.Cases.CountAsync();
        var caseNumber = $"SP/{DateTime.UtcNow.Year}/{(caseCount + 1):D6}";

        var caseEntity = new Case
        {
            CaseNumber = caseNumber,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            SupervisedEntityId = request.SupervisedEntityId,
            Priority = request.Priority,
            Status = CaseStatus.New,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();

        return (await GetCaseByIdAsync(caseEntity.Id))!;
    }

    /// <summary>
    /// Update an existing case
    /// </summary>
    public async Task<CaseResponse?> UpdateCaseAsync(long id, UpdateCaseRequest request, long userId)
    {
        var caseEntity = await _context.Cases.FindAsync(id);
        if (caseEntity == null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Title))
            caseEntity.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Description))
            caseEntity.Description = request.Description;

        if (!string.IsNullOrWhiteSpace(request.Category))
            caseEntity.Category = request.Category;

        if (request.Priority.HasValue)
            caseEntity.Priority = request.Priority.Value;

        if (request.HandlerId.HasValue)
            caseEntity.HandlerId = request.HandlerId.Value;

        caseEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetCaseByIdAsync(id);
    }

    /// <summary>
    /// Delete a case (soft delete by setting status to Cancelled)
    /// </summary>
    public async Task<bool> DeleteCaseAsync(long id, long userId, string? reason = null)
    {
        var caseEntity = await _context.Cases.FindAsync(id);
        if (caseEntity == null)
            return false;

        caseEntity.IsCancelled = true;
        caseEntity.CancelledAt = DateTime.UtcNow;
        caseEntity.CancellationReason = reason;
        caseEntity.Status = CaseStatus.Cancelled;
        caseEntity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Update case status
    /// </summary>
    public async Task<CaseResponse?> UpdateCaseStatusAsync(long id, CaseStatus newStatus, long userId)
    {
        var caseEntity = await _context.Cases.FindAsync(id);
        if (caseEntity == null)
            return null;

        caseEntity.Status = newStatus;
        caseEntity.UpdatedAt = DateTime.UtcNow;

        if (newStatus == CaseStatus.Resolved)
            caseEntity.ResolvedAt = DateTime.UtcNow;
        else if (newStatus == CaseStatus.Closed)
            caseEntity.ClosedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetCaseByIdAsync(id);
    }

    // Instance mapping method removed; pure projection expression used instead to satisfy EF Core translation.
}
