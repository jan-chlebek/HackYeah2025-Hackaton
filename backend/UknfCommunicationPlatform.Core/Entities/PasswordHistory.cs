namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Tracks historical passwords for a user to prevent password reuse.
/// Stores hashed passwords only.
/// </summary>
public class PasswordHistory
{
    /// <summary>
    /// Unique identifier for this history entry
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to User
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Hashed password (never store plain text)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// When this password was set
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation property
    /// <summary>
    /// The user this password history belongs to
    /// </summary>
    public User User { get; set; } = null!;
}
