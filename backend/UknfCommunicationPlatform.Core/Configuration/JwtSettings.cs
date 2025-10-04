namespace UknfCommunicationPlatform.Core.Configuration;

/// <summary>
/// JWT configuration settings
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for signing tokens
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer (e.g., "UKNF-API")
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience (e.g., "UKNF-Portal")
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration in minutes (default: 60)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration in days (default: 7)
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
