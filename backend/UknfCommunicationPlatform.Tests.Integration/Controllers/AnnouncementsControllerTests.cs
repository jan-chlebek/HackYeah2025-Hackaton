using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.DTOs.Announcements;
using UknfCommunicationPlatform.Core.DTOs.Responses;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for AnnouncementsController
/// Tests full HTTP pipeline with real database
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class AnnouncementsControllerTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private readonly HttpClient _client;
    private User? _uknfUser;
    private User? _entityUser;

    public AnnouncementsControllerTests(TestDatabaseFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetTestDataAsync();
        await ClearAnnouncementsAsync(); // Ensure no pre-seeded baseline announcements interfere with deterministic counts
        await SeedTestUsersAsync();
        await RefreshAdminTokenAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region GET /api/v1/announcements

    [Fact]
    public async Task GetAnnouncements_ShouldReturnPaginatedList()
    {
        // Arrange
        await SeedAnnouncementsAsync(5);

        // Act
        var response = await _client.GetAsync("/api/v1/announcements?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AnnouncementListItemResponse>>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task GetAnnouncements_WithUnreadOnlyFilter_ShouldReturnOnlyUnread()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(3);

        // Mark first announcement as read by current authenticated user (admin)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var admin = context.Users.First(u => u.Email == "admin@uknf.gov.pl");
        context.AnnouncementReads.Add(new AnnouncementRead
        {
            AnnouncementId = announcements[0].Id,
            UserId = admin.Id,
            ReadAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/announcements?unreadOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AnnouncementListItemResponse>>();

        result!.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(x => x.IsReadByCurrentUser.Should().BeFalse());
    }

    [Fact]
    public async Task GetAnnouncements_ShouldIncludeReadStatus()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(2);

        // Mark first announcement as read by current authenticated user (admin)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var admin = context.Users.First(u => u.Email == "admin@uknf.gov.pl");
        context.AnnouncementReads.Add(new AnnouncementRead
        {
            AnnouncementId = announcements[0].Id,
            UserId = admin.Id,
            ReadAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/announcements");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<AnnouncementListItemResponse>>();

        var readItem = result!.Items.FirstOrDefault(x => x.Id == announcements[0].Id);
        var unreadItem = result.Items.FirstOrDefault(x => x.Id == announcements[1].Id);

        readItem!.IsReadByCurrentUser.Should().BeTrue();
        unreadItem!.IsReadByCurrentUser.Should().BeFalse();
    }

    #endregion

    #region GET /api/v1/announcements/{id}

    [Fact]
    public async Task GetAnnouncementById_WithValidId_ShouldReturnDetails()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(1);
        var announcementId = announcements[0].Id;

        // Act
        var response = await _client.GetAsync($"/api/v1/announcements/{announcementId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AnnouncementResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().Be(announcementId);
        result.Title.Should().NotBeNullOrEmpty();
        result.Content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAnnouncementById_WithNonExistentId_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/announcements/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/announcements

    [Fact]
    public async Task CreateAnnouncement_WithValidData_ShouldCreate()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = "New Test Announcement",
            Content = "This is the content of the new announcement."
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/announcements", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<AnnouncementResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Test Announcement");
        result.Content.Should().Be("This is the content of the new announcement.");

        response.Headers.Location.Should().NotBeNull();
        // Case-insensitive check for location header (controller uses "Announcements" with capital A)
        response.Headers.Location!.ToString().ToLowerInvariant().Should().Contain($"/api/v1/announcements/{result.Id}".ToLowerInvariant());
    }

    [Fact]
    public async Task CreateAnnouncement_WithInvalidData_ShouldReturn400()
    {
        // Arrange
        var request = new CreateAnnouncementRequest
        {
            Title = "", // Invalid - required
            Content = "Content"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/announcements", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAnnouncement_AsNonAdmin_ShouldReturn403()
    {
        // Arrange: switch auth to a non-admin (entity) user
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _entityUser ??= await context.Users.FirstOrDefaultAsync(u => u.SupervisedEntityId != null)
                          ?? await context.Users.FirstAsync(u => u.Email != "admin@uknf.gov.pl");
            var nonAdminToken = _factory.GenerateJwtToken(_entityUser.Id, _entityUser.Email, "InternalUser");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", nonAdminToken);
        }

        var request = new CreateAnnouncementRequest
        {
            Title = "Should Fail",
            Content = "Non admin attempt"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/announcements", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Restore admin token for subsequent tests
        await RefreshAdminTokenAsync();
    }

    #endregion

    #region PUT /api/v1/announcements/{id}

    [Fact]
    public async Task UpdateAnnouncement_WithValidData_ShouldUpdate()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(1);
        var announcementId = announcements[0].Id;

        var request = new UpdateAnnouncementRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/announcements/{announcementId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AnnouncementResponse>();

        result!.Title.Should().Be("Updated Title");
        result.Content.Should().Be("Updated Content");
    }

    [Fact]
    public async Task UpdateAnnouncement_WithNonExistentId_ShouldReturn404()
    {
        // Arrange
        var request = new UpdateAnnouncementRequest
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/announcements/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/v1/announcements/{id}

    [Fact]
    public async Task DeleteAnnouncement_WithValidId_ShouldDelete()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(1);
        var announcementId = announcements[0].Id;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/announcements/{announcementId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/v1/announcements/{announcementId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAnnouncement_WithNonExistentId_ShouldReturn404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/announcements/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/announcements/{id}/read

    [Fact]
    public async Task MarkAnnouncementAsRead_FirstTime_ShouldSucceed()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(1);
        var announcementId = announcements[0].Id;

        // Act
        var response = await _client.PostAsync($"/api/v1/announcements/{announcementId}/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify read status
        var getResponse = await _client.GetAsync($"/api/v1/announcements/{announcementId}");
        var announcement = await getResponse.Content.ReadFromJsonAsync<AnnouncementResponse>();

        announcement!.IsReadByCurrentUser.Should().BeTrue();
        announcement.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkAnnouncementAsRead_AlreadyRead_ShouldReturn404()
    {
        // Arrange
        var announcements = await SeedAnnouncementsAsync(1);
        var announcementId = announcements[0].Id;

        // Mark as read first time
        await _client.PostAsync($"/api/v1/announcements/{announcementId}/read", null);

        // Act - Try to mark as read again
        var response = await _client.PostAsync($"/api/v1/announcements/{announcementId}/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkAnnouncementAsRead_WithNonExistentId_ShouldReturn404()
    {
        // Act
        var response = await _client.PostAsync("/api/v1/announcements/99999/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task SeedTestUsersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get existing seeded users or create new ones
        _uknfUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@uknf.gov.pl")
                    ?? await context.Users.FirstAsync(u => u.SupervisedEntityId == null);

        _entityUser = await context.Users.FirstOrDefaultAsync(u => u.SupervisedEntityId != null);

        if (_entityUser == null)
        {
            // Create entity user if none exists
            _entityUser = new User
            {
                Email = "test.entity@test.com",
                FirstName = "Test",
                LastName = "Entity",
                PasswordHash = "hash",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(_entityUser);
            await context.SaveChangesAsync();
        }
    }

    private async Task<List<Announcement>> SeedAnnouncementsAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var announcements = new List<Announcement>();

        for (int i = 0; i < count; i++)
        {
            var announcement = new Announcement
            {
                Title = $"Test Announcement {i + 1}",
                Content = $"This is the content of test announcement {i + 1}. " + new string('A', 250),
                CreatedByUserId = _uknfUser!.Id,
                CreatedAt = DateTime.UtcNow.AddMinutes(-count + i),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-count + i),
                Category = "General",
                Priority = AnnouncementPriority.Low,
                IsPublished = false
            };

            context.Announcements.Add(announcement);
            announcements.Add(announcement);
        }

        await context.SaveChangesAsync();
        // Ensure token still valid with current DB role/permissions (defensive)
        await RefreshAdminTokenAsync();
        return announcements;
    }

    private async Task RefreshAdminTokenAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var admin = await context.Users.FirstAsync(u => u.Email == "admin@uknf.gov.pl");
        var token = _factory.GenerateJwtToken(admin.Id, admin.Email, "Administrator");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    private async Task ClearAnnouncementsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Remove dependents first to avoid FK issues
        await context.Database.ExecuteSqlRawAsync("DELETE FROM announcement_reads");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM announcement_attachments");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM announcement_histories");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM announcement_recipients");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM announcements");
    }

    #endregion
}
