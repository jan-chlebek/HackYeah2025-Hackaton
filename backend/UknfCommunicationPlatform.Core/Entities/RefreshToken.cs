namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// User ID this token belongs to
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The refresh token value
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// When this token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// When this token was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// IP address that created this token
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// When this token was revoked (null if still valid)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// IP address that revoked this token
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Token that replaced this one (when refreshed)
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string? RevocationReason { get; set; }

    /// <summary>
    /// Check if token is currently active
    /// </summary>
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Check if token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
