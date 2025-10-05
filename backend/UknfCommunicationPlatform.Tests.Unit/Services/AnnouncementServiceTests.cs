using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Core.DTOs.Announcements;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for AnnouncementService
/// </summary>
public class AnnouncementServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<AnnouncementService>> _mockLogger;
    private readonly AnnouncementService _sut;
    private readonly User _uknfUser;
    private readonly User _entityUser1;
    private readonly User _entityUser2;

    public AnnouncementServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<AnnouncementService>>();
        _sut = new AnnouncementService(_context, _mockLogger.Object);

        // Create test users
        _uknfUser = new User
        {
            Id = 1,
            Email = "uknf@uknf.gov.pl",
            FirstName = "Jan",
            LastName = "Kowalski",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _entityUser1 = new User
        {
            Id = 2,
            Email = "user1@bank.com",
            FirstName = "Anna",
            LastName = "Nowak",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _entityUser2 = new User
        {
            Id = 3,
            Email = "user2@insurance.com",
            FirstName = "Piotr",
            LastName = "Wisniewski",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.AddRange(_uknfUser, _entityUser1, _entityUser2);
        _context.SaveChanges();
    }

    #region CreateAnnouncementAsync Tests

    [Fact]
    public async Task CreateAnnouncementAsync_WithValidData_ShouldCreateAnnouncement()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = "Test Announcement",
            Content = "This is a test announcement content."
        };

        // Act
        var result = await _sut.CreateAnnouncementAsync(request, _uknfUser.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Announcement");
        result.Content.Should().Be("This is a test announcement content.");
        result.CreatedByUserId.Should().Be(_uknfUser.Id);
        result.CreatedByName.Should().Be("Jan Kowalski");
        result.IsReadByCurrentUser.Should().BeFalse();
        result.ReadAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateAnnouncementAsync_ShouldTrimTitleAndContent()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = "  Test Title  ",
            Content = "  Test Content  "
        };

        // Act
        var result = await _sut.CreateAnnouncementAsync(request, _uknfUser.Id);

        // Assert
        result.Title.Should().Be("Test Title");
        result.Content.Should().Be("Test Content");
    }

    [Fact]
    public async Task CreateAnnouncementAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = "Persisted Announcement",
            Content = "This should be saved."
        };

        // Act
        var result = await _sut.CreateAnnouncementAsync(request, _uknfUser.Id);

        // Assert
        var savedAnnouncement = await _context.Announcements.FindAsync(result.Id);
        savedAnnouncement.Should().NotBeNull();
        savedAnnouncement!.Title.Should().Be("Persisted Announcement");
    }

    #endregion

    #region GetAnnouncementsAsync Tests

    [Fact]
    public async Task GetAnnouncementsAsync_WithNoAnnouncements_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetAnnouncementsAsync_WithAnnouncements_ShouldReturnPaginatedList()
    {
        // Arrange
        await SeedAnnouncementsAsync(5);

        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id, page: 1, pageSize: 3);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetAnnouncementsAsync_ShouldOrderByCreatedAtDescending()
    {
        // Arrange
        await SeedAnnouncementsAsync(3);

        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id);

        // Assert
        result.Items.Should().BeInDescendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public async Task GetAnnouncementsAsync_ShouldTruncateContentPreview()
    {
        // Arrange
        var longContent = new string('A', 300);
        var announcement = new Announcement
        {
            Title = "Test",
            Content = longContent,
            CreatedByUserId = _uknfUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = "General",
            Priority = AnnouncementPriority.Low,
            IsPublished = false
        };
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id);

        // Assert
        result.Items.First().ContentPreview.Length.Should().Be(203); // 200 chars + "..."
        result.Items.First().ContentPreview.Should().EndWith("...");
    }

    [Fact]
    public async Task GetAnnouncementsAsync_WithUnreadOnlyFilter_ShouldReturnOnlyUnread()
    {
        // Arrange
        await SeedAnnouncementsAsync(3);
        var announcements = await _context.Announcements.ToListAsync();

        // Mark first announcement as read
        await _sut.MarkAsReadAsync(announcements[0].Id, _entityUser1.Id);

        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id, unreadOnly: true);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(x => x.IsReadByCurrentUser.Should().BeFalse());
    }

    [Fact]
    public async Task GetAnnouncementsAsync_ShouldIndicateReadStatus()
    {
        // Arrange
        await SeedAnnouncementsAsync(2);
        var announcements = await _context.Announcements.ToListAsync();

        // Mark first announcement as read
        await _sut.MarkAsReadAsync(announcements[0].Id, _entityUser1.Id);

        // Act
        var result = await _sut.GetAnnouncementsAsync(_entityUser1.Id);

        // Assert
        var readItem = result.Items.First(x => x.Id == announcements[0].Id);
        var unreadItem = result.Items.First(x => x.Id == announcements[1].Id);

        readItem.IsReadByCurrentUser.Should().BeTrue();
        unreadItem.IsReadByCurrentUser.Should().BeFalse();
    }

    #endregion

    #region GetAnnouncementByIdAsync Tests

    [Fact]
    public async Task GetAnnouncementByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetAnnouncementByIdAsync(999, _entityUser1.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAnnouncementByIdAsync_WithValidId_ShouldReturnFullDetails()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test Announcement", "Full content here");

        // Act
        var result = await _sut.GetAnnouncementByIdAsync(announcement.Id, _entityUser1.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(announcement.Id);
        result.Title.Should().Be("Test Announcement");
        result.Content.Should().Be("Full content here");
        result.CreatedByName.Should().Be("Jan Kowalski");
    }

    [Fact]
    public async Task GetAnnouncementByIdAsync_WhenRead_ShouldIndicateReadStatus()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");
        await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id);

        // Act
        var result = await _sut.GetAnnouncementByIdAsync(announcement.Id, _entityUser1.Id);

        // Assert
        result!.IsReadByCurrentUser.Should().BeTrue();
        result.ReadAt.Should().NotBeNull();
        result.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task GetAnnouncementByIdAsync_WhenUnread_ShouldIndicateUnreadStatus()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");

        // Act
        var result = await _sut.GetAnnouncementByIdAsync(announcement.Id, _entityUser1.Id);

        // Assert
        result!.IsReadByCurrentUser.Should().BeFalse();
        result.ReadAt.Should().BeNull();
    }

    #endregion

    #region UpdateAnnouncementAsync Tests

    [Fact]
    public async Task UpdateAnnouncementAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateAnnouncementRequest
        {
            Title = "Updated",
            Content = "Updated content"
        };

        // Act
        var result = await _sut.UpdateAnnouncementAsync(999, request);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAnnouncementAsync_WithValidData_ShouldUpdateAnnouncement()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Original Title", "Original Content");
        var request = new UpdateAnnouncementRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        // Act
        var result = await _sut.UpdateAnnouncementAsync(announcement.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Content.Should().Be("Updated Content");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task UpdateAnnouncementAsync_ShouldPersistChanges()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Original", "Original");
        var request = new UpdateAnnouncementRequest
        {
            Title = "Updated",
            Content = "Updated"
        };

        // Act
        await _sut.UpdateAnnouncementAsync(announcement.Id, request);

        // Assert
        var updated = await _context.Announcements.FindAsync(announcement.Id);
        updated!.Title.Should().Be("Updated");
        updated.Content.Should().Be("Updated");
    }

    #endregion

    #region DeleteAnnouncementAsync Tests

    [Fact]
    public async Task DeleteAnnouncementAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteAnnouncementAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAnnouncementAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("To Delete", "Content");

        // Act
        var result = await _sut.DeleteAnnouncementAsync(announcement.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _context.Announcements.FindAsync(announcement.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAnnouncementAsync_ShouldCascadeDeleteReadRecords()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("To Delete", "Content");
        await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id);

        // Act
        await _sut.DeleteAnnouncementAsync(announcement.Id);

        // Assert
        var readRecords = await _context.AnnouncementReads
            .Where(ar => ar.AnnouncementId == announcement.Id)
            .ToListAsync();
        readRecords.Should().BeEmpty();
    }

    #endregion

    #region MarkAsReadAsync Tests

    [Fact]
    public async Task MarkAsReadAsync_WithNonExistentAnnouncement_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.MarkAsReadAsync(999, _entityUser1.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsReadAsync_FirstTime_ShouldCreateReadRecordAndReturnTrue()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");

        // Act
        var result = await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id, "192.168.1.1");

        // Assert
        result.Should().BeTrue();
        var readRecord = await _context.AnnouncementReads
            .FirstOrDefaultAsync(ar => ar.AnnouncementId == announcement.Id && ar.UserId == _entityUser1.Id);

        readRecord.Should().NotBeNull();
        readRecord!.IpAddress.Should().Be("192.168.1.1");
        readRecord.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task MarkAsReadAsync_AlreadyRead_ShouldReturnFalse()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");
        await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id);

        // Act
        var result = await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task MarkAsReadAsync_DifferentUsers_ShouldCreateSeparateRecords()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");

        // Act
        var result1 = await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id);
        var result2 = await _sut.MarkAsReadAsync(announcement.Id, _entityUser2.Id);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();

        var readRecords = await _context.AnnouncementReads
            .Where(ar => ar.AnnouncementId == announcement.Id)
            .ToListAsync();

        readRecords.Should().HaveCount(2);
    }

    [Fact]
    public async Task MarkAsReadAsync_WithoutIpAddress_ShouldStillWork()
    {
        // Arrange
        var announcement = await CreateAnnouncementAsync("Test", "Content");

        // Act
        var result = await _sut.MarkAsReadAsync(announcement.Id, _entityUser1.Id, ipAddress: null);

        // Assert
        result.Should().BeTrue();
        var readRecord = await _context.AnnouncementReads
            .FirstOrDefaultAsync(ar => ar.AnnouncementId == announcement.Id);

        readRecord!.IpAddress.Should().BeNull();
    }

    #endregion

    #region Helper Methods

    private async Task<Announcement> CreateAnnouncementAsync(string title, string content)
    {
        var announcement = new Announcement
        {
            Title = title,
            Content = content,
            CreatedByUserId = _uknfUser.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = "General",
            Priority = AnnouncementPriority.Low,
            IsPublished = false
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        return announcement;
    }

    private async Task SeedAnnouncementsAsync(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var announcement = new Announcement
            {
                Title = $"Announcement {i + 1}",
                Content = $"Content for announcement {i + 1}",
                CreatedByUserId = _uknfUser.Id,
                CreatedAt = DateTime.UtcNow.AddMinutes(-count + i),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-count + i),
                Category = "General",
                Priority = AnnouncementPriority.Low,
                IsPublished = false
            };

            _context.Announcements.Add(announcement);
        }

        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #endregion
}
