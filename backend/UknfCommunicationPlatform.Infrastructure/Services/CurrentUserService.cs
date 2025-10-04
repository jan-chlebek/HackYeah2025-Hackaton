using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UknfCommunicationPlatform.Core.Interfaces;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Service for accessing current authenticated user information from HTTP context
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <inheritdoc/>
    public long? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <inheritdoc/>
    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    /// <inheritdoc/>
    public long? SupervisedEntityId
    {
        get
        {
            var entityIdClaim = User?.FindFirst("supervised_entity_id")?.Value;
            return long.TryParse(entityIdClaim, out var entityId) ? entityId : null;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public IEnumerable<string> Permissions =>
        User?.FindAll("permission").Select(c => c.Value) ?? Enumerable.Empty<string>();

    /// <inheritdoc/>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public bool HasPermission(string permission) =>
        Permissions.Contains(permission);

    /// <inheritdoc/>
    public bool HasRole(string role) =>
        Roles.Contains(role);

    /// <inheritdoc/>
    public bool IsInternalUser =>
        HasRole("Administrator") || HasRole("InternalUser") || HasRole("Supervisor");

    /// <inheritdoc/>
    public bool IsExternalUser =>
        HasRole("ExternalUser") && SupervisedEntityId.HasValue;
}
