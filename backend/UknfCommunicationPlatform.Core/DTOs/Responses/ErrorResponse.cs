namespace UknfCommunicationPlatform.Core.DTOs.Responses;

/// <summary>
/// Standard error response
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Error code (optional)
    /// </summary>
    public string? ErrorCode { get; set; }
}
