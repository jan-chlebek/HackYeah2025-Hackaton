using System.ComponentModel.DataAnnotations;

namespace UknfCommunicationPlatform.Core.DTOs.Auth;

/// <summary>
/// Request to refresh access token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Expired or about-to-expire access token
    /// </summary>
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token obtained during login
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
