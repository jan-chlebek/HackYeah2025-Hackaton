using Microsoft.AspNetCore.Mvc;
using UknfCommunicationPlatform.Core.DTOs.Entities;
using UknfCommunicationPlatform.Core.DTOs.Users;
using UknfCommunicationPlatform.Infrastructure.Services;

namespace UknfCommunicationPlatform.Api.Controllers.v1;

/// <summary>
/// Supervised entity management operations (for system administrators)
/// </summary>
[ApiController]
[Route("api/v1/entities")]
[Produces("application/json")]
public class EntitiesController : ControllerBase
{
    private readonly EntityManagementService _entityService;
    private readonly ILogger<EntitiesController> _logger;

    public EntitiesController(EntityManagementService entityService, ILogger<EntitiesController> logger)
    {
        _entityService = entityService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of supervised entities with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="searchTerm">Search in name, NIP, REGON, KRS</param>
    /// <param name="entityType">Filter by entity type</param>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>List of entities with pagination metadata</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetEntities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? entityType = null,
        [FromQuery] bool? isActive = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (entities, totalCount) = await _entityService.GetEntitiesAsync(
            page, pageSize, searchTerm, entityType, isActive);

        return Ok(new
        {
            data = entities,
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
    /// Get supervised entity by ID
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntityResponse>> GetEntity(long id)
    {
        var entity = await _entityService.GetEntityByIdAsync(id);
        if (entity == null)
        {
            return NotFound(new { error = $"Entity with ID {id} not found" });
        }

        return Ok(entity);
    }

    /// <summary>
    /// Create a new supervised entity
    /// </summary>
    /// <param name="request">Entity creation data</param>
    /// <returns>Created entity details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntityResponse>> CreateEntity([FromBody] CreateEntityRequest request)
    {
        try
        {
            var entity = await _entityService.CreateEntityAsync(request);
            return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing supervised entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <param name="request">Updated entity data</param>
    /// <returns>Updated entity details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntityResponse>> UpdateEntity(long id, [FromBody] UpdateEntityRequest request)
    {
        try
        {
            var entity = await _entityService.UpdateEntityAsync(id, request);
            return Ok(entity);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { error = ex.Message });
            }
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a supervised entity (soft delete)
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEntity(long id)
    {
        var result = await _entityService.DeleteEntityAsync(id);
        if (!result)
        {
            return NotFound(new { error = $"Entity with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get users belonging to a supervised entity
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>List of users</returns>
    [HttpGet("{id}/users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserListItemResponse>>> GetEntityUsers(long id)
    {
        var users = await _entityService.GetEntityUsersAsync(id);
        return Ok(users);
    }

    /// <summary>
    /// Bulk import supervised entities from CSV (placeholder for now)
    /// </summary>
    /// <returns>Import result</returns>
    [HttpPost("import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> ImportEntities()
    {
        // TODO: Implement CSV import
        // This will be implemented in Sprint 3
        return Ok(new
        {
            message = "CSV import not yet implemented",
            note = "This will be available in Sprint 3"
        });
    }
}
