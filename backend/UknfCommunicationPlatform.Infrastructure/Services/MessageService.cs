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
        bool? isRead = null,
        string? searchTerm = null,
        long? relatedEntityId = null)
    {
        var query = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .Include(m => m.Attachments)
            .Where(m => m.SenderId == userId || m.RecipientId == userId);

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
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                (m.SenderId == userId || m.RecipientId == userId));

        if (message == null) return null;

        // Determine if sender is external user (has supervised entity)
        var isExternalSender = message.Sender.SupervisedEntityId != null;

        // Get the entity context - either from sender or recipient
        var entityId = message.RelatedEntityId ?? message.Sender.SupervisedEntityId ?? message.Recipient?.SupervisedEntityId;
        var entityName = message.RelatedEntity?.Name;

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
            Priority = message.Priority,
            IsRead = message.IsRead,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            RelatedEntityId = entityId,
            RelatedEntityName = entityName,
            AttachmentCount = message.Attachments.Count,
            ParentMessageId = message.ParentMessageId,

            // Polish UI fields - computed from entity data
            Identyfikator = $"{message.SentAt.Year}/System{message.SenderId}/{message.Id}",
            SygnaturaSprawy = $"{message.SentAt.Year:D4}/{message.Id:D6}",
            Podmiot = entityName ?? "Brak powiązania",
            StatusWiadomosci = message.Status switch
            {
                MessageStatus.Sent => "Wysłana",
                MessageStatus.Closed => "Zamknięta",
                MessageStatus.Read => "Przeczytana",
                MessageStatus.AwaitingUknfResponse => "Oczekuje na odpowiedź UKNF",
                MessageStatus.AwaitingUserResponse => "Oczekuje na odpowiedź użytkownika",
                _ => message.Status.ToString()
            },
            WiadomoscUzytkownika = isExternalSender ? message.Body : null,
            DataPrzeslaniaUKNF = !isExternalSender ? message.SentAt : null,
            PracownikUKNF = !isExternalSender ? $"{message.Sender.FirstName} {message.Sender.LastName}" : null,
            WiadomoscPracownikaUKNF = !isExternalSender ? message.Body : null,

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
    /// Create a new message with optional attachments
    /// </summary>
    public async Task<MessageResponse> CreateMessageAsync(long senderId, CreateMessageRequest request)
    {
        // Create the message entity
        var message = new Message
        {
            Subject = request.Subject,
            Body = request.Body,
            SenderId = senderId,
            RecipientId = request.RecipientId,
            RelatedEntityId = request.RelatedEntityId,
            Status = MessageStatus.Sent,
            Priority = request.Priority,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(); // Save to get the message ID

        // Process attachments if any - attachments are created atomically with the message
        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var file in request.Attachments)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var attachment = new MessageAttachment
                {
                    MessageId = message.Id, // Foreign key - attachment belongs to this message
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    UploadedByUserId = senderId
                };

                _context.MessageAttachments.Add(attachment);
            }

            await _context.SaveChangesAsync(); // Save attachments in the same transaction context
            _logger.LogInformation("Created {Count} attachments for message {MessageId}", request.Attachments.Count, message.Id);
        }

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
        await _context.Entry(message).Collection(m => m.Attachments).LoadAsync();

        _logger.LogInformation("Message {MessageId} created by user {UserId} with {AttachmentCount} attachments",
            message.Id, senderId, message.Attachments.Count);

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
    /// Get unread message count for a user
    /// </summary>
    public async Task<int> GetUnreadCountAsync(long userId)
    {
        return await _context.Messages
            .CountAsync(m =>
                m.RecipientId == userId &&
                !m.IsRead);
    }

    /// <summary>
    /// Get message statistics for a user
    /// </summary>
    public async Task<MessageStatsResponse> GetMessageStatsAsync(long userId)
    {
        var stats = new MessageStatsResponse
        {
            TotalInbox = await _context.Messages.CountAsync(m =>
                m.RecipientId == userId),
            TotalSent = await _context.Messages.CountAsync(m =>
                m.SenderId == userId),
            UnreadCount = await GetUnreadCountAsync(userId)
        };

        return stats;
    }

    /// <summary>
    /// Map Message entity to MessageResponse
    /// </summary>
    private MessageResponse MapToResponse(Message message)
    {
        // Determine if sender is external user (has supervised entity)
        var isExternalSender = message.Sender.SupervisedEntityId != null;

        // Get the entity context - either from sender or recipient
        var entityId = message.RelatedEntityId ?? message.Sender.SupervisedEntityId ?? message.Recipient?.SupervisedEntityId;
        var entityName = message.RelatedEntity?.Name;

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
            Priority = message.Priority,
            IsRead = message.IsRead,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            RelatedEntityId = entityId,
            RelatedEntityName = entityName,
            AttachmentCount = message.Attachments?.Count ?? 0,
            ParentMessageId = message.ParentMessageId,

            // Polish UI fields - computed from entity data
            Identyfikator = $"{message.SentAt.Year}/System{message.SenderId}/{message.Id}",
            SygnaturaSprawy = $"{message.SentAt.Year:D4}/{message.Id:D6}",
            Podmiot = entityName ?? "Brak powiązania",
            StatusWiadomosci = message.Status switch
            {
                MessageStatus.Sent => "Wysłana",
                MessageStatus.Closed => "Zamknięta",
                MessageStatus.Read => "Przeczytana",
                MessageStatus.AwaitingUknfResponse => "Oczekuje na odpowiedź UKNF",
                MessageStatus.AwaitingUserResponse => "Oczekuje na odpowiedź użytkownika",
                _ => message.Status.ToString()
            },
            WiadomoscUzytkownika = isExternalSender ? message.Body : null,
            DataPrzeslaniaUKNF = !isExternalSender ? message.SentAt : null,
            PracownikUKNF = !isExternalSender ? $"{message.Sender.FirstName} {message.Sender.LastName}" : null,
            WiadomoscPracownikaUKNF = !isExternalSender ? message.Body : null
        };
    }

    /// <summary>
    /// Get attachment by ID for download
    /// Verifies that the user has access to the message that owns the attachment
    /// </summary>
    public async Task<MessageAttachment?> GetAttachmentAsync(long messageId, long attachmentId, long userId)
    {
        // First verify that the user has access to the message
        var message = await _context.Messages
            .FirstOrDefaultAsync(m =>
                m.Id == messageId &&
                (m.SenderId == userId || m.RecipientId == userId));

        if (message == null)
        {
            _logger.LogWarning("User {UserId} attempted to access attachment {AttachmentId} for message {MessageId} - access denied",
                userId, attachmentId, messageId);
            return null;
        }

        // Get the attachment - ensure it belongs to the specified message
        var attachment = await _context.MessageAttachments
            .FirstOrDefaultAsync(a =>
                a.Id == attachmentId &&
                a.MessageId == messageId);

        if (attachment != null)
        {
            _logger.LogInformation("User {UserId} downloading attachment {AttachmentId} ({FileName}) from message {MessageId}",
                userId, attachmentId, attachment.FileName, messageId);
        }

        return attachment;
    }

    /// <summary>
    /// Reply to an existing message
    /// </summary>
    public async Task<MessageResponse> ReplyToMessageAsync(long parentMessageId, long userId, ReplyMessageRequest request)
    {
        // Get the parent message to verify access and get context
        var parentMessage = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .FirstOrDefaultAsync(m =>
                m.Id == parentMessageId &&
                (m.SenderId == userId || m.RecipientId == userId));

        if (parentMessage == null)
        {
            throw new UnauthorizedAccessException("Parent message not found or access denied");
        }

        // Determine recipient: if current user is sender, reply to recipient; otherwise reply to sender
        var recipientId = parentMessage.SenderId == userId
            ? parentMessage.RecipientId
            : parentMessage.SenderId;

        if (!recipientId.HasValue)
        {
            throw new InvalidOperationException("Cannot reply to a message without a recipient");
        }

        // Create reply message with "Re: " prefix if not already present
        var replySubject = parentMessage.Subject.StartsWith("Re: ")
            ? parentMessage.Subject
            : $"Re: {parentMessage.Subject}";

        var replyMessage = new Message
        {
            Subject = replySubject,
            Body = request.Body,
            SenderId = userId,
            RecipientId = recipientId,
            ParentMessageId = parentMessageId,
            RelatedEntityId = parentMessage.RelatedEntityId,
            Status = MessageStatus.Sent,
            Priority = request.Priority ?? parentMessage.Priority, // Inherit priority if not specified
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(replyMessage);
        await _context.SaveChangesAsync();

        // Process attachments if any
        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var file in request.Attachments)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var attachment = new MessageAttachment
                {
                    MessageId = replyMessage.Id,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.UtcNow,
                    UploadedByUserId = userId
                };

                _context.MessageAttachments.Add(attachment);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Created {Count} attachments for reply message {MessageId}", request.Attachments.Count, replyMessage.Id);
        }

        // Reload with navigation properties
        await _context.Entry(replyMessage).Reference(m => m.Sender).LoadAsync();
        await _context.Entry(replyMessage).Reference(m => m.Recipient).LoadAsync();
        if (replyMessage.RelatedEntityId.HasValue)
        {
            await _context.Entry(replyMessage).Reference(m => m.RelatedEntity).LoadAsync();
        }
        await _context.Entry(replyMessage).Collection(m => m.Attachments).LoadAsync();

        _logger.LogInformation("Reply message {MessageId} created by user {UserId} to parent {ParentMessageId}",
            replyMessage.Id, userId, parentMessageId);

        return MapToResponse(replyMessage);
    }

    /// <summary>
    /// Export messages to CSV format
    /// </summary>
    public async Task<List<MessageExportDto>> ExportMessagesAsync(
        long userId,
        bool? isRead = null,
        string? searchTerm = null,
        long? relatedEntityId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.RelatedEntity)
            .Include(m => m.Attachments)
            .Where(m => m.SenderId == userId || m.RecipientId == userId);

        // Apply filters
        if (isRead.HasValue)
        {
            query = query.Where(m => m.IsRead == isRead.Value);
        }

        if (relatedEntityId.HasValue)
        {
            query = query.Where(m => m.RelatedEntityId == relatedEntityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(m =>
                m.Subject.ToLower().Contains(term) ||
                m.Body.ToLower().Contains(term));
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(m => m.SentAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(m => m.SentAt <= dateTo.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m => new MessageExportDto
        {
            Id = m.Id,
            Subject = m.Subject,
            SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
            SenderEmail = m.Sender.Email,
            RecipientName = m.Recipient != null ? $"{m.Recipient.FirstName} {m.Recipient.LastName}" : null,
            RecipientEmail = m.Recipient?.Email,
            Status = m.Status.ToString(),
            Priority = m.Priority switch
            {
                MessagePriority.Low => "Niski",
                MessagePriority.Normal => "Normalny",
                MessagePriority.High => "Wysoki",
                _ => m.Priority.ToString()
            },
            IsRead = m.IsRead,
            SentAt = m.SentAt,
            ReadAt = m.ReadAt,
            RelatedEntityName = m.RelatedEntity?.Name,
            AttachmentCount = m.Attachments.Count,
            IsReply = m.ParentMessageId.HasValue
        }).ToList();
    }
}

/// <summary>
/// Message statistics response
/// </summary>
public class MessageStatsResponse
{
    public int TotalInbox { get; set; }
    public int TotalSent { get; set; }
    public int UnreadCount { get; set; }
}
