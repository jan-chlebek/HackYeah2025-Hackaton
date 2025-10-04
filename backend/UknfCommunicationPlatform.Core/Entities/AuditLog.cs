namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Comprehensive audit log for tracking administrative actions and security events.
/// Critical for compliance and security monitoring in financial sector.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier for this audit log entry
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Foreign key to User who performed the action (null for system actions)
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Action performed (e.g., "UserCreated", "PasswordReset", "RoleAssigned")
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Resource type affected (e.g., "User", "Role", "Entity", "Report")
    /// </summary>
    public string Resource { get; set; } = string.Empty;

    /// <summary>
    /// ID of the affected resource (if applicable)
    /// </summary>
    public int? ResourceId { get; set; }

    /// <summary>
    /// JSON string with detailed information about the change
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// IP address from which the action was performed
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    // Navigation property
    /// <summary>
    /// The user who performed the action
    /// </summary>
    public User? User { get; set; }
}
