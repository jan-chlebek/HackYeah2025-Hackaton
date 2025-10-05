using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Api.Controllers;

/// <summary>
/// Controller for FAQ operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FaqController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FaqController> _logger;

    public FaqController(ApplicationDbContext context, ILogger<FaqController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all FAQ questions with optional search and pagination
    /// </summary>
    /// <param name="search">Optional search query for question or answer text</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of FAQ questions</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<FaqQuestionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = _context.FaqQuestions.AsQueryable();

        // Apply search filter (case-insensitive)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(f => 
                f.Question.ToLower().Contains(searchLower) || 
                f.Answer.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FaqQuestionDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer
            })
            .ToListAsync();

        var result = new PagedResult<FaqQuestionDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        _logger.LogInformation("Retrieved {Count} FAQ questions (page {Page}/{TotalPages})", items.Count, page, result.TotalPages);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific FAQ question by ID
    /// </summary>
    /// <param name="id">FAQ question ID</param>
    /// <returns>FAQ question details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var faq = await _context.FaqQuestions
            .Where(f => f.Id == id)
            .Select(f => new FaqQuestionDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer
            })
            .FirstOrDefaultAsync();

        if (faq == null)
        {
            _logger.LogWarning("FAQ question with ID {Id} not found", id);
            return NotFound(new { message = $"FAQ question with ID {id} not found" });
        }

        _logger.LogInformation("Retrieved FAQ question with ID {Id}", id);
        return Ok(faq);
    }

    /// <summary>
    /// Submit a new FAQ question (without answer)
    /// </summary>
    /// <remarks>
    /// Allows users to submit questions that will be answered later by administrators.
    /// The answer field will be empty until an admin provides one.
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "question": "Jak mogę zaktualizować dane mojej firmy?"
    /// }
    /// ```
    /// 
    /// **Use Case:**
    /// - External users submit questions they need answered
    /// - System stores question with empty answer
    /// - UKNF administrators can later update the FAQ with an answer via PUT endpoint
    /// </remarks>
    /// <param name="request">Question submission request</param>
    /// <returns>Created FAQ question (with empty answer)</returns>
    /// <response code="201">Question submitted successfully</response>
    /// <response code="400">Invalid request - question cannot be empty</response>
    [HttpPost("submit-question")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitQuestion([FromBody] SubmitQuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Question cannot be empty.",
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext?.Request?.Path.Value ?? "/api/v1/faq/submit-question"
            });
        }

        var faq = new FaqQuestion
        {
            Question = request.Question.Trim(),
            Answer = string.Empty // Empty answer - to be filled later by admin
        };

        _context.FaqQuestions.Add(faq);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Question submitted with ID {FaqId}: {Question}", 
            faq.Id, faq.Question);

        var dto = new FaqQuestionDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer
        };

        return CreatedAtAction(nameof(GetById), new { id = faq.Id }, dto);
    }

    /// <summary>
    /// Create a new FAQ question
    /// </summary>
    /// <param name="request">FAQ question data</param>
    /// <returns>Created FAQ question</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateFaqQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var faq = new FaqQuestion
        {
            Question = request.Question.Trim(),
            Answer = request.Answer.Trim()
        };

        _context.FaqQuestions.Add(faq);
        await _context.SaveChangesAsync();

        var dto = new FaqQuestionDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer
        };

        _logger.LogInformation("Created FAQ question with ID {Id}", faq.Id);
        return CreatedAtAction(nameof(GetById), new { id = faq.Id }, dto);
    }

    /// <summary>
    /// Update an existing FAQ question
    /// </summary>
    /// <param name="id">FAQ question ID</param>
    /// <param name="request">Updated FAQ question data</param>
    /// <returns>Updated FAQ question</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(FaqQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateFaqQuestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var faq = await _context.FaqQuestions.FindAsync(id);
        if (faq == null)
        {
            _logger.LogWarning("FAQ question with ID {Id} not found", id);
            return NotFound(new { message = $"FAQ question with ID {id} not found" });
        }

        faq.Question = request.Question.Trim();
        faq.Answer = request.Answer.Trim();

        _context.FaqQuestions.Update(faq);
        await _context.SaveChangesAsync();

        var dto = new FaqQuestionDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer
        };

        _logger.LogInformation("Updated FAQ question with ID {Id}", faq.Id);
        return Ok(dto);
    }

    /// <summary>
    /// Delete an FAQ question
    /// </summary>
    /// <param name="id">FAQ question ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var faq = await _context.FaqQuestions.FindAsync(id);
        if (faq == null)
        {
            _logger.LogWarning("FAQ question with ID {Id} not found", id);
            return NotFound(new { message = $"FAQ question with ID {id} not found" });
        }

        _context.FaqQuestions.Remove(faq);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted FAQ question with ID {Id}", id);
        return NoContent();
    }
}

/// <summary>
/// DTO for FAQ question
/// </summary>
public class FaqQuestionDto
{
    public long Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// Request to submit a question without an answer
/// </summary>
public class SubmitQuestionRequest
{
    /// <summary>
    /// The question text
    /// </summary>
    /// <example>Jak mogę zaktualizować dane mojej firmy?</example>
    public string Question { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating FAQ question
/// </summary>
public class CreateFaqQuestionRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating FAQ question
/// </summary>
public class UpdateFaqQuestionRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// Paginated result wrapper
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
