using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Api.Controllers.v1;
using UknfCommunicationPlatform.Core.DTOs.Reports;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Controllers;

public class ReportsControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReportsService _service;
    private readonly ReportsController _controller;
    private readonly Mock<ILogger<ReportsController>> _logger;

    public ReportsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new ReportsService(_context);
        _logger = new Mock<ILogger<ReportsController>>();
        _controller = new ReportsController(_service, _logger.Object);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        // Create test user
        var user = new User
        {
            Id = 1,
            Email = "test@uknf.pl",
            FirstName = "Jan",
            LastName = "Kowalski",
            IsActive = true,
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Create test reports for different quarters
        var reports = new List<Report>
        {
            new Report
            {
                Id = 1,
                ReportNumber = "RPT-2025-0001",
                FileName = "report_q1_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q1,
                Status = ReportStatus.Submitted,
                FileContent = new byte[] { 1, 2, 3 },
                SubmittedAt = new DateTime(2025, 4, 1),
                SubmittedByUserId = 1
            },
            new Report
            {
                Id = 2,
                ReportNumber = "RPT-2025-0002",
                FileName = "report_q2_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q2,
                Status = ReportStatus.ValidationSuccessful,
                FileContent = new byte[] { 4, 5, 6 },
                SubmittedAt = new DateTime(2025, 7, 1),
                SubmittedByUserId = 1
            },
            new Report
            {
                Id = 3,
                ReportNumber = "RPT-2025-0003",
                FileName = "report_q1_2025_v2.xlsx",
                ReportingPeriod = ReportingPeriod.Q1,
                Status = ReportStatus.Draft,
                FileContent = new byte[] { 7, 8, 9 },
                SubmittedAt = new DateTime(2025, 4, 15),
                SubmittedByUserId = 1
            },
            new Report
            {
                Id = 4,
                ReportNumber = "RPT-2025-0004",
                FileName = "report_q3_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q3,
                Status = ReportStatus.Submitted,
                FileContent = new byte[] { 10, 11, 12 },
                SubmittedAt = new DateTime(2025, 10, 1),
                SubmittedByUserId = 1
            }
        };

        _context.Reports.AddRange(reports);
        _context.SaveChanges();
    }

    #region GetReports Tests

    [Fact]
    public async Task GetReports_ReturnsAllReports_WhenNoFilterProvided()
    {
        // Act
        var result = await _controller.GetReports();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reports = okResult.Value.Should().BeAssignableTo<List<ReportDto>>().Subject;

        reports.Should().HaveCount(4);
        // Reports are ordered by SubmittedAt DESC, so most recent first
        reports.First().Id.Should().Be(4); // Submitted 2025-10-01
        reports.Last().Id.Should().Be(1);  // Submitted 2025-04-01
    }

    [Fact]
    public async Task GetReports_ReturnsFilteredReports_WhenReportingPeriodProvided()
    {
        // Act - Filter by Q1
        var result = await _controller.GetReports(reportingPeriod: "Q1");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reports = okResult.Value.Should().BeAssignableTo<List<ReportDto>>().Subject;

        reports.Should().HaveCount(2);
        reports.Should().OnlyContain(r => r.ReportingPeriod == "Q1");
    }

    [Fact]
    public async Task GetReports_ReturnsEmptyList_WhenNoReportsMatchFilter()
    {
        // Act - Filter by Q4 (no reports for this period)
        var result = await _controller.GetReports(reportingPeriod: "Q4");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reports = okResult.Value.Should().BeAssignableTo<List<ReportDto>>().Subject;

        reports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReports_IncludesEntityName_FromSubmittedByUser()
    {
        // Act
        var result = await _controller.GetReports();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reports = okResult.Value.Should().BeAssignableTo<List<ReportDto>>().Subject;

        reports.Should().AllSatisfy(r =>
        {
            r.EntityName.Should().Be("Jan Kowalski");
            r.ReportNumber.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task GetReports_HandlesException_ReturnsInternalServerError()
    {
        // Arrange - Create a new context that we'll dispose to cause exception
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var tempContext = new ApplicationDbContext(options);
        var tempService = new ReportsService(tempContext);
        var tempController = new ReportsController(tempService, _logger.Object);
        
        // Dispose context to cause exception
        await tempContext.DisposeAsync();

        // Act
        var result = await tempController.GetReports();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        
        var errorMessage = statusCodeResult.Value?.GetType().GetProperty("message")?.GetValue(statusCodeResult.Value);
        errorMessage.Should().NotBeNull();
    }

    #endregion

    #region GetReport Tests

    [Fact]
    public async Task GetReport_ReturnsReport_WhenReportExists()
    {
        // Act
        var result = await _controller.GetReport(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var report = okResult.Value.Should().BeOfType<ReportDto>().Subject;

        report.Id.Should().Be(1);
        report.ReportNumber.Should().Be("RPT-2025-0001");
        report.ReportingPeriod.Should().Be("Q1");
        report.EntityName.Should().Be("Jan Kowalski");
    }

    [Fact]
    public async Task GetReport_ReturnsNotFound_WhenReportDoesNotExist()
    {
        // Act
        var result = await _controller.GetReport(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        
        var errorMessage = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value);
        errorMessage.Should().NotBeNull();
        errorMessage.ToString().Should().Contain("999");
    }

    [Fact]
    public async Task GetReport_ReturnsCorrectData_ForDifferentQuarters()
    {
        // Test Q2 report
        var resultQ2 = await _controller.GetReport(2);
        var okResultQ2 = resultQ2.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reportQ2 = okResultQ2.Value.Should().BeOfType<ReportDto>().Subject;
        
        reportQ2.ReportingPeriod.Should().Be("Q2");
        reportQ2.ReportNumber.Should().Be("RPT-2025-0002");

        // Test Q3 report
        var resultQ3 = await _controller.GetReport(4);
        var okResultQ3 = resultQ3.Result.Should().BeOfType<OkObjectResult>().Subject;
        var reportQ3 = okResultQ3.Value.Should().BeOfType<ReportDto>().Subject;
        
        reportQ3.ReportingPeriod.Should().Be("Q3");
        reportQ3.ReportNumber.Should().Be("RPT-2025-0004");
    }

    [Fact]
    public async Task GetReport_HandlesException_ReturnsInternalServerError()
    {
        // Arrange - Create a new context that we'll dispose to cause exception
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var tempContext = new ApplicationDbContext(options);
        var tempService = new ReportsService(tempContext);
        var tempController = new ReportsController(tempService, _logger.Object);
        
        // Dispose context to cause exception
        await tempContext.DisposeAsync();

        // Act
        var result = await tempController.GetReport(1);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task ReportsWorkflow_GetAllThenGetSpecific_WorksCorrectly()
    {
        // Get all reports
        var allResult = await _controller.GetReports();
        var allReports = ((OkObjectResult)allResult.Result!).Value as List<ReportDto>;
        
        allReports.Should().NotBeNull();
        allReports!.Should().HaveCount(4);

        // Get specific report
        var specificId = allReports.First().Id;
        var specificResult = await _controller.GetReport(specificId);
        var specificReport = ((OkObjectResult)specificResult.Result!).Value as ReportDto;

        specificReport.Should().NotBeNull();
        specificReport!.Id.Should().Be(specificId);
    }

    [Fact]
    public async Task MultipleFilters_WorkIndependently()
    {
        // Filter by Q1
        var q1Result = await _controller.GetReports(reportingPeriod: "Q1");
        var q1Reports = ((OkObjectResult)q1Result.Result!).Value as List<ReportDto>;
        q1Reports.Should().HaveCount(2);

        // Filter by Q2
        var q2Result = await _controller.GetReports(reportingPeriod: "Q2");
        var q2Reports = ((OkObjectResult)q2Result.Result!).Value as List<ReportDto>;
        q2Reports.Should().HaveCount(1);

        // Filter by Q3
        var q3Result = await _controller.GetReports(reportingPeriod: "Q3");
        var q3Reports = ((OkObjectResult)q3Result.Result!).Value as List<ReportDto>;
        q3Reports.Should().HaveCount(1);

        // Total should match
        (q1Reports!.Count + q2Reports!.Count + q3Reports!.Count).Should().Be(4);
    }

    #endregion
}
