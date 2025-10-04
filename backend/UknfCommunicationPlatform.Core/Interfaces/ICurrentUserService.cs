namespace UknfCommunicationPlatform.Core.Interfaces;

/// <summary>
/// Service for accessing current authenticated user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get current user ID
    /// </summary>
    long? UserId { get; }

    /// <summary>
    /// Get current user email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Get current user's supervised entity ID (for external users)
    /// </summary>
    long? SupervisedEntityId { get; }

    /// <summary>
    /// Get current user's roles
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Get current user's permissions
    /// </summary>
    IEnumerable<string> Permissions { get; }

    /// <summary>
    /// Check if current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Check if current user has a specific permission
    /// </summary>
    bool HasPermission(string permission);

    /// <summary>
    /// Check if current user has a specific role
    /// </summary>
    bool HasRole(string role);

    /// <summary>
    /// Check if current user is an internal user (UKNF staff)
    /// </summary>
    bool IsInternalUser { get; }

    /// <summary>
    /// Check if current user is an external user (entity representative)
    /// </summary>
    bool IsExternalUser { get; }
}
