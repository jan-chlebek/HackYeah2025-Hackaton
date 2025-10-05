using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Core.DTOs.Announcements;
using UknfCommunicationPlatform.Core.DTOs.Responses;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Interfaces;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing announcements
/// </summary>
public class AnnouncementService : IAnnouncementService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnnouncementService> _logger;
    private const int MaxContentPreviewLength = 200;

    public AnnouncementService(
        ApplicationDbContext context,
        ILogger<AnnouncementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResponse<AnnouncementListItemResponse>> GetAnnouncementsAsync(
        long userId,
        bool? unreadOnly = null,
        int page = 1,
        int pageSize = 20)
    {
        _logger.LogInformation(
            "Getting announcements for user {UserId}, unreadOnly: {UnreadOnly}, page: {Page}, pageSize: {PageSize}",
            userId, unreadOnly, page, pageSize);

        var query = _context.Announcements
            .Include(a => a.CreatedBy)
            .AsQueryable();

        // Apply read/unread filter
        if (unreadOnly.HasValue && unreadOnly.Value)
        {
            var readAnnouncementIds = await _context.AnnouncementReads
                .Where(ar => ar.UserId == userId)
                .Select(ar => ar.AnnouncementId)
                .ToListAsync();

            query = query.Where(a => !readAnnouncementIds.Contains(a.Id));
        }

        // Get total count
        var totalItems = await query.CountAsync();

        // Get paged data
        var announcements = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                Announcement = a,
                IsRead = _context.AnnouncementReads
                    .Any(ar => ar.AnnouncementId == a.Id && ar.UserId == userId)
            })
            .ToListAsync();

        var items = announcements.Select(x => new AnnouncementListItemResponse
        {
            Id = x.Announcement.Id,
            Title = x.Announcement.Title,
            ContentPreview = TruncateContent(x.Announcement.Content, MaxContentPreviewLength),
            CreatedByName = $"{x.Announcement.CreatedBy.FirstName} {x.Announcement.CreatedBy.LastName}",
            CreatedAt = x.Announcement.CreatedAt,
            UpdatedAt = x.Announcement.UpdatedAt,
            IsReadByCurrentUser = x.IsRead
        }).ToList();

        return new PagedResponse<AnnouncementListItemResponse>
        {
            Items = items,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse?> GetAnnouncementByIdAsync(long id, long userId)
    {
        _logger.LogInformation("Getting announcement {AnnouncementId} for user {UserId}", id, userId);

        var announcement = await _context.Announcements
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (announcement == null)
        {
            _logger.LogWarning("Announcement {AnnouncementId} not found", id);
            return null;
        }

        var readStatus = await _context.AnnouncementReads
            .FirstOrDefaultAsync(ar => ar.AnnouncementId == id && ar.UserId == userId);

        return new AnnouncementResponse
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            CreatedByUserId = announcement.CreatedByUserId,
            CreatedByName = $"{announcement.CreatedBy.FirstName} {announcement.CreatedBy.LastName}",
            CreatedAt = announcement.CreatedAt,
            UpdatedAt = announcement.UpdatedAt,
            IsReadByCurrentUser = readStatus != null,
            ReadAt = readStatus?.ReadAt
        };
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse> CreateAnnouncementAsync(
        CreateAnnouncementRequest request,
        long createdByUserId)
    {
        _logger.LogInformation("Creating announcement by user {UserId}", createdByUserId);

        var announcement = new Announcement
        {
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            // Set defaults for required fields that aren't in simplified version
            Category = "General",
            Priority = Core.Enums.AnnouncementPriority.Low,
            IsPublished = false
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created announcement {AnnouncementId}", announcement.Id);

        // Load creator for response
        await _context.Entry(announcement)
            .Reference(a => a.CreatedBy)
            .LoadAsync();

        return new AnnouncementResponse
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            CreatedByUserId = announcement.CreatedByUserId,
            CreatedByName = $"{announcement.CreatedBy.FirstName} {announcement.CreatedBy.LastName}",
            CreatedAt = announcement.CreatedAt,
            UpdatedAt = announcement.UpdatedAt,
            IsReadByCurrentUser = false,
            ReadAt = null
        };
    }

    /// <inheritdoc />
    public async Task<AnnouncementResponse?> UpdateAnnouncementAsync(long id, UpdateAnnouncementRequest request)
    {
        _logger.LogInformation("Updating announcement {AnnouncementId}", id);

        var announcement = await _context.Announcements
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (announcement == null)
        {
            _logger.LogWarning("Announcement {AnnouncementId} not found for update", id);
            return null;
        }

        announcement.Title = request.Title.Trim();
        announcement.Content = request.Content.Trim();
        announcement.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated announcement {AnnouncementId}", id);

        return new AnnouncementResponse
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            CreatedByUserId = announcement.CreatedByUserId,
            CreatedByName = $"{announcement.CreatedBy.FirstName} {announcement.CreatedBy.LastName}",
            CreatedAt = announcement.CreatedAt,
            UpdatedAt = announcement.UpdatedAt,
            IsReadByCurrentUser = false, // Not tracking for update response
            ReadAt = null
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAnnouncementAsync(long id)
    {
        _logger.LogInformation("Deleting announcement {AnnouncementId}", id);

        var announcement = await _context.Announcements.FindAsync(id);

        if (announcement == null)
        {
            _logger.LogWarning("Announcement {AnnouncementId} not found for deletion", id);
            return false;
        }

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted announcement {AnnouncementId}", id);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> MarkAsReadAsync(long announcementId, long userId, string? ipAddress = null)
    {
        _logger.LogInformation("Marking announcement {AnnouncementId} as read for user {UserId}", announcementId, userId);

        // Check if announcement exists
        var announcementExists = await _context.Announcements.AnyAsync(a => a.Id == announcementId);
        if (!announcementExists)
        {
            _logger.LogWarning("Announcement {AnnouncementId} not found", announcementId);
            return false;
        }

        // Check if already read
        var existingRead = await _context.AnnouncementReads
            .FirstOrDefaultAsync(ar => ar.AnnouncementId == announcementId && ar.UserId == userId);

        if (existingRead != null)
        {
            _logger.LogInformation(
                "Announcement {AnnouncementId} already marked as read by user {UserId}",
                announcementId, userId);
            return false;
        }

        // Create read record
        var readRecord = new AnnouncementRead
        {
            AnnouncementId = announcementId,
            UserId = userId,
            ReadAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        _context.AnnouncementReads.Add(readRecord);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Marked announcement {AnnouncementId} as read for user {UserId}",
            announcementId, userId);

        return true;
    }

    /// <summary>
    /// Truncate content to specified length with ellipsis
    /// </summary>
    private static string TruncateContent(string content, int maxLength)
    {
        if (string.IsNullOrEmpty(content) || content.Length <= maxLength)
        {
            return content;
        }

        return content.Substring(0, maxLength).TrimEnd() + "...";
    }
}
