using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Integration tests for Message entity and related operations
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class MessageIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public MessageIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        // Use lightweight reset - only clean test-created data, keep seed data
        await _factory.ResetTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CanCreateMessageWithSenderAndRecipient()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get user IDs without tracking
        var senderId = await context.Users
            .Where(u => u.Email == "jan.kowalski@uknf.gov.pl")
            .Select(u => u.Id)
            .FirstAsync();
        var recipientId = await context.Users
            .Where(u => u.Email.Contains("@bank"))
            .Select(u => u.Id)
            .FirstAsync();

        var testSubject = $"Test Message {Guid.NewGuid().ToString().Substring(0, 8)}";
        var message = new Message
        {
            Subject = testSubject,
            Body = "This is a test message body",
            SentAt = DateTime.UtcNow,
            SenderId = senderId,
            RecipientId = recipientId,
            IsRead = false
        };

        // Act
        context.Messages.Add(message);
        await context.SaveChangesAsync();

        var retrieved = await context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .FirstOrDefaultAsync(m => m.Subject == testSubject && m.SenderId == senderId);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Subject.Should().Be(testSubject);
        retrieved.Sender.Should().NotBeNull();
        retrieved.Sender!.Email.Should().Be("jan.kowalski@uknf.gov.pl");
        retrieved.Recipient.Should().NotBeNull();
        retrieved.Recipient!.Email.Should().Contain("@bank");
        retrieved.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task CanMarkMessageAsRead()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get user IDs without tracking to avoid EF Core attaching related entities
        var senderId = await context.Users
            .Where(u => u.Email == "jan.kowalski@uknf.gov.pl")
            .Select(u => u.Id)
            .FirstAsync();
        var recipientId = await context.Users
            .Where(u => u.Email.Contains("@bank"))
            .Select(u => u.Id)
            .FirstAsync();

        var message = new Message
        {
            Subject = "Unread Message",
            Body = "This message should be marked as read",
            SentAt = DateTime.UtcNow,
            SenderId = senderId,
            RecipientId = recipientId,
            IsRead = false
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync();

        // Act
        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var retrieved = await context.Messages
            .FirstOrDefaultAsync(m => m.Id == message.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.IsRead.Should().BeTrue();
        retrieved.ReadAt.Should().NotBeNull();
    }
}
