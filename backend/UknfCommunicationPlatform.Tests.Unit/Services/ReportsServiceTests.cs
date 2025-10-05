using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

/// <summary>
/// Unit tests for ReportsService
/// </summary>
public class ReportsServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReportsService _sut;

    public ReportsServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new ReportsService(_context);
    }

    [Fact]
    public async Task GetReportsAsync_WithoutFilters_ShouldReturnAllReports()
    {
        // Arrange
        await SeedReportsAsync();

        // Act
        var reports = await _sut.GetReportsAsync();

        // Assert
        reports.Should().HaveCount(4);
        reports.Should().BeInDescendingOrder(r => r.Id); // Ordered by SubmittedAt descending
    }

    [Fact]
    public async Task GetReportsAsync_WithReportingPeriodFilter_ShouldReturnFilteredReports()
    {
        // Arrange
        await SeedReportsAsync();

        // Act
        var reports = await _sut.GetReportsAsync("Q1");

        // Assert
        reports.Should().HaveCount(1);
        reports.First().ReportingPeriod.Should().Be("Q1");
    }

    [Fact]
    public async Task GetReportsAsync_WithNonMatchingFilter_ShouldReturnEmpty()
    {
        // Arrange
        await SeedReportsAsync();

        // Act
        var reports = await _sut.GetReportsAsync("NonExistentPeriod");

        // Assert
        reports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReportsAsync_ShouldIncludeEntityNameFromUser()
    {
        // Arrange
        await SeedReportsAsync();

        // Act
        var reports = await _sut.GetReportsAsync();

        // Assert
        reports.Should().AllSatisfy(r =>
        {
            r.EntityName.Should().NotBeNullOrEmpty();
            r.EntityName.Should().Contain(" "); // Should contain FirstName + " " + LastName
        });
    }

    [Fact]
    public async Task GetReportByIdAsync_ExistingReport_ShouldReturnReport()
    {
        // Arrange
        var report = await SeedSingleReportAsync();

        // Act
        var result = await _sut.GetReportByIdAsync(report.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(report.Id);
        result.ReportNumber.Should().Be(report.ReportNumber);
        result.ReportingPeriod.Should().Be(report.ReportingPeriod.ToString());
        result.EntityName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetReportByIdAsync_NonExistingReport_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetReportByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetReportsAsync_NoReportsInDatabase_ShouldReturnEmptyList()
    {
        // Act
        var reports = await _sut.GetReportsAsync();

        // Assert
        reports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReportsAsync_WithWhitespaceFilter_ShouldReturnAllReports()
    {
        // Arrange
        await SeedReportsAsync();

        // Act
        var reports = await _sut.GetReportsAsync("   ");

        // Assert
        reports.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetReportsAsync_ShouldPopulateAllDtoFields()
    {
        // Arrange
        var report = await SeedSingleReportAsync();

        // Act
        var reports = await _sut.GetReportsAsync();
        var result = reports.First();

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.ReportNumber.Should().NotBeNullOrEmpty();
        result.ReportingPeriod.Should().NotBeNullOrEmpty();
        result.EntityName.Should().NotBeNullOrEmpty();
    }

    // Helper methods for seeding test data

    private async Task<Report> SeedSingleReportAsync()
    {
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var report = new Report
        {
            ReportNumber = "RPT-2025-0001",
            FileName = "test_report.xlsx",
            ReportingPeriod = ReportingPeriod.Q1,
            Status = ReportStatus.Submitted,
            FileContent = new byte[] { 0x50, 0x4B },
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return report;
    }

    private async Task SeedReportsAsync()
    {
        var users = new List<User>
        {
            new User
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@test.com",
                PasswordHash = "hash1",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob@test.com",
                PasswordHash = "hash2",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var reports = new List<Report>
        {
            new Report
            {
                ReportNumber = "RPT-2025-0001",
                FileName = "report_q1_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q1,
                Status = ReportStatus.Submitted,
                FileContent = new byte[] { 0x50, 0x4B },
                SubmittedAt = DateTime.UtcNow.AddDays(-30),
                SubmittedByUserId = users[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Report
            {
                ReportNumber = "RPT-2025-0002",
                FileName = "report_q2_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q2,
                Status = ReportStatus.ValidationSuccessful,
                FileContent = new byte[] { 0x50, 0x4B },
                SubmittedAt = DateTime.UtcNow.AddDays(-20),
                SubmittedByUserId = users[1].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Report
            {
                ReportNumber = "RPT-2025-0003",
                FileName = "report_q3_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q3,
                Status = ReportStatus.Draft,
                FileContent = new byte[] { 0x50, 0x4B },
                SubmittedAt = DateTime.UtcNow.AddDays(-10),
                SubmittedByUserId = users[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Report
            {
                ReportNumber = "RPT-2025-0004",
                FileName = "report_q4_2025.xlsx",
                ReportingPeriod = ReportingPeriod.Q4,
                Status = ReportStatus.QuestionedByUKNF,
                FileContent = new byte[] { 0x50, 0x4B },
                SubmittedAt = DateTime.UtcNow.AddDays(-5),
                SubmittedByUserId = users[1].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        _context.Reports.AddRange(reports);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
