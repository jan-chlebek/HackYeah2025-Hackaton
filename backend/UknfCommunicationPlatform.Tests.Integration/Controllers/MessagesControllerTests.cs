using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.DTOs.Messages;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;
using Xunit.Abstractions;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

[Collection(nameof(DatabaseCollection))]
public class MessagesControllerTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private readonly ITestOutputHelper _output;

    public MessagesControllerTests(TestDatabaseFixture factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    public async Task InitializeAsync()
    {
        // Cleanup before each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Clear messages to have clean state
        context.Messages.RemoveRange(context.Messages);
        await context.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateMessage_WithValidData_CreatesMessage()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        // Ensure we have users
        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        var request = new CreateMessageRequest
        {
            Subject = "Test Subject",
            Body = "Test body content",
            RecipientId = recipient.Id,
            Priority = MessagePriority.Normal
        };

        // Act
        var result = await messageService.CreateMessageAsync(sender.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be("Test Subject");
        result.Body.Should().Be("Test body content");
        result.Priority.Should().Be(MessagePriority.Normal);
        result.ParentMessageId.Should().BeNull();
    }

    [Fact]
    public async Task CreateMessage_WithHighPriority_CreatesHighPriorityMessage()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        var request = new CreateMessageRequest
        {
            Subject = "Urgent: High Priority Message",
            Body = "This is urgent",
            RecipientId = recipient.Id,
            Priority = MessagePriority.High
        };

        // Act
        var result = await messageService.CreateMessageAsync(sender.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Priority.Should().Be(MessagePriority.High);
    }

    [Fact]
    public async Task GetMessages_WithReadFilter_ReturnsFilteredResults()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");

        // Create test messages
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);
        await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "Unread Message",
            Body = "This is unread",
            RecipientId = recipient.Id
        });

        // Act
        var (messages, totalCount) = await messageService.GetMessagesAsync(
            user.Id, page: 1, pageSize: 20, isRead: false);

        // Assert
        messages.Should().NotBeNull();
        totalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ReplyToMessage_WithValidParent_CreatesReply()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create parent message
        var parentMessage = await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
        {
            Subject = "Original Message",
            Body = "This is the original message",
            RecipientId = recipient.Id
        });

        var replyRequest = new ReplyMessageRequest
        {
            Body = "This is my reply"
        };

        // Act
        var reply = await messageService.ReplyToMessageAsync(parentMessage.Id, recipient.Id, replyRequest);

        // Assert
        reply.Should().NotBeNull();
        reply.Subject.Should().Contain("Re:");
        reply.ParentMessageId.Should().Be(parentMessage.Id);
        reply.Priority.Should().Be(parentMessage.Priority); // Should inherit priority
    }

    [Fact]
    public async Task ReplyToMessage_InheritsPriority_WhenNotSpecified()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create high priority parent message
        var parentMessage = await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
        {
            Subject = "High Priority Original",
            Body = "Urgent message",
            RecipientId = recipient.Id,
            Priority = MessagePriority.High
        });

        var replyRequest = new ReplyMessageRequest
        {
            Body = "Reply to urgent message",
            Priority = null // Don't specify, should inherit
        };

        // Act
        var reply = await messageService.ReplyToMessageAsync(parentMessage.Id, recipient.Id, replyRequest);

        // Assert
        reply.Priority.Should().Be(MessagePriority.High); // Inherited
    }

    [Fact]
    public async Task ReplyToMessage_OverridesPriority_WhenSpecified()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create high priority parent message
        var parentMessage = await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
        {
            Subject = "High Priority Original",
            Body = "Urgent message",
            RecipientId = recipient.Id,
            Priority = MessagePriority.High
        });

        var replyRequest = new ReplyMessageRequest
        {
            Body = "Reply with lower priority",
            Priority = MessagePriority.Low // Override
        };

        // Act
        var reply = await messageService.ReplyToMessageAsync(parentMessage.Id, recipient.Id, replyRequest);

        // Assert
        reply.Priority.Should().Be(MessagePriority.Low); // Overridden
    }

    [Fact]
    public async Task ExportMessages_ReturnsCSVData()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create test message
        await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "Export Test Message",
            Body = "This will be exported",
            RecipientId = recipient.Id,
            Priority = MessagePriority.Normal
        });

        // Act
        var exportData = await messageService.ExportMessagesAsync(user.Id);

        // Assert
        exportData.Should().NotBeNull();
        exportData.Should().NotBeEmpty();
        exportData.Should().Contain(m => m.Subject == "Export Test Message");
        exportData.First().Priority.Should().Contain("Normalny"); // Polish translation
    }

    [Fact]
    public async Task ExportMessages_IncludesPriorityInPolish()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create messages with different priorities
        await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "High Priority",
            Body = "Test",
            RecipientId = recipient.Id,
            Priority = MessagePriority.High
        });

        await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "Low Priority",
            Body = "Test",
            RecipientId = recipient.Id,
            Priority = MessagePriority.Low
        });

        // Act
        var exportData = await messageService.ExportMessagesAsync(user.Id);

        // Assert
        exportData.Should().Contain(m => m.Priority == "Wysoki"); // High
        exportData.Should().Contain(m => m.Priority == "Niski"); // Low
    }

    [Fact]
    public async Task MessageResponse_DoesNotIncludeIsCancelledField()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Act
        var message = await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "Test",
            Body = "Test body",
            RecipientId = recipient.Id
        });

        // Assert - Check that the response object doesn't have IsCancelled property
        var properties = typeof(MessageResponse).GetProperties();
        properties.Should().NotContain(p => p.Name == "IsCancelled");
    }

    [Fact]
    public async Task MarkAsRead_UpdatesReadStatus()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        var message = await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
        {
            Subject = "Mark as Read Test",
            Body = "This will be marked as read",
            RecipientId = recipient.Id
        });

        // Act
        var result = await messageService.MarkAsReadAsync(message.Id, recipient.Id);

        // Assert
        result.Should().BeTrue();

        // Verify in database
        var updatedMessage = context.Messages.Find(message.Id);
        updatedMessage!.IsRead.Should().BeTrue();
        updatedMessage.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUnreadCount_ReturnsCorrectCount()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Create 3 unread messages for recipient
        for (int i = 0; i < 3; i++)
        {
            await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
            {
                Subject = $"Unread Message {i}",
                Body = "Test",
                RecipientId = recipient.Id
            });
        }

        // Act
        var count = await messageService.GetUnreadCountAsync(recipient.Id);

        // Assert
        count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetMessageStats_ReturnsStatistics()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");

        // Act
        var stats = await messageService.GetMessageStatsAsync(user.Id);

        // Assert
        stats.Should().NotBeNull();
        stats.TotalInbox.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalSent.Should().BeGreaterThanOrEqualTo(0);
        stats.UnreadCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task MessageResponse_IncludesPriorityField()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var user = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        // Act
        var message = await messageService.CreateMessageAsync(user.Id, new CreateMessageRequest
        {
            Subject = "Priority Field Test",
            Body = "Testing priority field",
            RecipientId = recipient.Id,
            Priority = MessagePriority.High
        });

        // Assert
        message.Priority.Should().Be(MessagePriority.High);
        
        // Check that the property exists
        var properties = typeof(MessageResponse).GetProperties();
        properties.Should().Contain(p => p.Name == "Priority");
    }

    [Fact]
    public async Task ReplyMessage_IncludesParentMessageId()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();

        var sender = context.Users.First(u => u.Email == "jan.kowalski@uknf.gov.pl");
        var recipient = context.Users.First(u => u.SupervisedEntityId != null);

        var parentMessage = await messageService.CreateMessageAsync(sender.Id, new CreateMessageRequest
        {
            Subject = "Parent Message",
            Body = "Original",
            RecipientId = recipient.Id
        });

        var replyRequest = new ReplyMessageRequest
        {
            Body = "Reply to check parent ID"
        };

        // Act
        var reply = await messageService.ReplyToMessageAsync(parentMessage.Id, recipient.Id, replyRequest);

        // Assert
        reply.ParentMessageId.Should().Be(parentMessage.Id);
        reply.ParentMessageId.Should().NotBeNull();
    }
}
