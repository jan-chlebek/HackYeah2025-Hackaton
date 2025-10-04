namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// System-wide password policy configuration.
/// This is a single-row configuration table.
/// </summary>
public class PasswordPolicy
{
    /// <summary>
    /// Primary key (should always be 1 - single row table)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Minimum password length required
    /// </summary>
    public int MinLength { get; set; } = 12;

    /// <summary>
    /// Whether password must contain at least one uppercase letter
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Whether password must contain at least one lowercase letter
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Whether password must contain at least one digit
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Whether password must contain at least one special character
    /// </summary>
    public bool RequireSpecialChar { get; set; } = true;

    /// <summary>
    /// Number of days until password expires (0 = never expires)
    /// </summary>
    public int ExpirationDays { get; set; } = 90;

    /// <summary>
    /// Number of previous passwords to check against reuse
    /// </summary>
    public int HistoryCount { get; set; } = 5;

    /// <summary>
    /// Maximum failed login attempts before account lockout
    /// </summary>
    public int MaxFailedAttempts { get; set; } = 5;

    /// <summary>
    /// Duration of account lockout in minutes
    /// </summary>
    public int LockoutDurationMinutes { get; set; } = 30;

    /// <summary>
    /// When this policy was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// ID of the user who last updated this policy
    /// </summary>
    public long? UpdatedByUserId { get; set; }

    // Navigation property
    /// <summary>
    /// User who last updated this policy
    /// </summary>
    public User? UpdatedByUser { get; set; }
}
