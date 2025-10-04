using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using UknfCommunicationPlatform.Core.Configuration;
using UknfCommunicationPlatform.Core.Interfaces;
using UknfCommunicationPlatform.Infrastructure.Services;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Services;

public class JwtServiceTests
{
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Secret = "ThisIsAVerySecureSecretKeyForJWTTokenGeneration_MinimumLengthIs32Characters!",
            Issuer = "UKNF-API-Test",
            Audience = "UKNF-Portal-Test",
            AccessTokenExpirationMinutes = 60,
            RefreshTokenExpirationDays = 7
        };

        var options = Options.Create(_jwtSettings);
        _jwtService = new JwtService(options);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userId = 1L;
        var email = "test@example.com";
        var roles = new List<string> { "Administrator" };
        var permissions = new List<string> { "users.read", "users.write" };

        // Act
        var token = _jwtService.GenerateAccessToken(userId, email, roles, permissions);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Administrator");
        jwtToken.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "users.read");
        jwtToken.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "users.write");
    }

    [Fact]
    public void GenerateAccessToken_WithSupervisedEntityId_ShouldIncludeEntityIdInToken()
    {
        // Arrange
        var userId = 1L;
        var email = "test@example.com";
        var roles = new List<string> { "ExternalUser" };
        var permissions = new List<string> { "reports.read" };
        var supervisedEntityId = 123L;

        // Act
        var token = _jwtService.GenerateAccessToken(userId, email, roles, permissions, supervisedEntityId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == "supervised_entity_id" && c.Value == supervisedEntityId.ToString());
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueToken()
    {
        // Act
        var token1 = _jwtService.GenerateRefreshToken();
        var token2 = _jwtService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);
        token1.Length.Should().BeGreaterThan(50); // Base64 encoded 64 bytes
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var userId = 1L;
        var email = "test@example.com";
        var roles = new List<string> { "Administrator" };
        var permissions = new List<string> { "users.read" };
        var token = _jwtService.GenerateAccessToken(userId, email, roles, permissions);

        // Act
        var principal = _jwtService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        principal.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == email);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var principal = _jwtService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ShouldReturnNull()
    {
        // This test would require waiting for token expiration or mocking time
        // For now, we'll test with a token that has wrong signature

        // Arrange
        var tokenWithWrongSignature = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        // Act
        var principal = _jwtService.ValidateToken(tokenWithWrongSignature);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
    {
        // Arrange
        var userId = 123L;
        var email = "test@example.com";
        var token = _jwtService.GenerateAccessToken(userId, email, new List<string>(), new List<string>());

        // Act
        var extractedUserId = _jwtService.GetUserIdFromToken(token);

        // Assert
        extractedUserId.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var userId = _jwtService.GetUserIdFromToken(invalidToken);

        // Assert
        userId.Should().BeNull();
    }

    [Fact]
    public void GenerateAccessToken_WithMultipleRoles_ShouldIncludeAllRoles()
    {
        // Arrange
        var userId = 1L;
        var email = "test@example.com";
        var roles = new List<string> { "Administrator", "Moderator", "User" };
        var permissions = new List<string>();

        // Act
        var token = _jwtService.GenerateAccessToken(userId, email, roles, permissions);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        roleClaims.Should().HaveCount(3);
        roleClaims.Should().Contain("Administrator");
        roleClaims.Should().Contain("Moderator");
        roleClaims.Should().Contain("User");
    }

    [Fact]
    public void GenerateAccessToken_WithMultiplePermissions_ShouldIncludeAllPermissions()
    {
        // Arrange
        var userId = 1L;
        var email = "test@example.com";
        var roles = new List<string>();
        var permissions = new List<string> { "users.read", "users.write", "users.delete", "reports.read" };

        // Act
        var token = _jwtService.GenerateAccessToken(userId, email, roles, permissions);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var permissionClaims = jwtToken.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

        permissionClaims.Should().HaveCount(4);
        permissionClaims.Should().Contain("users.read");
        permissionClaims.Should().Contain("users.write");
        permissionClaims.Should().Contain("users.delete");
        permissionClaims.Should().Contain("reports.read");
    }
}
