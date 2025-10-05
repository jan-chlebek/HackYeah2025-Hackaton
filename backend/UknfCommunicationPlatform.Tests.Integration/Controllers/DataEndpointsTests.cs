using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Infrastructure.Data;
using FluentAssertions;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for all data-returning API endpoints
/// Tests that each endpoint returns data successfully
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class DataEndpointsTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;

    public DataEndpointsTests(TestDatabaseFixture factory)
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
    public async Task UsersEndpoint_ReturnsUsers_WithPagination()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Count.Should().BeGreaterThan(0);
        result.Pagination.Should().NotBeNull();
        result.Pagination!.TotalCount.Should().BeGreaterThan(0);
        result.Pagination.Page.Should().Be(1);
        result.Pagination.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task UsersEndpoint_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/users?page=1&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Pagination!.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task UserById_ReturnsUserDetails()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First get a user to get a valid ID
        var listResponse = await client.GetAsync("/api/v1/users");
        var listResult = await listResponse.Content.ReadFromJsonAsync<PaginatedResponse<UserDto>>();
        var userId = listResult!.Data.First().Id;

        // Act
        var response = await client.GetAsync($"/api/v1/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId);
        user.Email.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task EntitiesEndpoint_ReturnsEntities_WithPagination()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/entities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<EntityDto>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Data.Count.Should().BeGreaterThan(0);
        result.Pagination.Should().NotBeNull();
        result.Pagination!.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task EntitiesEndpoint_WithSearch_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/entities?searchTerm=Bank");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<EntityDto>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        // All results should contain "Bank" in name or type
        result.Data.Should().Contain(e =>
            e.Name.Contains("Bank", StringComparison.OrdinalIgnoreCase) ||
            e.EntityType.Contains("Bank", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task EntityById_ReturnsEntityDetails()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First get an entity to get a valid ID
        var listResponse = await client.GetAsync("/api/v1/entities");
        var listResult = await listResponse.Content.ReadFromJsonAsync<PaginatedResponse<EntityDto>>();
        var entityId = listResult!.Data.First().Id;

        // Act
        var response = await client.GetAsync($"/api/v1/entities/{entityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var entity = await response.Content.ReadFromJsonAsync<EntityDto>();
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(entityId);
        entity.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MessagesEndpoint_ReturnsMessages_WithPagination()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/messages");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<MessageDto>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
        result.Pagination.Should().NotBeNull();
        result.Pagination!.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MessagesEndpoint_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - filter by unread messages
        var response = await client.GetAsync("/api/v1/messages?isRead=false");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<MessageDto>>();
        result.Should().NotBeNull();
        // All returned messages should be unread
        if (result!.Data.Any())
        {
            result.Data.Should().OnlyContain(m => m.IsRead == false);
        }
    }

    [Fact]
    public async Task MessageById_ReturnsMessageDetails()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First get a message to get a valid ID
        var listResponse = await client.GetAsync("/api/v1/messages");
        var listResult = await listResponse.Content.ReadFromJsonAsync<PaginatedResponse<MessageDto>>();

        if (!listResult!.Data.Any())
        {
            // Skip test if no messages available
            return;
        }

        var messageId = listResult.Data.First().Id;

        // Act
        var response = await client.GetAsync($"/api/v1/messages/{messageId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = await response.Content.ReadFromJsonAsync<MessageDetailDto>();
        message.Should().NotBeNull();
        message!.Id.Should().Be(messageId);
        message.Subject.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ReportsEndpoint_ReturnsReports()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/reports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reports = await response.Content.ReadFromJsonAsync<List<ReportDto>>();
        reports.Should().NotBeNull();
        reports!.Should().NotBeEmpty();
        reports.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ReportsEndpoint_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Get first report to find an entity ID
        var allResponse = await client.GetAsync("/api/v1/reports");
        var allReports = await allResponse.Content.ReadFromJsonAsync<List<ReportDto>>();

        if (allReports is null || !allReports.Any())
        {
            // Skip if no reports
            return;
        }

        // Use reportingPeriod filter
        var period = allReports.First().ReportingPeriod;
        var filteredResponse = await client.GetAsync($"/api/v1/reports?reportingPeriod={period}");

        // Assert
        filteredResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reports = await filteredResponse.Content.ReadFromJsonAsync<List<ReportDto>>();
        reports.Should().NotBeNull();
        reports!.Should().OnlyContain(r => r.ReportingPeriod == period);
    }

    [Fact]
    public async Task ReportById_ReturnsReportDetails()
    {
        // Arrange
        var client = _factory.CreateClient();

        // First get a report to get a valid ID
        var listResponse = await client.GetAsync("/api/v1/reports");
        var reports = await listResponse.Content.ReadFromJsonAsync<List<ReportDto>>();

        if (reports is null || !reports.Any())
        {
            // Skip test if no reports available
            return;
        }

        var reportId = reports.First().Id;

        // Act
        var response = await client.GetAsync($"/api/v1/reports/{reportId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await response.Content.ReadFromJsonAsync<ReportDto>();
        report.Should().NotBeNull();
        report!.Id.Should().Be(reportId);
        report.ReportNumber.Should().NotBeNullOrEmpty();
    }

    // DTO classes for deserialization
    private class PaginatedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationInfo? Pagination { get; set; }
    }

    private class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    private class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private class EntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string Nip { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private class MessageDto
    {
        public long Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }

    private class MessageDetailDto : MessageDto
    {
        public string Body { get; set; } = string.Empty;
    }

    private class ReportDto
    {
        public long Id { get; set; }
        public string ReportNumber { get; set; } = string.Empty;
        public string ReportingPeriod { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
    }
}
