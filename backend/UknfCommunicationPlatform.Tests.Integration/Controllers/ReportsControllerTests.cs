using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.DTOs.Reports;
using UknfCommunicationPlatform.Infrastructure.Data;
using Xunit;
using Xunit.Abstractions;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for ReportsController
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class ReportsControllerTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private readonly ITestOutputHelper _output;

    public ReportsControllerTests(TestDatabaseFixture factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    public async Task InitializeAsync()
    {
        // Clean up reports before each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var reports = context.Reports.ToList();
        context.Reports.RemoveRange(reports);
        await context.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private HttpClient GetAuthenticatedClient()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = context.Users.First(u => u.SupervisedEntityId != null);
        
        var client = _factory.CreateClient();
        var token = _factory.GenerateJwtToken(user.Id, user.Email, "SupervisorUser");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task SubmitReport_WithValidXlsxFile_CreatesReport()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q1"), "reportingPeriod");

        // Create a minimal valid XLSX file
        var xlsxContent = new byte[]
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0x08, 0x00, 0x00, 0x00,
            0x21, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x13, 0x00, 0x08, 0x02, 0x5B, 0x43, 0x6F, 0x6E, 0x74, 0x65,
            0x6E, 0x74, 0x5F, 0x54, 0x79, 0x70, 0x65, 0x73, 0x5D, 0x2E, 0x78, 0x6D,
            0x6C, 0x20, 0xA2, 0x04, 0x02, 0x28, 0xA0, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x50, 0x4B, 0x05, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00,
            0x41, 0x00, 0x00, 0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        var fileContent = new ByteArrayContent(xlsxContent);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        formData.Add(fileContent, "file", "test_report_Q1_2025.xlsx");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ReportDto>();
        result.Should().NotBeNull();
        result!.ReportNumber.Should().MatchRegex(@"RPT-\d{4}-\d{4}");
        result.ReportingPeriod.Should().Be("Q1");
        result.FileName.Should().Be("test_report_Q1_2025.xlsx");

        // Verify in database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var savedReport = context.Reports.FirstOrDefault(r => r.Id == result.Id);
        savedReport.Should().NotBeNull();
        savedReport!.FileName.Should().Be("test_report_Q1_2025.xlsx");
        savedReport.FileContent.Should().HaveCount(xlsxContent.Length);
    }

    [Fact]
    public async Task SubmitReport_WithNonXlsxFile_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q1"), "reportingPeriod");

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("This is a PDF"));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        formData.Add(fileContent, "file", "document.pdf");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(".xlsx");
    }

    [Fact]
    public async Task SubmitReport_WithInvalidMimeType_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q2"), "reportingPeriod");

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Fake XLSX"));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        formData.Add(fileContent, "file", "fake.xlsx");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("content type");
    }

    [Fact]
    public async Task SubmitReport_WithNoFile_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q1"), "reportingPeriod");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitReport_WithInvalidReportingPeriod_ReturnsBadRequest()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q5"), "reportingPeriod"); // Invalid

        var xlsxContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var fileContent = new ByteArrayContent(xlsxContent);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        formData.Add(fileContent, "file", "test.xlsx");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid reporting period");
    }

    [Fact]
    public async Task SubmitReport_GeneratesUniqueReportNumbers()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        var xlsxContent = new byte[]
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0x08, 0x00, 0x00, 0x00,
            0x21, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        // Act - Submit two reports
        var formData1 = new MultipartFormDataContent();
        formData1.Add(new StringContent("Q1"), "reportingPeriod");
        var fileContent1 = new ByteArrayContent(xlsxContent);
        fileContent1.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        formData1.Add(fileContent1, "file", "report1.xlsx");

        var formData2 = new MultipartFormDataContent();
        formData2.Add(new StringContent("Q2"), "reportingPeriod");
        var fileContent2 = new ByteArrayContent(xlsxContent);
        fileContent2.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        formData2.Add(fileContent2, "file", "report2.xlsx");

        var response1 = await client.PostAsync("/api/v1/reports", formData1);
        var response2 = await client.PostAsync("/api/v1/reports", formData2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);

        var result1 = await response1.Content.ReadFromJsonAsync<ReportDto>();
        var result2 = await response2.Content.ReadFromJsonAsync<ReportDto>();

        result1!.ReportNumber.Should().NotBe(result2!.ReportNumber);
    }

    [Fact]
    public async Task DownloadReport_WithValidId_ReturnsFile()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = context.Users.First(u => u.SupervisedEntityId != null);
        var xlsxContent = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00 };

        var report = new Core.Entities.Report
        {
            ReportNumber = "RPT-2025-TEST",
            FileName = "download_test.xlsx",
            FileContent = xlsxContent,
            ReportingPeriod = Core.Enums.ReportingPeriod.Q1,
            Status = Core.Enums.ReportStatus.Draft,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Reports.Add(report);
        await context.SaveChangesAsync();

        var client = GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync($"/api/v1/reports/{report.Id}/download");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        var downloadedContent = await response.Content.ReadAsByteArrayAsync();
        downloadedContent.Should().Equal(xlsxContent);
    }

    [Fact]
    public async Task DownloadReport_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/reports/999999/download");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReports_ReturnsSubmittedReports()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = context.Users.First(u => u.SupervisedEntityId != null);
        var xlsxContent = new byte[] { 0x50, 0x4B };

        context.Reports.Add(new Core.Entities.Report
        {
            ReportNumber = "RPT-2025-0001",
            FileName = "q1.xlsx",
            FileContent = xlsxContent,
            ReportingPeriod = Core.Enums.ReportingPeriod.Q1,
            Status = Core.Enums.ReportStatus.Submitted,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var client = GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/reports");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reports = await response.Content.ReadFromJsonAsync<List<ReportDto>>();
        reports.Should().NotBeNull();
        reports.Should().HaveCountGreaterThanOrEqualTo(1);
        reports!.Should().Contain(r => r.FileName == "q1.xlsx");
    }

    [Fact]
    public async Task GetReports_WithFilter_ReturnsFilteredReports()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = context.Users.First(u => u.SupervisedEntityId != null);
        var xlsxContent = new byte[] { 0x50, 0x4B };

        context.Reports.AddRange(
            new Core.Entities.Report
            {
                ReportNumber = "RPT-2025-0001",
                FileName = "q1.xlsx",
                FileContent = xlsxContent,
                ReportingPeriod = Core.Enums.ReportingPeriod.Q1,
                Status = Core.Enums.ReportStatus.Draft,
                SubmittedAt = DateTime.UtcNow,
                SubmittedByUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Core.Entities.Report
            {
                ReportNumber = "RPT-2025-0002",
                FileName = "q2.xlsx",
                FileContent = xlsxContent,
                ReportingPeriod = Core.Enums.ReportingPeriod.Q2,
                Status = Core.Enums.ReportStatus.Draft,
                SubmittedAt = DateTime.UtcNow,
                SubmittedByUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        await context.SaveChangesAsync();

        var client = GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/reports?reportingPeriod=Q1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reports = await response.Content.ReadFromJsonAsync<List<ReportDto>>();
        reports.Should().NotBeNull();
        reports.Should().Contain(r => r.ReportingPeriod == "Q1");
        reports!.Should().Contain(r => r.FileName == "q1.xlsx");
    }

    [Fact]
    public async Task SubmitReport_StoresDraftStatus()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var client = GetAuthenticatedClient();

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("Q3"), "reportingPeriod");

        var xlsxContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var fileContent = new ByteArrayContent(xlsxContent);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        formData.Add(fileContent, "file", "q3_report.xlsx");

        // Act
        var response = await client.PostAsync("/api/v1/reports", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ReportDto>();

        var savedReport = context.Reports.Find(result!.Id);
        savedReport!.Status.Should().Be(Core.Enums.ReportStatus.Draft);
    }
}
