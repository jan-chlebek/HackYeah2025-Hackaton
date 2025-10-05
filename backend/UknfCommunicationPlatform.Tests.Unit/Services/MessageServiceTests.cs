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
            RecipientId = _testRecipient.Id
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
                IsRead = true,
                SentAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow
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
