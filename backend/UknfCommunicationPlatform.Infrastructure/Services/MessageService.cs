using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Core.DTOs.Messages;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for managing messages and communication
/// </summary>
public class MessageService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessageService> _logger;

    public MessageService(ApplicationDbContext context, ILogger<MessageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get messages for a user with pagination and filtering
    /// </summary>
    public async Task<(List<MessageResponse> messages, int totalCount)> GetMessagesAsync(
        long userId,
        int page,
        int pageSize,
        MessageFolder? folder = null,
        bool? isRead = null,
        string? searchTerm = null,
        long? relatedEntityId = null)
    {
        var query = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .Include(m => m.Attachments)
            .Include(m => m.Replies)
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .Where(m => !m.IsCancelled);

        // Filter by folder
        if (folder.HasValue)
        {
            query = query.Where(m => m.Folder == folder.Value);
        }

        // Filter by read status
        if (isRead.HasValue)
        {
            query = query.Where(m => m.IsRead == isRead.Value);
        }

        // Filter by related entity
        if (relatedEntityId.HasValue)
        {
            query = query.Where(m => m.RelatedEntityId == relatedEntityId.Value);
        }

        // Search in subject and body
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(m =>
                m.Subject.ToLower().Contains(term) ||
                m.Body.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var messageEntities = await query
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var messages = messageEntities.Select(m => MapToResponse(m)).ToList();

        return (messages, totalCount);
    }

    /// <summary>
    /// Get message by ID with full details
    /// </summary>
    public async Task<MessageDetailResponse?> GetMessageByIdAsync(long messageId, long userId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .Include(m => m.Attachments)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Sender)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Recipient)
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                (m.SenderId == userId || m.RecipientId == userId) &&
                !m.IsCancelled);

        if (message == null) return null;

        return new MessageDetailResponse
        {
            Id = message.Id,
            Subject = message.Subject,
            Body = message.Body,
            Sender = new MessageUserInfo
            {
                Id = message.Sender.Id,
                Email = message.Sender.Email,
                FirstName = message.Sender.FirstName,
                LastName = message.Sender.LastName
            },
            Recipient = message.Recipient != null ? new MessageUserInfo
            {
                Id = message.Recipient.Id,
                Email = message.Recipient.Email,
                FirstName = message.Recipient.FirstName,
                LastName = message.Recipient.LastName
            } : null,
            Status = message.Status,
            Folder = message.Folder,
            ThreadId = message.ThreadId,
            ParentMessageId = message.ParentMessageId,
            IsRead = message.IsRead,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            RelatedEntityId = message.RelatedEntityId,
            RelatedEntityName = message.RelatedEntity?.Name,
            RelatedReportId = message.RelatedReportId,
            RelatedCaseId = message.RelatedCaseId,
            AttachmentCount = message.Attachments.Count,
            ReplyCount = message.Replies.Count,
            IsCancelled = message.IsCancelled,
            CancelledAt = message.CancelledAt,
            Replies = message.Replies.Select(r => MapToResponse(r)).ToList(),
            Attachments = message.Attachments.Select(a => new MessageAttachmentInfo
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSize = a.FileSize,
                ContentType = a.ContentType,
                UploadedAt = a.UploadedAt
            }).ToList()
        };
    }

    /// <summary>
    /// Create a new message
    /// </summary>
    public async Task<MessageResponse> CreateMessageAsync(long senderId, CreateMessageRequest request)
    {
        var message = new Message
        {
            Subject = request.Subject,
            Body = request.Body,
            SenderId = senderId,
            RecipientId = request.RecipientId,
            Folder = request.Folder,
            ThreadId = request.ThreadId,
            ParentMessageId = request.ParentMessageId,
            RelatedEntityId = request.RelatedEntityId,
            RelatedReportId = request.RelatedReportId,
            RelatedCaseId = request.RelatedCaseId,
            Status = request.SendImmediately ? MessageStatus.Sent : MessageStatus.Draft,
            IsRead = false,
            SentAt = request.SendImmediately ? DateTime.UtcNow : default,
            IsCancelled = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
        if (message.RecipientId.HasValue)
        {
            await _context.Entry(message).Reference(m => m.Recipient).LoadAsync();
        }
        if (message.RelatedEntityId.HasValue)
        {
            await _context.Entry(message).Reference(m => m.RelatedEntity).LoadAsync();
        }

        _logger.LogInformation("Message {MessageId} created by user {UserId}", message.Id, senderId);

        return MapToResponse(message);
    }

    /// <summary>
    /// Update a message (typically drafts only)
    /// </summary>
    public async Task<MessageResponse?> UpdateMessageAsync(long messageId, long userId, UpdateMessageRequest request)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.SenderId == userId &&
                m.Status == MessageStatus.Draft);

        if (message == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Subject))
            message.Subject = request.Subject;

        if (!string.IsNullOrWhiteSpace(request.Body))
            message.Body = request.Body;

        if (request.RecipientId.HasValue)
            message.RecipientId = request.RecipientId.Value;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Message {MessageId} updated by user {UserId}", messageId, userId);

        return MapToResponse(message);
    }

    /// <summary>
    /// Mark message as read
    /// </summary>
    public async Task<bool> MarkAsReadAsync(long messageId, long userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.RecipientId == userId &&
                !m.IsRead);

        if (message == null) return false;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        message.Status = MessageStatus.Read;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Message {MessageId} marked as read by user {UserId}", messageId, userId);

        return true;
    }

    /// <summary>
    /// Mark multiple messages as read
    /// </summary>
    public async Task<int> MarkMultipleAsReadAsync(List<long> messageIds, long userId)
    {
        var messages = await _context.Messages
            .Where(m =>
                messageIds.Contains(m.Id) &&
                m.RecipientId == userId &&
                !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            message.Status = MessageStatus.Read;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("{Count} messages marked as read by user {UserId}", messages.Count, userId);

        return messages.Count;
    }

    /// <summary>
    /// Send a draft message
    /// </summary>
    public async Task<MessageResponse?> SendDraftAsync(long messageId, long userId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.SenderId == userId &&
                m.Status == MessageStatus.Draft);

        if (message == null) return null;

        message.Status = MessageStatus.Sent;
        message.SentAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Draft message {MessageId} sent by user {UserId}", messageId, userId);

        return MapToResponse(message);
    }

    /// <summary>
    /// Cancel a sent message (before it's read)
    /// </summary>
    public async Task<bool> CancelMessageAsync(long messageId, long userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.SenderId == userId &&
                !m.IsRead &&
                !m.IsCancelled);

        if (message == null) return false;

        message.IsCancelled = true;
        message.CancelledAt = DateTime.UtcNow;
        message.Status = MessageStatus.Cancelled;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Message {MessageId} cancelled by user {UserId}", messageId, userId);

        return true;
    }

    /// <summary>
    /// Delete a draft message
    /// </summary>
    public async Task<bool> DeleteDraftAsync(long messageId, long userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                m.SenderId == userId &&
                m.Status == MessageStatus.Draft);

        if (message == null) return false;

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Draft message {MessageId} deleted by user {UserId}", messageId, userId);

        return true;
    }

    /// <summary>
    /// Get unread message count for a user
    /// </summary>
    public async Task<int> GetUnreadCountAsync(long userId)
    {
        return await _context.Messages
            .CountAsync(m =>
                m.RecipientId == userId &&
                !m.IsRead &&
                !m.IsCancelled);
    }

    /// <summary>
    /// Get message statistics for a user
    /// </summary>
    public async Task<MessageStatsResponse> GetMessageStatsAsync(long userId)
    {
        var stats = new MessageStatsResponse
        {
            TotalInbox = await _context.Messages.CountAsync(m =>
                m.RecipientId == userId && m.Folder == MessageFolder.Inbox && !m.IsCancelled),
            TotalSent = await _context.Messages.CountAsync(m =>
                m.SenderId == userId && m.Folder == MessageFolder.Sent && !m.IsCancelled),
            TotalDrafts = await _context.Messages.CountAsync(m =>
                m.SenderId == userId && m.Status == MessageStatus.Draft),
            UnreadCount = await GetUnreadCountAsync(userId)
        };

        return stats;
    }

    /// <summary>
    /// Map Message entity to MessageResponse
    /// </summary>
    private MessageResponse MapToResponse(Message message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            Subject = message.Subject,
            Body = message.Body,
            Sender = new MessageUserInfo
            {
                Id = message.Sender.Id,
                Email = message.Sender.Email,
                FirstName = message.Sender.FirstName,
                LastName = message.Sender.LastName
            },
            Recipient = message.Recipient != null ? new MessageUserInfo
            {
                Id = message.Recipient.Id,
                Email = message.Recipient.Email,
                FirstName = message.Recipient.FirstName,
                LastName = message.Recipient.LastName
            } : null,
            Status = message.Status,
            Folder = message.Folder,
            ThreadId = message.ThreadId,
            ParentMessageId = message.ParentMessageId,
            IsRead = message.IsRead,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            RelatedEntityId = message.RelatedEntityId,
            RelatedEntityName = message.RelatedEntity?.Name,
            RelatedReportId = message.RelatedReportId,
            RelatedCaseId = message.RelatedCaseId,
            AttachmentCount = message.Attachments?.Count ?? 0,
            ReplyCount = message.Replies?.Count ?? 0,
            IsCancelled = message.IsCancelled,
            CancelledAt = message.CancelledAt,

            // Polish UI fields - computed from entity data
            Identyfikator = $"{message.SentAt.Year}/System{message.SenderId}/{message.Id}",
            SygnaturaSprawy = message.RelatedCase?.CaseNumber,
            Podmiot = message.RelatedEntity?.Name,
            StatusWiadomosci = message.Status switch
            {
                MessageStatus.Sent => "Wysłana",
                MessageStatus.Draft => "Wersja robocza",
                MessageStatus.Cancelled => "Anulowana",
                MessageStatus.Closed => "Zamknięta",
                MessageStatus.Read => "Przeczytana",
                _ => message.Status.ToString()
            },
            Priorytet = null, // Priority not tracked in current model
            DataPrzeslaniaPodmiotu = message.SenderId != message.RelatedEntity?.Id ? null : message.SentAt,
            Uzytkownik = $"{message.Sender.FirstName} {message.Sender.LastName}",
            WiadomoscUzytkownika = message.Body,
            DataPrzeslaniaUKNF = message.Sender.SupervisedEntityId == null ? message.SentAt : null,
            PracownikUKNF = message.Sender.SupervisedEntityId == null ? $"{message.Sender.FirstName} {message.Sender.LastName}" : null,
            WiadomoscPracownikaUKNF = message.Sender.SupervisedEntityId == null ? message.Body : null
        };
    }
}

/// <summary>
/// Message statistics response
/// </summary>
public class MessageStatsResponse
{
    public int TotalInbox { get; set; }
    public int TotalSent { get; set; }
    public int TotalDrafts { get; set; }
    public int UnreadCount { get; set; }
}
