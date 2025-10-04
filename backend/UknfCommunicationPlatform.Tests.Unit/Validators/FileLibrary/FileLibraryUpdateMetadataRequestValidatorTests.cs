using FluentValidation.TestHelper;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;
using UknfCommunicationPlatform.Core.Validators.FileLibrary;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Validators.FileLibrary;

public class FileLibraryUpdateMetadataRequestValidatorTests
{
    private readonly FileLibraryUpdateMetadataRequestValidator _validator;

    public FileLibraryUpdateMetadataRequestValidatorTests()
    {
        _validator = new FileLibraryUpdateMetadataRequestValidator();
    }

    [Fact]
    public void Should_HaveError_When_AllFieldsAreNull()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = null,
            Description = null,
            Category = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one field (Name, Description, or Category) must be provided");
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaxLength()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = new string('a', 501)
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
        var request = new FileLibraryUpdateMetadataRequest
        {
            Description = new string('a', 2001)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 2000 characters");
    }

    [Fact]
    public void Should_HaveError_When_CategoryExceedsMaxLength()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Category = new string('a', 251)
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Category cannot exceed 250 characters");
    }

    [Fact]
    public void Should_NotHaveError_When_OnlyNameIsProvided()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = "New Name"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_OnlyDescriptionIsProvided()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Description = "New Description"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_OnlyCategoryIsProvided()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Category = "New Category"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_AllFieldsAreProvided()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Name = "New Name",
            Description = "New Description",
            Category = "New Category"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_DescriptionIsEmptyString()
    {
        // Arrange
        var request = new FileLibraryUpdateMetadataRequest
        {
            Description = ""
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
