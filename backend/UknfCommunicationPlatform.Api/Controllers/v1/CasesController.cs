using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UknfCommunicationPlatform.Api.Authorization;
using UknfCommunicationPlatform.Core.DTOs.Cases;
using UknfCommunicationPlatform.Core.Enums;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Cases/Access Requests management operations
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/cases")]
[Produces("application/json")]
public class CasesController : ControllerBase
{
    private readonly CaseService _caseService;
    private readonly ILogger<CasesController> _logger;

    public CasesController(CaseService caseService, ILogger<CasesController> logger)
    {
        _caseService = caseService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of cases with optional filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="status">Filter by status</param>
    /// <param name="supervisedEntityId">Filter by entity</param>
    /// <param name="handlerId">Filter by handler</param>
    /// <param name="category">Filter by category</param>
    /// <returns>List of cases with pagination metadata</returns>
    [HttpGet]
    [RequirePermission("cases.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<object>> GetCases(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] CaseStatus? status = null,
        [FromQuery] long? supervisedEntityId = null,
        [FromQuery] long? handlerId = null,
        [FromQuery] string? category = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (cases, totalCount) = await _caseService.GetCasesAsync(
            page, pageSize, status, supervisedEntityId, handlerId, category);

        return Ok(new
        {
            data = cases,
            pagination = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    /// <summary>
    /// Get case by ID
    /// </summary>
    /// <param name="id">Case ID</param>
    /// <returns>Case details</returns>
    [HttpGet("{id}")]
    [RequirePermission("cases.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaseResponse>> GetCase(long id)
    {
        var caseResponse = await _caseService.GetCaseByIdAsync(id);

        if (caseResponse == null)
        {
            return NotFound(new { message = $"Case with ID {id} not found" });
        }

        return Ok(caseResponse);
    }

    /// <summary>
    /// Create a new case
    /// </summary>
    /// <param name="request">Create case request</param>
    /// <returns>Created case</returns>
    [HttpPost]
    [RequirePermission("cases.write")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CaseResponse>> CreateCase([FromBody] CreateCaseRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        try
        {
            var caseResponse = await _caseService.CreateCaseAsync(request, userId);
            return CreatedAtAction(nameof(GetCase), new { id = caseResponse.Id }, caseResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating case");
            return StatusCode(500, new { message = "An error occurred while creating the case" });
        }
    }

    /// <summary>
    /// Update an existing case
    /// </summary>
    /// <param name="id">Case ID</param>
    /// <param name="request">Update case request</param>
    /// <returns>Updated case</returns>
    [HttpPut("{id}")]
    [RequirePermission("cases.write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaseResponse>> UpdateCase(long id, [FromBody] UpdateCaseRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        try
        {
            var caseResponse = await _caseService.UpdateCaseAsync(id, request, userId);

            if (caseResponse == null)
            {
                return NotFound(new { message = $"Case with ID {id} not found" });
            }

            return Ok(caseResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating case {CaseId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the case" });
        }
    }

    /// <summary>
    /// Delete a case (soft delete)
    /// </summary>
    /// <param name="id">Case ID</param>
    /// <param name="reason">Cancellation reason</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    [RequirePermission("cases.delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCase(long id, [FromQuery] string? reason = null)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        try
        {
            var success = await _caseService.DeleteCaseAsync(id, userId, reason);

            if (!success)
            {
                return NotFound(new { message = $"Case with ID {id} not found" });
            }

            return Ok(new { message = "Case deleted successfully", id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting case {CaseId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the case" });
        }
    }

    /// <summary>
    /// Update case status
    /// </summary>
    /// <param name="id">Case ID</param>
    /// <param name="newStatus">New status</param>
    /// <returns>Updated case</returns>
    [HttpPatch("{id}/status")]
    [RequirePermission("cases.write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaseResponse>> UpdateCaseStatus(long id, [FromQuery] CaseStatus newStatus)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        try
        {
            var caseResponse = await _caseService.UpdateCaseStatusAsync(id, newStatus, userId);

            if (caseResponse == null)
            {
                return NotFound(new { message = $"Case with ID {id} not found" });
            }

            return Ok(caseResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating case status {CaseId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the case status" });
        }
    }
}
