using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.DTOs.Announcements;
using UknfCommunicationPlatform.Core.DTOs.Responses;
using UknfCommunicationPlatform.Core.Interfaces;

namespace UknfCommunicationPlatform.Api.Controllers;

/// <summary>
/// API endpoints for managing announcements
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
[Produces("application/json")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(
        IAnnouncementService announcementService,
        ICurrentUserService currentUserService,
        ILogger<AnnouncementsController> logger)
    {
        _announcementService = announcementService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of announcements
    /// </summary>
    /// <param name="unreadOnly">Filter to show only unread announcements</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <returns>Paginated list of announcements</returns>
    /// <response code="200">Returns paginated announcements</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AnnouncementListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<AnnouncementListItemResponse>>> GetAnnouncements(
        [FromQuery] bool? unreadOnly = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = GetCurrentUserId();

        // Validate pagination
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var result = await _announcementService.GetAnnouncementsAsync(userId, unreadOnly, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get announcement details by ID
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>Announcement details</returns>
    /// <response code="200">Returns announcement details</response>
    /// <response code="404">Announcement not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AnnouncementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AnnouncementResponse>> GetAnnouncementById(long id)
    {
        var userId = GetCurrentUserId();
        var announcement = await _announcementService.GetAnnouncementByIdAsync(id, userId);

        if (announcement == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Announcement not found",
                Details = $"Announcement with ID {id} does not exist"
            });
        }

        return Ok(announcement);
    }

    /// <summary>
    /// Create a new announcement (UKNF staff only)
    /// </summary>
    /// <param name="request">Create announcement request</param>
    /// <returns>Created announcement</returns>
    /// <response code="201">Announcement created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - UKNF staff only</response>
    [HttpPost]
    // TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
    // [Authorize(Roles = "UKNF")]
    [ProducesResponseType(typeof(AnnouncementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AnnouncementResponse>> CreateAnnouncement(
        [FromBody] CreateAnnouncementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Invalid request data",
                Details = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage))
            });
        }

        var userId = GetCurrentUserId();
        var announcement = await _announcementService.CreateAnnouncementAsync(request, userId);

        return CreatedAtAction(
            nameof(GetAnnouncementById),
            new { id = announcement.Id },
            announcement);
    }

    /// <summary>
    /// Update an existing announcement (UKNF staff only)
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <param name="request">Update announcement request</param>
    /// <returns>Updated announcement</returns>
    /// <response code="200">Announcement updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Announcement not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - UKNF staff only</response>
    [HttpPut("{id}")]
    // TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
    // [Authorize(Roles = "UKNF")]
    [ProducesResponseType(typeof(AnnouncementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AnnouncementResponse>> UpdateAnnouncement(
        long id,
        [FromBody] UpdateAnnouncementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Invalid request data",
                Details = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage))
            });
        }

        var announcement = await _announcementService.UpdateAnnouncementAsync(id, request);

        if (announcement == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Announcement not found",
                Details = $"Announcement with ID {id} does not exist"
            });
        }

        return Ok(announcement);
    }

    /// <summary>
    /// Delete an announcement (UKNF staff only)
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Announcement deleted successfully</response>
    /// <response code="404">Announcement not found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">Forbidden - UKNF staff only</response>
    [HttpDelete("{id}")]
    // TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
    // [Authorize(Roles = "UKNF")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAnnouncement(long id)
    {
        var deleted = await _announcementService.DeleteAnnouncementAsync(id);

        if (!deleted)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Announcement not found",
                Details = $"Announcement with ID {id} does not exist"
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Mark an announcement as read by the current user
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">Announcement marked as read</response>
    /// <response code="404">Announcement not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{id}/read")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAnnouncementAsRead(long id)
    {
        var userId = GetCurrentUserId();

        // Get IP address from request
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var marked = await _announcementService.MarkAsReadAsync(id, userId, ipAddress);

        if (!marked)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Announcement not found or already read",
                Details = $"Announcement with ID {id} does not exist or has already been marked as read"
            });
        }

        return Ok(new { message = "Announcement marked as read", announcementId = id });
    }

    /// <summary>
    /// Get the current user ID from the JWT token
    /// </summary>
    private long GetCurrentUserId()
    {
        // TODO: RE-ENABLE AUTHORIZATION - Temporarily using hardcoded user ID for testing
        // When authorization is disabled, return user ID 2 (jan.kowalski@uknf.gov.pl)
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("Authorization disabled - using default user ID 2 (jan.kowalski@uknf.gov.pl)");
            return 2; // Default to jan.kowalski user
        }

        if (!long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}
