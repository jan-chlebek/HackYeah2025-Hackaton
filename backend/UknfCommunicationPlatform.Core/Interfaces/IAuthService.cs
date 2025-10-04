using UknfCommunicationPlatform.Core.DTOs.Auth;

namespace UknfCommunicationPlatform.Core.Interfaces;

/// <summary>
/// Service for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="ipAddress">Client IP address for audit</param>
    /// <returns>Login response with tokens and user info</returns>
    Task<LoginResponse?> LoginAsync(string email, string password, string? ipAddress);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="accessToken">Expired or about-to-expire access token</param>
    /// <param name="refreshToken">Valid refresh token</param>
    /// <param name="ipAddress">Client IP address for audit</param>
    /// <returns>New login response with refreshed tokens</returns>
    Task<LoginResponse?> RefreshTokenAsync(string accessToken, string refreshToken, string? ipAddress);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="ipAddress">Client IP address for audit</param>
    /// <param name="reason">Reason for revocation</param>
    /// <returns>True if revoked successfully</returns>
    Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress, string? reason = null);

    /// <summary>
    /// Logout user by revoking all their refresh tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ipAddress">Client IP address for audit</param>
    /// <returns>True if logged out successfully</returns>
    Task<bool> LogoutAsync(long userId, string? ipAddress);

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currentPassword">Current password</param>
    /// <param name="newPassword">New password</param>
    /// <returns>True if password changed successfully</returns>
    Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword);

    /// <summary>
    /// Check if user account is locked
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if account is locked</returns>
    Task<bool> IsAccountLockedAsync(long userId);

    /// <summary>
    /// Unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>True if unlocked successfully</returns>
    Task<bool> UnlockAccountAsync(long userId);
}
