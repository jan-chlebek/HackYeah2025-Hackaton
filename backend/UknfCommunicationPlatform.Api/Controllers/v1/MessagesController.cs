using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.DTOs.Messages;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Message and communication management
/// </summary>
[ApiController]
[Route("api/v1/messages")]
[Produces("application/json")]
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
public class MessagesController : ControllerBase
{
    private readonly MessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(MessageService messageService, ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    /// <summary>
    /// Get messages for the current user with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="folder">Filter by message folder</param>
    /// <param name="isRead">Filter by read status</param>
    /// <param name="searchTerm">Search in subject and body</param>
    /// <param name="relatedEntityId">Filter by related supervised entity</param>
    /// <returns>List of messages with pagination metadata</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<object>> GetMessages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] MessageFolder? folder = null,
        [FromQuery] bool? isRead = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] long? relatedEntityId = null)
    {
        var userId = GetCurrentUserId();

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (messages, totalCount) = await _messageService.GetMessagesAsync(
            userId, page, pageSize, folder, isRead, searchTerm, relatedEntityId);

        return Ok(new
        {
            data = messages,
            pagination = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    /// <summary>
    /// Get message by ID with full details including replies and attachments
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Detailed message information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDetailResponse>> GetMessage(long id)
    {
        var userId = GetCurrentUserId();
        var message = await _messageService.GetMessageByIdAsync(id, userId);

        if (message == null)
        {
            return NotFound(new { error = "Message not found or access denied" });
        }

        return Ok(message);
    }

    /// <summary>
    /// Create and send a new message with optional file attachments
    /// </summary>
    /// <param name="request">Message creation data with optional file attachments</param>
    /// <returns>Created message</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequestSizeLimit(100_000_000)] // 100MB max total request size
    public async Task<ActionResult<MessageResponse>> CreateMessage([FromForm] CreateMessageRequest request)
    {
        var userId = GetCurrentUserId();

        if (!request.RecipientId.HasValue)
        {
            return BadRequest(new { error = "Recipient is required" });
        }

        // Validate attachments if provided
        if (request.Attachments != null && request.Attachments.Any())
        {
            const long maxFileSize = 50_000_000; // 50MB per file
            foreach (var file in request.Attachments)
            {
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new { error = $"File '{file.FileName}' exceeds maximum size of 50MB" });
                }
                if (file.Length == 0)
                {
                    return BadRequest(new { error = $"File '{file.FileName}' is empty" });
                }
            }
        }

        var message = await _messageService.CreateMessageAsync(userId, request);

        return CreatedAtAction(
            nameof(GetMessage),
            new { id = message.Id },
            message);
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkAsRead(long id)
    {
        var userId = GetCurrentUserId();
        var success = await _messageService.MarkAsReadAsync(id, userId);

        if (!success)
        {
            return NotFound(new { error = "Message not found or already read" });
        }

        return Ok(new { message = "Message marked as read" });
    }

    /// <summary>
    /// Mark multiple messages as read
    /// </summary>
    /// <param name="request">List of message IDs</param>
    /// <returns>Number of messages marked as read</returns>
    [HttpPost("read-multiple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> MarkMultipleAsRead([FromBody] MarkMultipleAsReadRequest request)
    {
        var userId = GetCurrentUserId();
        var count = await _messageService.MarkMultipleAsReadAsync(request.MessageIds, userId);

        return Ok(new { markedCount = count, message = $"{count} message(s) marked as read" });
    }

    /// <summary>
    /// Get unread message count for the current user
    /// </summary>
    /// <returns>Unread count</returns>
    [HttpGet("unread-count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<object>> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        var count = await _messageService.GetUnreadCountAsync(userId);

        return Ok(new { unreadCount = count });
    }

    /// <summary>
    /// Get message statistics for the current user
    /// </summary>
    /// <returns>Message statistics</returns>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageStatsResponse>> GetMessageStats()
    {
        var userId = GetCurrentUserId();
        var stats = await _messageService.GetMessageStatsAsync(userId);

        return Ok(stats);
    }

    /// <summary>
    /// Download a message attachment
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <param name="attachmentId">Attachment ID</param>
    /// <returns>File content</returns>
    [HttpGet("{messageId}/attachments/{attachmentId}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(long messageId, long attachmentId)
    {
        var userId = GetCurrentUserId();
        var attachment = await _messageService.GetAttachmentAsync(messageId, attachmentId, userId);

        if (attachment == null)
        {
            return NotFound(new { error = "Attachment not found or access denied" });
        }

        return File(attachment.FileContent, attachment.ContentType, attachment.FileName);
    }

    /// <summary>
    /// Get the current user ID from the JWT token
    /// </summary>
    private long GetCurrentUserId()
    {
        // TODO: RE-ENABLE AUTHORIZATION - Temporarily using hardcoded user ID for testing
        // When authorization is disabled, return user ID 2 (jan.kowalski@uknf.gov.pl) who has seeded messages
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("Authorization disabled - using default user ID 2 (jan.kowalski@uknf.gov.pl)");
            return 2; // Default to jan.kowalski user who has seeded messages
        }

        if (!long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}

/// <summary>
/// Request for marking multiple messages as read
/// </summary>
public class MarkMultipleAsReadRequest
{
    /// <summary>
    /// List of message IDs to mark as read
    /// </summary>
    public List<long> MessageIds { get; set; } = new();
}
