using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.DTOs.Cases;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

public class CasesControllerTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private HttpClient? _client;

    public CasesControllerTests(TestDatabaseFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = GetAuthenticatedClient();
        await Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client?.Dispose();
        return Task.CompletedTask;
    }

    private HttpClient GetAuthenticatedClient()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Get an admin user
        var admin = context.Users.First(u => u.Email == "admin@uknf.gov.pl");
        
        var client = _factory.CreateClient();
        var token = _factory.GenerateJwtToken(admin.Id, admin.Email, "UKNF");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task GetCases_ShouldReturnPaginatedList()
    {
        // Act
        var response = await _client!.GetAsync("/api/v1/cases?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"data\"", content);
        Assert.Contains("\"pagination\"", content);
    }

    [Fact]
    public async Task GetCase_WithValidId_ShouldReturnCase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingCase = context.Cases.First();

        // Act
        var response = await _client!.GetAsync($"/api/v1/cases/{existingCase.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var caseResponse = await response.Content.ReadFromJsonAsync<CaseResponse>();
        Assert.NotNull(caseResponse);
        Assert.Equal(existingCase.Id, caseResponse.Id);
    }

    [Fact]
    public async Task GetCase_WithInvalidId_ShouldReturn404()
    {
        // Act
        var response = await _client!.GetAsync("/api/v1/cases/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCase_WithValidData_ShouldCreateCase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entity = context.SupervisedEntities.First();

        var request = new CreateCaseRequest
        {
            Title = "Test Case",
            Description = "Test Description",
            Category = "TestCategory",
            SupervisedEntityId = entity.Id,
            Priority = 2
        };

        // Act
        var response = await _client!.PostAsJsonAsync("/api/v1/cases", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var caseResponse = await response.Content.ReadFromJsonAsync<CaseResponse>();
        Assert.NotNull(caseResponse);
        Assert.Equal("Test Case", caseResponse.Title);
        Assert.Equal("Test Description", caseResponse.Description);
    }

    [Fact]
    public async Task UpdateCase_WithValidData_ShouldUpdateCase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingCase = context.Cases.First();

        var request = new UpdateCaseRequest
        {
            Title = "Updated Title",
            Priority = 3
        };

        // Act
        var response = await _client!.PutAsJsonAsync($"/api/v1/cases/{existingCase.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var caseResponse = await response.Content.ReadFromJsonAsync<CaseResponse>();
        Assert.NotNull(caseResponse);
        Assert.Equal("Updated Title", caseResponse.Title);
        Assert.Equal(3, caseResponse.Priority);
    }

    [Fact]
    public async Task DeleteCase_WithValidId_ShouldDeleteCase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var caseToDelete = context.Cases.First(c => !c.IsCancelled);

        // Act
        var response = await _client!.DeleteAsync($"/api/v1/cases/{caseToDelete.Id}?reason=Test deletion");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCaseStatus_WithValidStatus_ShouldUpdateStatus()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var existingCase = context.Cases.First(c => c.Status == CaseStatus.New);

        // Act
        var response = await _client!.PatchAsync(
            $"/api/v1/cases/{existingCase.Id}/status?newStatus={CaseStatus.InProgress}", 
            null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var caseResponse = await response.Content.ReadFromJsonAsync<CaseResponse>();
        Assert.NotNull(caseResponse);
        Assert.Equal(CaseStatus.InProgress, caseResponse.Status);
    }

    [Fact]
    public async Task GetCases_WithFilters_ShouldReturnFilteredCases()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entity = context.SupervisedEntities.First();

        // Act
        var response = await _client!.GetAsync(
            $"/api/v1/cases?supervisedEntityId={entity.Id}&status={CaseStatus.New}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"data\"", content);
    }
}
