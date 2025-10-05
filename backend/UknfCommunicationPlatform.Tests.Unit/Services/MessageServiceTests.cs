using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Core.DTOs.Messages;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for MessageService
/// </summary>
public class MessageServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<MessageService>> _mockLogger;
    private readonly MessageService _sut;
    private readonly User _testSender;
    private readonly User _testRecipient;

    public MessageServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<MessageService>>();
        _sut = new MessageService(_context, _mockLogger.Object);

        // Create test users
        _testSender = new User
        {
            Id = 1,
            Email = "sender@uknf.gov.pl",
            FirstName = "John",
            LastName = "Sender",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _testRecipient = new User
        {
            Id = 2,
            Email = "recipient@bank.com",
            FirstName = "Jane",
            LastName = "Recipient",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.AddRange(_testSender, _testRecipient);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateMessageAsync_WithValidData_ShouldCreateMessage()
    {
        // Arrange
        var request = new CreateMessageRequest
        {
            Subject = "Test Subject",
            Body = "Test Body",
            RecipientId = _testRecipient.Id,
            Folder = MessageFolder.Sent,
            SendImmediately = true
        };

        // Act
        var result = await _sut.CreateMessageAsync(_testSender.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be("Test Subject");
        result.Body.Should().Be("Test Body");
        result.Sender.Id.Should().Be(_testSender.Id);
        result.Recipient.Should().NotBeNull();
        result.Recipient!.Id.Should().Be(_testRecipient.Id);
        result.Status.Should().Be(MessageStatus.Sent);
        result.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task CreateMessageAsync_AsDraft_ShouldCreateDraft()
    {
        // Arrange
        var request = new CreateMessageRequest
        {
            Subject = "Draft Message",
            Body = "Draft Body",
            RecipientId = _testRecipient.Id,
            SendImmediately = false
        };

        // Act
        var result = await _sut.CreateMessageAsync(_testSender.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(MessageStatus.Draft);
        result.SentAt.Should().Be(default(DateTime));
    }

    [Fact]
    public async Task GetMessagesAsync_ShouldReturnUserMessages()
    {
        // Arrange
        await SeedMessagesAsync();

        // Act
        var (messages, totalCount) = await _sut.GetMessagesAsync(
            userId: _testSender.Id,
            page: 1,
            pageSize: 10);

        // Assert
        messages.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        messages.Should().OnlyContain(m =>
            m.Sender.Id == _testSender.Id || (m.Recipient != null && m.Recipient.Id == _testSender.Id));
    }

    [Fact]
    public async Task GetMessagesAsync_WithFolderFilter_ShouldReturnFilteredMessages()
    {
        // Arrange
        await SeedMessagesAsync();

        // Act
        var (messages, totalCount) = await _sut.GetMessagesAsync(
            userId: _testSender.Id,
            page: 1,
            pageSize: 10,
            folder: MessageFolder.Sent);

        // Assert
        messages.Should().OnlyContain(m => m.Folder == MessageFolder.Sent);
    }

    [Fact]
    public async Task GetMessagesAsync_WithSearchTerm_ShouldReturnMatchingMessages()
    {
        // Arrange
        await SeedMessagesAsync();

        // Act
        var (messages, totalCount) = await _sut.GetMessagesAsync(
            userId: _testSender.Id,
            page: 1,
            pageSize: 10,
            searchTerm: "Important");

        // Assert
        messages.Should().NotBeEmpty();
        messages.Should().OnlyContain(m =>
            m.Subject.Contains("Important", StringComparison.OrdinalIgnoreCase) ||
            m.Body.Contains("Important", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetMessagesAsync_WithReadStatusFilter_ShouldReturnFilteredMessages()
    {
        // Arrange
        await SeedMessagesAsync();
        var unreadMessage = new Message
        {
            Subject = "Unread Message",
            Body = "Unread Body",
            SenderId = _testRecipient.Id,
            RecipientId = _testSender.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Inbox,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(unreadMessage);
        await _context.SaveChangesAsync();

        // Act
        var (messages, totalCount) = await _sut.GetMessagesAsync(
            userId: _testSender.Id,
            page: 1,
            pageSize: 10,
            isRead: false);

        // Assert
        messages.Should().OnlyContain(m => !m.IsRead);
        totalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetMessageByIdAsync_WithValidId_ShouldReturnMessageDetails()
    {
        // Arrange
        var message = new Message
        {
            Subject = "Detail Test",
            Body = "Detail Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Sent,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetMessageByIdAsync(message.Id, _testSender.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Subject.Should().Be("Detail Test");
        result.Replies.Should().NotBeNull();
        result.Attachments.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMessageByIdAsync_WithUnauthorizedUser_ShouldReturnNull()
    {
        // Arrange
        var message = new Message
        {
            Subject = "Private Message",
            Body = "Private Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Sent,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetMessageByIdAsync(message.Id, userId: 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task MarkAsReadAsync_WithUnreadMessage_ShouldMarkAsRead()
    {
        // Arrange
        var message = new Message
        {
            Subject = "Unread",
            Body = "Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Inbox,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.MarkAsReadAsync(message.Id, _testRecipient.Id);

        // Assert
        result.Should().BeTrue();
        var updatedMessage = await _context.Messages.FindAsync(message.Id);
        updatedMessage!.IsRead.Should().BeTrue();
        updatedMessage.ReadAt.Should().NotBeNull();
        updatedMessage.Status.Should().Be(MessageStatus.Read);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithAlreadyReadMessage_ShouldReturnFalse()
    {
        // Arrange
        var message = new Message
        {
            Subject = "Already Read",
            Body = "Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Read,
            Folder = MessageFolder.Inbox,
            IsRead = true,
            ReadAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.MarkAsReadAsync(message.Id, _testRecipient.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkMultipleAsReadAsync_ShouldMarkAllUnreadMessages()
    {
        // Arrange
        var messages = new[]
        {
            new Message
            {
                Subject = "Unread 1",
                Body = "Body 1",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                IsRead = false,
                SentAt = DateTime.UtcNow
            },
            new Message
            {
                Subject = "Unread 2",
                Body = "Body 2",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                IsRead = false,
                SentAt = DateTime.UtcNow
            }
        };
        _context.Messages.AddRange(messages);
        await _context.SaveChangesAsync();

        var messageIds = messages.Select(m => m.Id).ToList();

        // Act
        var count = await _sut.MarkMultipleAsReadAsync(messageIds, _testRecipient.Id);

        // Assert
        count.Should().Be(2);
        var updatedMessages = await _context.Messages
            .Where(m => messageIds.Contains(m.Id))
            .ToListAsync();
        updatedMessages.Should().OnlyContain(m => m.IsRead);
    }

    [Fact]
    public async Task UpdateMessageAsync_WithDraft_ShouldUpdateMessage()
    {
        // Arrange
        var draft = new Message
        {
            Subject = "Original Subject",
            Body = "Original Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Draft,
            Folder = MessageFolder.Drafts
        };
        _context.Messages.Add(draft);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateMessageRequest
        {
            Subject = "Updated Subject",
            Body = "Updated Body"
        };

        // Act
        var result = await _sut.UpdateMessageAsync(draft.Id, _testSender.Id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Subject.Should().Be("Updated Subject");
        result.Body.Should().Be("Updated Body");
    }

    [Fact]
    public async Task UpdateMessageAsync_WithSentMessage_ShouldReturnNull()
    {
        // Arrange
        var sentMessage = new Message
        {
            Subject = "Sent Message",
            Body = "Sent Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Sent,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(sentMessage);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateMessageRequest
        {
            Subject = "Try to Update"
        };

        // Act
        var result = await _sut.UpdateMessageAsync(sentMessage.Id, _testSender.Id, updateRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SendDraftAsync_WithDraft_ShouldSendMessage()
    {
        // Arrange
        var draft = new Message
        {
            Subject = "Draft to Send",
            Body = "Draft Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Draft,
            Folder = MessageFolder.Drafts
        };
        _context.Messages.Add(draft);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.SendDraftAsync(draft.Id, _testSender.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(MessageStatus.Sent);
        result.SentAt.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task CancelMessageAsync_WithUnreadMessage_ShouldCancelMessage()
    {
        // Arrange
        var message = new Message
        {
            Subject = "To Cancel",
            Body = "Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Sent,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CancelMessageAsync(message.Id, _testSender.Id);

        // Assert
        result.Should().BeTrue();
        var cancelledMessage = await _context.Messages.FindAsync(message.Id);
        cancelledMessage!.IsCancelled.Should().BeTrue();
        cancelledMessage.CancelledAt.Should().NotBeNull();
        cancelledMessage.Status.Should().Be(MessageStatus.Cancelled);
    }

    [Fact]
    public async Task CancelMessageAsync_WithReadMessage_ShouldReturnFalse()
    {
        // Arrange
        var message = new Message
        {
            Subject = "Already Read",
            Body = "Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Read,
            Folder = MessageFolder.Sent,
            IsRead = true,
            ReadAt = DateTime.UtcNow,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CancelMessageAsync(message.Id, _testSender.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDraftAsync_WithDraft_ShouldDeleteMessage()
    {
        // Arrange
        var draft = new Message
        {
            Subject = "Draft to Delete",
            Body = "Draft Body",
            SenderId = _testSender.Id,
            Status = MessageStatus.Draft,
            Folder = MessageFolder.Drafts
        };
        _context.Messages.Add(draft);
        await _context.SaveChangesAsync();
        var draftId = draft.Id;

        // Act
        var result = await _sut.DeleteDraftAsync(draftId, _testSender.Id);

        // Assert
        result.Should().BeTrue();
        var deletedDraft = await _context.Messages.FindAsync(draftId);
        deletedDraft.Should().BeNull();
    }

    [Fact]
    public async Task DeleteDraftAsync_WithSentMessage_ShouldReturnFalse()
    {
        // Arrange
        var sentMessage = new Message
        {
            Subject = "Sent Message",
            Body = "Body",
            SenderId = _testSender.Id,
            RecipientId = _testRecipient.Id,
            Status = MessageStatus.Sent,
            Folder = MessageFolder.Sent,
            SentAt = DateTime.UtcNow
        };
        _context.Messages.Add(sentMessage);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.DeleteDraftAsync(sentMessage.Id, _testSender.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var unreadMessages = new[]
        {
            new Message
            {
                Subject = "Unread 1",
                Body = "Body",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                IsRead = false,
                SentAt = DateTime.UtcNow
            },
            new Message
            {
                Subject = "Unread 2",
                Body = "Body",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Inbox,
                IsRead = false,
                SentAt = DateTime.UtcNow
            }
        };
        _context.Messages.AddRange(unreadMessages);
        await _context.SaveChangesAsync();

        // Act
        var count = await _sut.GetUnreadCountAsync(_testRecipient.Id);

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task GetMessageStatsAsync_ShouldReturnCorrectStats()
    {
        // Arrange
        await SeedMessagesAsync();

        // Act
        var stats = await _sut.GetMessageStatsAsync(_testSender.Id);

        // Assert
        stats.Should().NotBeNull();
        stats.TotalInbox.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalSent.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalDrafts.Should().BeGreaterThanOrEqualTo(0);
        stats.UnreadCount.Should().BeGreaterThanOrEqualTo(0);
    }

    private async Task SeedMessagesAsync()
    {
        var messages = new[]
        {
            new Message
            {
                Subject = "Important Update",
                Body = "This is an important message",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                IsRead = false,
                SentAt = DateTime.UtcNow
            },
            new Message
            {
                Subject = "Regular Message",
                Body = "Regular content",
                SenderId = _testSender.Id,
                RecipientId = _testRecipient.Id,
                Status = MessageStatus.Sent,
                Folder = MessageFolder.Sent,
                IsRead = true,
                SentAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow
            },
            new Message
            {
                Subject = "Draft Message",
                Body = "Draft content",
                SenderId = _testSender.Id,
                Status = MessageStatus.Draft,
                Folder = MessageFolder.Drafts
            }
        };

        _context.Messages.AddRange(messages);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
