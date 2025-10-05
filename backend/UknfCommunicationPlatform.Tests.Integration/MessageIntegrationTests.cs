using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace UknfCommunicationPlatform.Tests.Integration;

/// <summary>
/// Integration tests for Message entity and related operations
/// </summary>
public class MessageIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public MessageIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CanCreateMessageWithSenderAndRecipient()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var senderEmail = $"sender{Guid.NewGuid().ToString().Substring(0, 8)}@uknf.gov.pl";
        var recipientEmail = $"recipient{Guid.NewGuid().ToString().Substring(0, 8)}@bank.com";

        var sender = new User
        {
            Email = senderEmail,
            FirstName = "John",
            LastName = "Sender",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var recipient = new User
        {
            Email = recipientEmail,
            FirstName = "Jane",
            LastName = "Recipient",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(sender, recipient);
        await context.SaveChangesAsync();

        var message = new Message
        {
            Subject = "Test Message",
            Body = "This is a test message body",
            SentAt = DateTime.UtcNow,
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            IsRead = false
        };

        // Act
        context.Messages.Add(message);
        await context.SaveChangesAsync();

        var retrieved = await context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .FirstOrDefaultAsync(m => m.Subject == "Test Message" && m.SenderId == sender.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Subject.Should().Be("Test Message");
        retrieved.Sender.Should().NotBeNull();
        retrieved.Sender!.Email.Should().Be(senderEmail);
        retrieved.Recipient.Should().NotBeNull();
        retrieved.Recipient!.Email.Should().Be(recipientEmail);
        retrieved.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task CanMarkMessageAsRead()
    {
        // Arrange
        using var scope = _factory.CreateDbContextScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var senderEmail = $"sender{Guid.NewGuid().ToString().Substring(0, 8)}@uknf.gov.pl";
        var recipientEmail = $"recipient{Guid.NewGuid().ToString().Substring(0, 8)}@bank.com";

        var sender = new User
        {
            Email = senderEmail,
            FirstName = "Alice",
            LastName = "Sender",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var recipient = new User
        {
            Email = recipientEmail,
            FirstName = "Bob",
            LastName = "Recipient",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(sender, recipient);
        await context.SaveChangesAsync();

        var message = new Message
        {
            Subject = "Unread Message",
            Body = "This message should be marked as read",
            SentAt = DateTime.UtcNow,
            SenderId = sender.Id,
            RecipientId = recipient.Id,
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
