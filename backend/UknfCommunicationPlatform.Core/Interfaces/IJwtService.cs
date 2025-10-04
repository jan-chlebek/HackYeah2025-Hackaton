using System.Security.Claims;

namespace UknfCommunicationPlatform.Core.Interfaces;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate JWT access token for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="roles">User roles</param>
    /// <param name="permissions">User permissions</param>
    /// <param name="supervisedEntityId">Supervised entity ID (for external users)</param>
    /// <returns>JWT access token</returns>
    string GenerateAccessToken(
        long userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        long? supervisedEntityId = null);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    /// <returns>Cryptographically secure random token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Get user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    long? GetUserIdFromToken(string token);
}
