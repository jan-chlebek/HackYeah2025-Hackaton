using UknfCommunicationPlatform.Core.Enums;

namespace UknfCommunicationPlatform.Core.Entities;

/// <summary>
/// Represents a question in the FAQ system
/// </summary>
public class FaqQuestion
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Question title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Question content/body
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Category (e.g., "Reports", "Technical Issues", "Legal")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Tags for search (comma-separated)
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public FaqQuestionStatus Status { get; set; }

    /// <summary>
    /// Answer content (WYSIWYG HTML)
    /// </summary>
    public string? AnswerContent { get; set; }

    /// <summary>
    /// Date answer was provided
    /// </summary>
    public DateTime? AnsweredAt { get; set; }

    /// <summary>
    /// User who answered (UKNF staff)
    /// </summary>
    public long? AnsweredByUserId { get; set; }

    /// <summary>
    /// Navigation property - User who answered
    /// </summary>
    public User? AnsweredBy { get; set; }

    /// <summary>
    /// Submitted by user ID (can be anonymous)
    /// </summary>
    public long? SubmittedByUserId { get; set; }

    /// <summary>
    /// Navigation property - Submitter
    /// </summary>
    public User? SubmittedBy { get; set; }

    /// <summary>
    /// Anonymous submitter name (if not logged in)
    /// </summary>
    public string? AnonymousName { get; set; }

    /// <summary>
    /// Anonymous submitter email (if not logged in)
    /// </summary>
    public string? AnonymousEmail { get; set; }

    /// <summary>
    /// Submission date
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Publication date (when made visible to all)
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// View count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Average rating (1-5 stars)
    /// </summary>
    public decimal? AverageRating { get; set; }

    /// <summary>
    /// Total number of ratings
    /// </summary>
    public int RatingCount { get; set; }

    /// <summary>
    /// Navigation property - Individual ratings
    /// </summary>
    public ICollection<FaqRating> Ratings { get; set; } = new List<FaqRating>();
}
