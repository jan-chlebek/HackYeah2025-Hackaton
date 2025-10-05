using UknfCommunicationPlatform.Core.DTOs.Announcements;
using UknfCommunicationPlatform.Core.DTOs.Responses;

namespace UknfCommunicationPlatform.Core.Interfaces;

/// <summary>
/// Service for managing announcements
/// </summary>
public interface IAnnouncementService
{
    /// <summary>
    /// Get paginated list of announcements with optional filtering
    /// </summary>
    /// <param name="userId">Current user ID for read status</param>
    /// <param name="unreadOnly">Filter to show only unread announcements</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Paginated list of announcements</returns>
    Task<PagedResponse<AnnouncementListItemResponse>> GetAnnouncementsAsync(
        long userId,
        bool? unreadOnly = null,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get announcement details by ID
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <param name="userId">Current user ID for read status</param>
    /// <returns>Announcement details</returns>
    Task<AnnouncementResponse?> GetAnnouncementByIdAsync(long id, long userId);

    /// <summary>
    /// Create a new announcement (UKNF staff only)
    /// </summary>
    /// <param name="request">Create announcement request</param>
    /// <param name="createdByUserId">Creator user ID</param>
    /// <returns>Created announcement</returns>
    Task<AnnouncementResponse> CreateAnnouncementAsync(CreateAnnouncementRequest request, long createdByUserId);

    /// <summary>
    /// Update an existing announcement (UKNF staff only)
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <param name="request">Update announcement request</param>
    /// <returns>Updated announcement or null if not found</returns>
    Task<AnnouncementResponse?> UpdateAnnouncementAsync(long id, UpdateAnnouncementRequest request);

    /// <summary>
    /// Delete an announcement (UKNF staff only)
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAnnouncementAsync(long id);

    /// <summary>
    /// Mark an announcement as read by the current user
    /// </summary>
    /// <param name="announcementId">Announcement ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>True if marked as read, false if already read or announcement not found</returns>
    Task<bool> MarkAsReadAsync(long announcementId, long userId, string? ipAddress = null);
}
