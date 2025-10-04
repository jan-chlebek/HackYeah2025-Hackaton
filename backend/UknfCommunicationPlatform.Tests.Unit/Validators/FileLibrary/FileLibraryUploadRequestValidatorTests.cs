using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;
using UknfCommunicationPlatform.Core.Validators.FileLibrary;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Validators.FileLibrary;

public class FileLibraryUploadRequestValidatorTests
{
    private readonly FileLibraryUploadRequestValidator _validator;

    public FileLibraryUploadRequestValidatorTests()
    {
        _validator = new FileLibraryUploadRequestValidator();
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var fileMock = CreateValidFileMock();
        var request = new FileLibraryUploadRequest
        {
            Name = "",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("File name is required");
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaxLength()
    {
        // Arrange
        var fileMock = CreateValidFileMock();
        var request = new FileLibraryUploadRequest
        {
            Name = new string('a', 501),
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("File name cannot exceed 500 characters");
    }

    [Fact]
    public void Should_HaveError_When_DescriptionExceedsMaxLength()
    {
        // Arrange
        var fileMock = CreateValidFileMock();
        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Description = new string('a', 2001),
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void Should_HaveError_When_CategoryIsEmpty()
    {
        // Arrange
        var fileMock = CreateValidFileMock();
        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Category is required");
    }

    [Fact]
    public void Should_HaveError_When_FileIsNull()
    {
        // Arrange
        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "Templates",
            File = null!
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Should_HaveError_When_FileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("File cannot be empty");
    }

    [Fact]
    public void Should_HaveError_When_FileSizeExceedsLimit()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(105_000_000); // 105 MB
        fileMock.Setup(f => f.ContentType).Returns("application/pdf");

        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("File size cannot exceed 100 MB");
    }

    [Fact]
    public void Should_HaveError_When_FileTypeNotAllowed()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.ContentType).Returns("application/x-executable");

        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
            .Should().Contain(err => err.ErrorMessage.Contains("File type must be one of"));
    }

    [Fact]
    public void Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        var fileMock = CreateValidFileMock();
        var request = new FileLibraryUploadRequest
        {
            Name = "Valid File Name",
            Description = "Valid description",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("application/zip")]
    [InlineData("text/csv")]
    public void Should_NotHaveError_When_FileTypeIsAllowed(string contentType)
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        fileMock.Setup(f => f.ContentType).Returns(contentType);

        var request = new FileLibraryUploadRequest
        {
            Name = "Valid Name",
            Category = "Templates",
            File = fileMock.Object
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    private Mock<IFormFile> CreateValidFileMock()
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.Length).Returns(1024);
        mock.Setup(f => f.ContentType).Returns("application/pdf");
        return mock;
    }
}
