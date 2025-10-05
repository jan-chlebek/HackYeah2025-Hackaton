using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UknfCommunicationPlatform.Core.DTOs.Requests;
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
    private readonly Mock<ILogger<ReportsService>> _loggerMock;

    public ReportsServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<ReportsService>>();
        _sut = new ReportsService(_context, _loggerMock.Object);
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
        result.FileName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SubmitReportAsync_WithValidXlsxFile_ShouldCreateReport()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockXlsxFile("test_report.xlsx", 1024);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act
        var result = await _sut.SubmitReportAsync(user.Id, file.Object, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.ReportNumber.Should().MatchRegex(@"RPT-\d{4}-\d{4}");
        result.ReportingPeriod.Should().Be("Q1");
        result.FileName.Should().Be("test_report.xlsx");

        var savedReport = await _context.Reports.FindAsync(result.Id);
        savedReport.Should().NotBeNull();
        savedReport!.FileName.Should().Be("test_report.xlsx");
        savedReport.FileContent.Should().NotBeEmpty();
        savedReport.Status.Should().Be(ReportStatus.Draft);
    }

    [Fact]
    public async Task SubmitReportAsync_WithNullFile_ShouldThrowArgumentException()
    {
        // Arrange
        var user = await SeedUserAsync();
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.SubmitReportAsync(user.Id, null!, request));
    }

    [Fact]
    public async Task SubmitReportAsync_WithEmptyFile_ShouldThrowArgumentException()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockXlsxFile("empty.xlsx", 0);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.SubmitReportAsync(user.Id, file.Object, request));
        exception.Message.Should().Contain("required");
    }

    [Fact]
    public async Task SubmitReportAsync_WithNonXlsxFile_ShouldThrowArgumentException()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockFile("document.pdf", "application/pdf", 1024);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.SubmitReportAsync(user.Id, file.Object, request));
        exception.Message.Should().Contain(".xlsx");
    }

    [Fact]
    public async Task SubmitReportAsync_WithInvalidMimeType_ShouldThrowArgumentException()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockFile("fake.xlsx", "text/plain", 1024);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.SubmitReportAsync(user.Id, file.Object, request));
        exception.Message.Should().Contain("content type");
    }

    [Fact]
    public async Task SubmitReportAsync_WithFileTooLarge_ShouldThrowArgumentException()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockXlsxFile("huge.xlsx", 51 * 1024 * 1024); // 51MB
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.SubmitReportAsync(user.Id, file.Object, request));
        exception.Message.Should().Contain("exceeds maximum");
    }

    [Fact]
    public async Task SubmitReportAsync_ShouldGenerateUniqueReportNumber()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file1 = CreateMockXlsxFile("report1.xlsx", 1024);
        var file2 = CreateMockXlsxFile("report2.xlsx", 1024);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q1 };

        // Act
        var result1 = await _sut.SubmitReportAsync(user.Id, file1.Object, request);
        var result2 = await _sut.SubmitReportAsync(user.Id, file2.Object, request);

        // Assert
        result1.ReportNumber.Should().NotBe(result2.ReportNumber);
        result1.ReportNumber.Should().MatchRegex(@"RPT-\d{4}-\d{4}");
        result2.ReportNumber.Should().MatchRegex(@"RPT-\d{4}-\d{4}");
    }

    [Fact]
    public async Task SubmitReportAsync_ShouldStoreFileContent()
    {
        // Arrange
        var user = await SeedUserAsync();
        var file = CreateMockXlsxFile("test.xlsx", 2048);
        var request = new SubmitReportRequest { ReportingPeriod = ReportingPeriod.Q2 };

        // Act
        var result = await _sut.SubmitReportAsync(user.Id, file.Object, request);

        // Assert
        var savedReport = await _context.Reports.FindAsync(result.Id);
        savedReport!.FileContent.Should().HaveCount(2048);
        savedReport.FileName.Should().Be("test.xlsx");
    }

    [Fact]
    public async Task GetReportFileAsync_ExistingReport_ShouldReturnFileData()
    {
        // Arrange
        var report = await SeedSingleReportAsync();

        // Act
        var result = await _sut.GetReportFileAsync(report.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Value.FileContent.Should().Equal(report.FileContent);
        result.Value.FileName.Should().Be(report.FileName);
    }

    [Fact]
    public async Task GetReportFileAsync_NonExistingReport_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetReportFileAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetReportFileAsync_ReportWithEmptyFile_ShouldReturnNull()
    {
        // Arrange
        var user = await SeedUserAsync();
        var report = new Report
        {
            ReportNumber = "RPT-2025-0001",
            FileName = "empty.xlsx",
            FileContent = Array.Empty<byte>(),
            ReportingPeriod = ReportingPeriod.Q1,
            Status = ReportStatus.Draft,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetReportFileAsync(report.Id);

        // Assert
        result.Should().BeNull();
    }

    // Helper methods for creating mock files

    private Mock<IFormFile> CreateMockXlsxFile(string fileName, long length)
    {
        return CreateMockFile(
            fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            length);
    }

    private Mock<IFormFile> CreateMockFile(string fileName, string contentType, long length)
    {
        var fileMock = new Mock<IFormFile>();
        var content = new byte[length];
        var ms = new MemoryStream(content);

        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream stream, CancellationToken token) =>
            {
                ms.Position = 0;
                return ms.CopyToAsync(stream, token);
            });

        return fileMock;
    }

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
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
