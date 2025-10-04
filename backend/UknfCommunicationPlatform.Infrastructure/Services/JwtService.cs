using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UknfCommunicationPlatform.Core.Configuration;
using UknfCommunicationPlatform.Core.Interfaces;

namespace UknfCommunicationPlatform.Infrastructure.Services;

/// <summary>
/// Implementation of JWT token service
/// </summary>
public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <inheritdoc/>
    public string GenerateAccessToken(
        long userId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        long? supervisedEntityId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions as custom claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        // Add supervised entity ID if present
        if (supervisedEntityId.HasValue)
        {
            claims.Add(new Claim("supervised_entity_id", supervisedEntityId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return _tokenHandler.WriteToken(token);
    }

    /// <inheritdoc/>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <inheritdoc/>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public long? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null)
            return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return null;

        return long.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
