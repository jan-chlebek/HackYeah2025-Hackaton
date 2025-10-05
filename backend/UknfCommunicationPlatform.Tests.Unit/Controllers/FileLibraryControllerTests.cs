using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using UknfCommunicationPlatform.Api.Controllers.v1;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Controllers;

public class FileLibraryControllerTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly FileLibraryController _controller;
    private readonly Mock<ILogger<FileLibraryController>> _loggerMock;

    public FileLibraryControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<FileLibraryController>>();
        _controller = new FileLibraryController(_context, _loggerMock.Object);

        // Mock User context for authorization
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "InternalUser")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        var files = new List<FileLibrary>
        {
            new FileLibrary
            {
                Id = 1,
                Name = "Test Template",
                Description = "A test template file",
                FileName = "template.xlsx",
                FileSize = 1024,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Category = "Templates",
                FileContent = new byte[] { 1, 2, 3, 4 },
                UploadedByUserId = 1,
                UploadedAt = DateTime.UtcNow.AddDays(-1)
            },
            new FileLibrary
            {
                Id = 2,
                Name = "Legal Document",
                Description = "Legal requirements",
                FileName = "legal.pdf",
                FileSize = 2048,
                ContentType = "application/pdf",
                Category = "Legal",
                FileContent = new byte[] { 5, 6, 7, 8 },
                UploadedByUserId = 1,
                UploadedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        _context.FileLibraries.AddRange(files);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetFiles_ReturnsAllFiles_WhenNoFiltersProvided()
    {
        // Act
        var result = await _controller.GetFiles();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var files = okResult.Value.Should().BeAssignableTo<IEnumerable<FileLibraryResponse>>().Subject;
        files.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetFiles_ReturnsFilteredFiles_WhenCategoryProvided()
    {
        // Act
        var result = await _controller.GetFiles(category: "Templates");

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var files = okResult.Value.Should().BeAssignableTo<IEnumerable<FileLibraryResponse>>().Subject;
        files.Should().HaveCount(1);
        files.First().Category.Should().Be("Templates");
    }

    [Fact]
    public async Task GetFiles_ReturnsFilteredFiles_WhenSearchProvided()
    {
        // Act
        var result = await _controller.GetFiles(search: "legal");

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var files = okResult.Value.Should().BeAssignableTo<IEnumerable<FileLibraryResponse>>().Subject;
        files.Should().HaveCount(1);
        files.First().Name.Should().Contain("Legal");
    }

    [Fact]
    public async Task GetFileById_ReturnsFile_WhenFileExists()
    {
        // Act
        var result = await _controller.GetFileById(1);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var file = okResult.Value.Should().BeOfType<FileLibraryResponse>().Subject;
        file.Id.Should().Be(1);
        file.Name.Should().Be("Test Template");
    }

    [Fact]
    public async Task GetFileById_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Act
        var result = await _controller.GetFileById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DownloadFile_ReturnsFileContent_WhenFileExists()
    {
        // Act
        var result = await _controller.DownloadFile(1);

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.FileContents.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
        fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileResult.FileDownloadName.Should().Be("template.xlsx");
    }

    [Fact]
    public async Task DownloadFile_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Act
        var result = await _controller.DownloadFile(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UploadFile_CreatesFile_WhenRequestIsValid()
    {
        // Arrange
        var fileContent = new byte[] { 10, 20, 30, 40 };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("newfile.pdf");
        fileMock.Setup(f => f.Length).Returns(fileContent.Length);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => stream.Write(fileContent, 0, fileContent.Length))
            .Returns(Task.CompletedTask);

        var request = new FileLibraryUploadRequest
        {
            Name = "New File",
            Description = "Test description",
            Category = "Test",
            File = fileMock.Object
        };

        // Act
        var result = await _controller.UploadFile(request);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<FileLibraryResponse>().Subject;
        response.Name.Should().Be("New File");
        response.FileName.Should().Be("newfile.pdf");
        response.Category.Should().Be("Test");

        // Verify file was saved to database
        var savedFile = await _context.FileLibraries.FindAsync(response.Id);
        savedFile.Should().NotBeNull();
        savedFile!.FileContent.Should().BeEquivalentTo(fileContent);
    }

    [Fact]
    public async Task UploadFile_ReturnsBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        var request = new FileLibraryUploadRequest
        {
            Name = "Empty File",
            Category = "Test",
            File = fileMock.Object
        };

        // Act
        var result = await _controller.UploadFile(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadFile_ReturnsBadRequest_WhenFileTypeNotAllowed()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.ContentType).Returns("application/x-executable");

        var request = new FileLibraryUploadRequest
        {
            Name = "Malicious File",
            Category = "Test",
            File = fileMock.Object
        };

        // Act
        var result = await _controller.UploadFile(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateMetadata_UpdatesFileMetadata_WhenFileExists()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = "Updated Template Name",
            Description = "Updated description",
            Category = "Updated Category"
        };

        // Act
        var result = await _controller.UpdateMetadata(1, request);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<FileLibraryResponse>().Subject;
        response.Name.Should().Be("Updated Template Name");
        response.Description.Should().Be("Updated description");
        response.Category.Should().Be("Updated Category");

        // Verify file content was not changed
        var file = await _context.FileLibraries.FindAsync(1L);
        file!.FileContent.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }

    [Fact]
    public async Task UpdateMetadata_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = "New Name"
        };

        // Act
        var result = await _controller.UpdateMetadata(999, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ReplaceFile_ReplacesFileContent_WhenFileExists()
    {
        // Arrange
        var newFileContent = new byte[] { 100, 101, 102, 103 };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("updated.xlsx");
        fileMock.Setup(f => f.Length).Returns(newFileContent.Length);
        fileMock.Setup(f => f.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => stream.Write(newFileContent, 0, newFileContent.Length))
            .Returns(Task.CompletedTask);

        var request = new FileLibraryReplaceRequest
        {
            Name = "Replaced Template",
            File = fileMock.Object
        };

        // Act
        var result = await _controller.ReplaceFile(1, request);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<FileLibraryResponse>().Subject;
        response.Name.Should().Be("Replaced Template");
        response.FileName.Should().Be("updated.xlsx");

        // Verify file content was replaced
        var file = await _context.FileLibraries.FindAsync(1L);
        file!.FileContent.Should().BeEquivalentTo(newFileContent);
    }

    [Fact]
    public async Task ReplaceFile_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        var request = new FileLibraryReplaceRequest
        {
            File = fileMock.Object
        };

        // Act
        var result = await _controller.ReplaceFile(999, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteFile_RemovesFile_WhenFileExists()
    {
        // Act
        var result = await _controller.DeleteFile(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify file was deleted
        var file = await _context.FileLibraries.FindAsync(1L);
        file.Should().BeNull();
    }

    [Fact]
    public async Task DeleteFile_ReturnsNotFound_WhenFileDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteFile(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCategories_ReturnsDistinctCategories()
    {
        // Act
        var result = await _controller.GetCategories();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
        categories.Should().HaveCount(2);
        categories.Should().Contain("Templates");
        categories.Should().Contain("Legal");
    }
}
