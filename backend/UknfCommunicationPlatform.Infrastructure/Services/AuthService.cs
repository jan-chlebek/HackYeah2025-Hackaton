using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UknfCommunicationPlatform.Core.DTOs.Auth;
using UknfCommunicationPlatform.Core.Entities;
using UknfCommunicationPlatform.Core.Interfaces;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Implementation of authentication service
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IPasswordHashingService passwordHashingService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHashingService = passwordHashingService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginResponse?> LoginAsync(string email, string password, string? ipAddress)
    {
        // Find user by email
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found with email {Email}", email);
            return null;
        }

        // Check if account is locked
        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
        {
            _logger.LogWarning("Login failed: Account locked for user {UserId} until {LockedUntil}",
                user.Id, user.LockedUntil.Value);
            return null;
        }

        // Check if account is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: Account inactive for user {UserId}", user.Id);
            return null;
        }

        // Verify password
        if (!_passwordHashingService.VerifyPassword(password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;

            // Lock account if too many failed attempts
            var passwordPolicy = await GetPasswordPolicyAsync();
            if (user.FailedLoginAttempts >= passwordPolicy.MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(passwordPolicy.LockoutDurationMinutes);
                _logger.LogWarning("Account locked for user {UserId} after {Attempts} failed login attempts",
                    user.Id, user.FailedLoginAttempts);
            }

            await _context.SaveChangesAsync();

            _logger.LogWarning("Login failed: Invalid password for user {UserId}. Failed attempts: {Attempts}",
                user.Id, user.FailedLoginAttempts);
            return null;
        }

        // Reset failed login attempts on successful login
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Extract roles and permissions
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            roles,
            permissions,
            user.SupervisedEntityId);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        // Store refresh token in database
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // TODO: Make configurable
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged in successfully from IP {IpAddress}", user.Id, ipAddress);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            TokenType = "Bearer",
            ExpiresIn = 60 * 60, // 1 hour in seconds
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles,
                Permissions = permissions,
                SupervisedEntityId = user.SupervisedEntityId
            }
        };
    }

    /// <inheritdoc/>
    public async Task<LoginResponse?> RefreshTokenAsync(string accessToken, string refreshToken, string? ipAddress)
    {
        // Validate the old access token (without checking expiration)
        var userId = _jwtService.GetUserIdFromToken(accessToken);
        if (userId == null)
        {
            _logger.LogWarning("Refresh token failed: Invalid access token");
            return null;
        }

        // Find the refresh token in database
        var storedRefreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId.Value);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            _logger.LogWarning("Refresh token failed: Token not found or inactive for user {UserId}", userId.Value);
            return null;
        }

        // Revoke old refresh token
        storedRefreshToken.RevokedAt = DateTime.UtcNow;
        storedRefreshToken.RevokedByIp = ipAddress;
        storedRefreshToken.RevocationReason = "Replaced by new token";

        // Generate new tokens
        var user = storedRefreshToken.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var newAccessToken = _jwtService.GenerateAccessToken(
            user.Id,
            user.Email,
            roles,
            permissions,
            user.SupervisedEntityId);

        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

        // Store new refresh token
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        storedRefreshToken.ReplacedByToken = newRefreshTokenValue;
        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Refresh token succeeded for user {UserId} from IP {IpAddress}", user.Id, ipAddress);

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            TokenType = "Bearer",
            ExpiresIn = 60 * 60,
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles,
                Permissions = permissions,
                SupervisedEntityId = user.SupervisedEntityId
            }
        };
    }

    /// <inheritdoc/>
    public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress, string? reason = null)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
        {
            _logger.LogWarning("Revoke token failed: Token not found or already inactive");
            return false;
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.RevocationReason = reason ?? "Revoked by user";

        await _context.SaveChangesAsync();

        _logger.LogInformation("Refresh token revoked for user {UserId} from IP {IpAddress}. Reason: {Reason}",
            token.UserId, ipAddress, token.RevocationReason);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> LogoutAsync(long userId, string? ipAddress)
    {
        // Revoke all active refresh tokens for the user
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.RevocationReason = "User logout";
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged out. Revoked {TokenCount} refresh tokens from IP {IpAddress}",
            userId, activeTokens.Count, ipAddress);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Change password failed: User {UserId} not found", userId);
            return false;
        }

        // Verify current password
        if (!_passwordHashingService.VerifyPassword(currentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Change password failed: Invalid current password for user {UserId}", userId);
            return false;
        }

        // Check password policy
        var passwordPolicy = await GetPasswordPolicyAsync();
        if (!ValidatePasswordPolicy(newPassword, passwordPolicy))
        {
            _logger.LogWarning("Change password failed: New password does not meet policy for user {UserId}", userId);
            return false;
        }

        // Check password history
        var passwordHistories = await _context.PasswordHistories
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.CreatedAt)
            .Take(passwordPolicy.HistoryCount)
            .ToListAsync();

        foreach (var history in passwordHistories)
        {
            if (_passwordHashingService.VerifyPassword(newPassword, history.PasswordHash))
            {
                _logger.LogWarning("Change password failed: Password recently used for user {UserId}", userId);
                return false;
            }
        }

        // Add old password to history
        _context.PasswordHistories.Add(new PasswordHistory
        {
            UserId = userId,
            PasswordHash = user.PasswordHash,
            CreatedAt = DateTime.UtcNow
        });

        // Update password
        user.PasswordHash = _passwordHashingService.HashPassword(newPassword);
        user.LastPasswordChangeAt = DateTime.UtcNow;
        user.RequirePasswordChange = false;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Password changed successfully for user {UserId}", userId);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> IsAccountLockedAsync(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        return user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public async Task<bool> UnlockAccountAsync(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("Unlock account failed: User {UserId} not found", userId);
            return false;
        }

        user.LockedUntil = null;
        user.FailedLoginAttempts = 0;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Account unlocked for user {UserId}", userId);

        return true;
    }

    // Helper methods

    private async Task<PasswordPolicy> GetPasswordPolicyAsync()
    {
        var policy = await _context.PasswordPolicies.FirstOrDefaultAsync();

        // Return default policy if none exists
        return policy ?? new PasswordPolicy
        {
            MinLength = 8,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecialChar = true,
            ExpirationDays = 90,
            HistoryCount = 5,
            MaxFailedAttempts = 5,
            LockoutDurationMinutes = 15
        };
    }

    private bool ValidatePasswordPolicy(string password, PasswordPolicy policy)
    {
        if (password.Length < policy.MinLength)
            return false;

        if (policy.RequireUppercase && !password.Any(char.IsUpper))
            return false;

        if (policy.RequireLowercase && !password.Any(char.IsLower))
            return false;

        if (policy.RequireDigit && !password.Any(char.IsDigit))
            return false;

        if (policy.RequireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

        return true;
    }
}
