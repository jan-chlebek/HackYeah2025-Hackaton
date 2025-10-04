using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.DTOs.FileLibrary;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Manages file library operations (upload, download, update, delete)
/// </summary>
[ApiController]
[Route("api/v1/library/files")]
[Produces("application/json")]
public class FileLibraryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FileLibraryController> _logger;
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

    public FileLibraryController(ApplicationDbContext context, ILogger<FileLibraryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all files from the library with optional filtering
    /// </summary>
    /// <param name="category">Filter by category</param>
    /// <param name="search">Search by name or filename</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>List of files</returns>
    /// <response code="200">Returns the list of files</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FileLibraryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FileLibraryResponse>>> GetFiles(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.FileLibraries
            .Include(f => f.UploadedBy)
            .Include(f => f.Permissions)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(f => f.Category == category);

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(f => 
                f.Name.ToLower().Contains(searchLower) || 
                f.FileName.ToLower().Contains(searchLower) ||
                (f.Description != null && f.Description.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync();

        var files = await query
            .OrderByDescending(f => f.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FileLibraryResponse
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FileName = f.FileName,
                FileSize = f.FileSize,
                FileSizeFormatted = FormatFileSize(f.FileSize),
                ContentType = f.ContentType,
                Category = f.Category,
                UploadedAt = f.UploadedAt,
                UploadedByUserId = f.UploadedByUserId,
                UploadedByName = $"{f.UploadedBy.FirstName} {f.UploadedBy.LastName}",
                UploadedByEmail = f.UploadedBy.Email,
                PermissionCount = f.Permissions.Count
            })
            .ToListAsync();

        // Add pagination header (if Response is available - might be null in unit tests)
        if (Response != null)
        {
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(new
            {
                currentPage = page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                hasPrevious = page > 1,
                hasNext = page < (int)Math.Ceiling(totalCount / (double)pageSize)
            }));
        }

        return Ok(files);
    }

    /// <summary>
    /// Get a specific file's metadata by ID
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>File metadata</returns>
    /// <response code="200">Returns the file metadata</response>
    /// <response code="404">File not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FileLibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FileLibraryResponse>> GetFileById(long id)
    {
        var file = await _context.FileLibraries
            .Include(f => f.UploadedBy)
            .Include(f => f.Permissions)
            .Where(f => f.Id == id)
            .Select(f => new FileLibraryResponse
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FileName = f.FileName,
                FileSize = f.FileSize,
                FileSizeFormatted = FormatFileSize(f.FileSize),
                ContentType = f.ContentType,
                Category = f.Category,
                UploadedAt = f.UploadedAt,
                UploadedByUserId = f.UploadedByUserId,
                UploadedByName = $"{f.UploadedBy.FirstName} {f.UploadedBy.LastName}",
                UploadedByEmail = f.UploadedBy.Email,
                PermissionCount = f.Permissions.Count
            })
            .FirstOrDefaultAsync();

        if (file == null)
        {
            return NotFound(new { message = $"File with ID {id} not found" });
        }

        return Ok(file);
    }

    /// <summary>
    /// Download a file from the library
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>File content</returns>
    /// <response code="200">Returns the file</response>
    /// <response code="404">File not found</response>
    [HttpGet("{id}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(long id)
    {
        var file = await _context.FileLibraries
            .Where(f => f.Id == id)
            .FirstOrDefaultAsync();

        if (file == null)
        {
            return NotFound(new { message = $"File with ID {id} not found" });
        }

        _logger.LogInformation("File {FileId} ({FileName}) downloaded", id, file.FileName);

        return File(file.FileContent, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Upload a new file to the library
    /// </summary>
    /// <param name="request">Upload request with file and metadata</param>
    /// <returns>Created file metadata</returns>
    /// <response code="201">File uploaded successfully</response>
    /// <response code="400">Invalid request (file too large, unsupported format, etc.)</response>
    [HttpPost]
    [ProducesResponseType(typeof(FileLibraryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<FileLibraryResponse>> UploadFile([FromForm] FileLibraryUploadRequest request)
    {
        // Validate file
        if (request.File.Length == 0)
        {
            return BadRequest(new { message = "File is empty" });
        }

        if (request.File.Length > MaxFileSizeBytes)
        {
            return BadRequest(new { message = $"File size exceeds maximum allowed size of {MaxFileSizeBytes / 1_048_576} MB" });
        }

        if (!AllowedContentTypes.Contains(request.File.ContentType))
        {
            return BadRequest(new { message = $"File type '{request.File.ContentType}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}" });
        }

        // Read file content
        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await request.File.CopyToAsync(memoryStream);
            fileContent = memoryStream.ToArray();
        }

        // TODO: Authorization - get current user ID from JWT claims
        // For now, using a default user ID (this will be replaced with actual auth)
        long currentUserId = 2; // jan.kowalski@uknf.gov.pl (UKNF staff)
        _logger.LogWarning("Authorization disabled - using default user ID {UserId}", currentUserId);

        // Create file library entry
        var fileLibrary = new FileLibrary
        {
            Name = request.Name,
            Description = request.Description,
            FileName = request.File.FileName,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            Category = request.Category,
            FileContent = fileContent,
            UploadedByUserId = currentUserId,
            UploadedAt = DateTime.UtcNow
        };

        _context.FileLibraries.Add(fileLibrary);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File {FileName} uploaded to library with ID {FileId}", fileLibrary.FileName, fileLibrary.Id);

        // Fetch the created file with navigation properties
        var createdFile = await _context.FileLibraries
            .Include(f => f.UploadedBy)
            .Include(f => f.Permissions)
            .Where(f => f.Id == fileLibrary.Id)
            .Select(f => new FileLibraryResponse
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                FileName = f.FileName,
                FileSize = f.FileSize,
                FileSizeFormatted = FormatFileSize(f.FileSize),
                ContentType = f.ContentType,
                Category = f.Category,
                UploadedAt = f.UploadedAt,
                UploadedByUserId = f.UploadedByUserId,
                UploadedByName = $"{f.UploadedBy.FirstName} {f.UploadedBy.LastName}",
                UploadedByEmail = f.UploadedBy.Email,
                PermissionCount = f.Permissions.Count
            })
            .FirstOrDefaultAsync();

        if (createdFile == null)
        {
            // Fallback for when navigation properties aren't loaded (e.g., in-memory database)
            return CreatedAtAction(nameof(GetFileById), new { id = fileLibrary.Id }, new FileLibraryResponse
            {
                Id = fileLibrary.Id,
                Name = fileLibrary.Name,
                Description = fileLibrary.Description,
                FileName = fileLibrary.FileName,
                FileSize = fileLibrary.FileSize,
                FileSizeFormatted = FormatFileSize(fileLibrary.FileSize),
                ContentType = fileLibrary.ContentType,
                Category = fileLibrary.Category,
                UploadedAt = fileLibrary.UploadedAt,
                UploadedByUserId = fileLibrary.UploadedByUserId,
                UploadedByName = "Unknown",
                UploadedByEmail = "",
                PermissionCount = 0
            });
        }

        return CreatedAtAction(nameof(GetFileById), new { id = fileLibrary.Id }, createdFile);
    }

    /// <summary>
    /// Update file metadata without replacing the file content
    /// </summary>
    /// <param name="id">File ID</param>
    /// <param name="request">Metadata update request</param>
    /// <returns>Updated file metadata</returns>
    /// <response code="200">Metadata updated successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="400">Invalid request</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(FileLibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FileLibraryResponse>> UpdateMetadata(long id, [FromBody] FileLibraryUpdateMetadataRequest request)
    {
        var file = await _context.FileLibraries
            .Include(f => f.UploadedBy)
            .Include(f => f.Permissions)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (file == null)
        {
            return NotFound(new { message = $"File with ID {id} not found" });
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Name))
            file.Name = request.Name;

        if (request.Description != null)
            file.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            file.Category = request.Category;

        await _context.SaveChangesAsync();

        _logger.LogInformation("File {FileId} metadata updated", id);

        var response = new FileLibraryResponse
        {
            Id = file.Id,
            Name = file.Name,
            Description = file.Description,
            FileName = file.FileName,
            FileSize = file.FileSize,
            FileSizeFormatted = FormatFileSize(file.FileSize),
            ContentType = file.ContentType,
            Category = file.Category,
            UploadedAt = file.UploadedAt,
            UploadedByUserId = file.UploadedByUserId,
            UploadedByName = $"{file.UploadedBy.FirstName} {file.UploadedBy.LastName}",
            UploadedByEmail = file.UploadedBy.Email,
            PermissionCount = file.Permissions.Count
        };

        return Ok(response);
    }

    /// <summary>
    /// Replace an existing file (update content and optionally metadata)
    /// </summary>
    /// <param name="id">File ID to replace</param>
    /// <param name="request">Replace request with new file and optional metadata</param>
    /// <returns>Updated file metadata</returns>
    /// <response code="200">File replaced successfully</response>
    /// <response code="404">File not found</response>
    /// <response code="400">Invalid request (file too large, unsupported format, etc.)</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FileLibraryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<FileLibraryResponse>> ReplaceFile(long id, [FromForm] FileLibraryReplaceRequest request)
    {
        var file = await _context.FileLibraries
            .Include(f => f.UploadedBy)
            .Include(f => f.Permissions)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (file == null)
        {
            return NotFound(new { message = $"File with ID {id} not found" });
        }

        // Validate new file
        if (request.File.Length == 0)
        {
            return BadRequest(new { message = "File is empty" });
        }

        if (request.File.Length > MaxFileSizeBytes)
        {
            return BadRequest(new { message = $"File size exceeds maximum allowed size of {MaxFileSizeBytes / 1_048_576} MB" });
        }

        if (!AllowedContentTypes.Contains(request.File.ContentType))
        {
            return BadRequest(new { message = $"File type '{request.File.ContentType}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}" });
        }

        // Read new file content
        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await request.File.CopyToAsync(memoryStream);
            fileContent = memoryStream.ToArray();
        }

        // Update file content and metadata
        file.FileName = request.File.FileName;
        file.FileSize = request.File.Length;
        file.ContentType = request.File.ContentType;
        file.FileContent = fileContent;

        // Update optional metadata if provided
        if (!string.IsNullOrEmpty(request.Name))
            file.Name = request.Name;

        if (request.Description != null)
            file.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            file.Category = request.Category;

        // Note: UploadedAt and UploadedBy remain unchanged to preserve original upload info
        // Consider adding a separate "LastModifiedAt" and "LastModifiedBy" if needed

        await _context.SaveChangesAsync();

        _logger.LogInformation("File {FileId} replaced with new content", id);

        var response = new FileLibraryResponse
        {
            Id = file.Id,
            Name = file.Name,
            Description = file.Description,
            FileName = file.FileName,
            FileSize = file.FileSize,
            FileSizeFormatted = FormatFileSize(file.FileSize),
            ContentType = file.ContentType,
            Category = file.Category,
            UploadedAt = file.UploadedAt,
            UploadedByUserId = file.UploadedByUserId,
            UploadedByName = $"{file.UploadedBy.FirstName} {file.UploadedBy.LastName}",
            UploadedByEmail = file.UploadedBy.Email,
            PermissionCount = file.Permissions.Count
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a file from the library
    /// </summary>
    /// <param name="id">File ID</param>
    /// <returns>No content</returns>
    /// <response code="204">File deleted successfully</response>
    /// <response code="404">File not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile(long id)
    {
        var file = await _context.FileLibraries
            .Include(f => f.Permissions)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (file == null)
        {
            return NotFound(new { message = $"File with ID {id} not found" });
        }

        // Delete associated permissions first (cascade should handle this, but being explicit)
        _context.FileLibraryPermissions.RemoveRange(file.Permissions);
        
        // Delete the file
        _context.FileLibraries.Remove(file);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File {FileId} ({FileName}) deleted from library", id, file.FileName);

        return NoContent();
    }

    /// <summary>
    /// Get list of available categories
    /// </summary>
    /// <returns>List of categories</returns>
    /// <response code="200">Returns the list of categories</response>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _context.FileLibraries
            .Select(f => f.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Format file size in human-readable format
    /// </summary>
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
