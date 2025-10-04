namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a rating for an FAQ answer
/// </summary>
public class FaqRating
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// FAQ question ID
    /// </summary>
    public long FaqQuestionId { get; set; }

    /// <summary>
    /// Navigation property - FAQ question
    /// </summary>
    public FaqQuestion FaqQuestion { get; set; } = null!;

    /// <summary>
    /// User ID who rated
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Rating value (1-5)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Rating date
    /// </summary>
    public DateTime RatedAt { get; set; }
}
