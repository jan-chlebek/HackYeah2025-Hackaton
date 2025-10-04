using FluentValidation;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;

namespace UknfCommunicationPlatform.Core.Validators.FileLibrary;

/// <summary>
/// Validator for FileLibraryUpdateMetadataRequest
/// </summary>
public class FileLibraryUpdateMetadataRequestValidator : AbstractValidator<FileLibraryUpdateMetadataRequest>
{
    public FileLibraryUpdateMetadataRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(500).WithMessage("File name cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .MaximumLength(250).WithMessage("Category cannot exceed 250 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));

        // At least one field must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Name) ||
                       x.Description != null ||
                       !string.IsNullOrEmpty(x.Category))
            .WithMessage("At least one field (Name, Description, or Category) must be provided");
    }
}
