namespace UknfCommunicationPlatform.Core.DTOs.Auth;

/// <summary>
/// Response after successful login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type (usually "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserInfoDto User { get; set; } = new();
}

/// <summary>
/// Basic user information included in login response
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// User's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's roles
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// User's permissions
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Supervised entity ID (for external users)
    /// </summary>
    public long? SupervisedEntityId { get; set; }
}
