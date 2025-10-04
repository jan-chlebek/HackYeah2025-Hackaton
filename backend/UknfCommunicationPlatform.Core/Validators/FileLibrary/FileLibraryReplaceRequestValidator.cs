using FluentValidation;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;

namespace UknfCommunicationPlatform.Core.Validators.FileLibrary;

/// <summary>
/// Validator for FileLibraryReplaceRequest
/// </summary>
public class FileLibraryReplaceRequestValidator : AbstractValidator<FileLibraryReplaceRequest>
{
    private const long MaxFileSizeBytes = 104_857_600; // 100 MB
    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/csv",
        "text/plain",
        "audio/mpeg",
        "application/zip",
        "application/x-zip-compressed"
    };

    public FileLibraryReplaceRequestValidator()
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

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required")
            .Must(file => file != null && file.Length > 0).WithMessage("File cannot be empty")
            .Must(file => file == null || file.Length <= MaxFileSizeBytes)
                .WithMessage($"File size cannot exceed {MaxFileSizeBytes / 1_048_576} MB")
            .Must(file => file == null || AllowedContentTypes.Contains(file.ContentType))
                .WithMessage($"File type must be one of: {string.Join(", ", AllowedContentTypes)}");
    }
}
