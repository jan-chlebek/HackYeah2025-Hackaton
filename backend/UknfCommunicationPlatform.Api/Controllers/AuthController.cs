using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UknfCommunicationPlatform.Core.DTOs.Auth;
using UknfCommunicationPlatform.Core.Interfaces;

namespace UknfCommunicationPlatform.Api.Controllers;

/// <summary>
/// Controller for authentication and authorization operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress);

        if (result == null)
        {
            _logger.LogWarning("Login attempt failed for email: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        _logger.LogInformation("User {Email} logged in successfully", request.Email);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, ipAddress);

        if (result == null)
        {
            _logger.LogWarning("Refresh token failed from IP: {IpAddress}", ipAddress);
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        _logger.LogInformation("Token refreshed successfully");
        return Ok(result);
    }

    /// <summary>
    /// Logout (revokes all refresh tokens for the user)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LogoutAsync(userId, ipAddress);

        if (!result)
        {
            return BadRequest(new { message = "Logout failed" });
        }

        _logger.LogInformation("User {UserId} logged out successfully", userId);
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        if (!result)
        {
            return BadRequest(new { message = "Password change failed. Please check your current password and ensure the new password meets policy requirements." });
        }

        _logger.LogInformation("User {UserId} changed password successfully", userId);
        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Get current user information from JWT token
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var roles = User?.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
        var permissions = User?.FindAll("permission").Select(c => c.Value).ToList() ?? new List<string>();
        var supervisedEntityIdClaim = User?.FindFirst("supervised_entity_id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user authentication" });
        }

        long? supervisedEntityId = null;
        if (!string.IsNullOrEmpty(supervisedEntityIdClaim) && long.TryParse(supervisedEntityIdClaim, out var entityId))
        {
            supervisedEntityId = entityId;
        }

        return Ok(new
        {
            id = userId,
            email = emailClaim,
            roles,
            permissions,
            supervisedEntityId
        });
    }

    /// <summary>
    /// Check if account is locked (admin endpoint)
    /// </summary>
    /// <param name="userId">User ID to check</param>
    /// <returns>Lock status</returns>
    [HttpGet("users/{userId}/lock-status")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLockStatus(long userId)
    {
        var isLocked = await _authService.IsAccountLockedAsync(userId);
        return Ok(new { userId, isLocked });
    }

    /// <summary>
    /// Unlock user account (admin endpoint)
    /// </summary>
    /// <param name="userId">User ID to unlock</param>
    /// <returns>Success message</returns>
    [HttpPost("users/{userId}/unlock")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnlockAccount(long userId)
    {
        var result = await _authService.UnlockAccountAsync(userId);

        if (!result)
        {
            return BadRequest(new { message = "Failed to unlock account" });
        }

        var adminIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var adminId = long.TryParse(adminIdClaim, out var id) ? id : (long?)null;
        
        _logger.LogInformation("Account unlocked for user {UserId} by admin {AdminId}",
            userId, adminId);

        return Ok(new { message = "Account unlocked successfully" });
    }
}
