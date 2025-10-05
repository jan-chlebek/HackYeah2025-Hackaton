using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UknfCommunicationPlatform.Core.DTOs.Auth;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for authentication endpoints
/// Tests login, logout, and token refresh with real database credentials
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class AuthenticationEndpointTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private readonly HttpClient _client;

    // Test credentials from DatabaseSeeder
    private const string AdminEmail = "admin@uknf.gov.pl";
    private const string AdminPassword = "Admin123!";
    private const string InternalUserEmail = "jan.kowalski@uknf.gov.pl";
    private const string InternalUserPassword = "User123!";
    private const string SupervisorEmail = "anna.nowak@uknf.gov.pl";
    private const string SupervisorPassword = "Supervisor123!";

    public AuthenticationEndpointTests(TestDatabaseFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region Login Tests

    [Fact]
    public async Task Login_WithAdminCredentials_ReturnsTokenAndAdministratorRole()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().BeGreaterThan(0);
        
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(AdminEmail);
        result.User.Roles.Should().Contain("Administrator");
        result.User.Permissions.Should().NotBeEmpty();
        result.User.Permissions.Should().Contain("users.read");
        result.User.Permissions.Should().Contain("users.write");
        result.User.Permissions.Should().Contain("users.delete");
        result.User.Permissions.Should().Contain("entities.read");
        result.User.Permissions.Should().Contain("entities.write");
        result.User.Permissions.Should().Contain("messages.read");
        result.User.Permissions.Should().Contain("messages.write");
        result.User.Permissions.Should().Contain("reports.read");
        result.User.Permissions.Should().Contain("reports.write");
    }

    [Fact]
    public async Task Login_WithInternalUserCredentials_ReturnsTokenAndInternalUserRole()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = InternalUserEmail,
            Password = InternalUserPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        
        result.User.Email.Should().Be(InternalUserEmail);
        result.User.Roles.Should().Contain("InternalUser");
        result.User.Permissions.Should().Contain("messages.read");
        result.User.Permissions.Should().Contain("messages.write");
        result.User.Permissions.Should().Contain("entities.read");
        result.User.Permissions.Should().Contain("reports.read");
    }

    [Fact]
    public async Task Login_WithSupervisorCredentials_ReturnsTokenAndSupervisorRole()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = SupervisorEmail,
            Password = SupervisorPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        
        result.User.Email.Should().Be(SupervisorEmail);
        result.User.Roles.Should().Contain("Supervisor");
        result.User.Permissions.Should().Contain("messages.read");
        result.User.Permissions.Should().Contain("messages.write");
        result.User.Permissions.Should().Contain("entities.read");
        result.User.Permissions.Should().Contain("entities.write");
        result.User.Permissions.Should().Contain("reports.read");
        result.User.Permissions.Should().Contain("reports.write");
        result.User.Permissions.Should().Contain("users.read");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@uknf.gov.pl",
            Password = "SomePassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "",
            Password = AdminPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Token Refresh Tests

    [Fact]
    public async Task RefreshToken_WithValidTokens_ReturnsNewTokens()
    {
        // Arrange - First login to get tokens
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = loginResult!.AccessToken,
            RefreshToken = loginResult.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBe(loginResult.AccessToken); // Should be a new token
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(AdminEmail);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidAccessToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = "invalid-access-token",
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithEmptyTokens_ReturnsBadRequest()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = "",
            RefreshToken = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region JWT Token Validation Tests

    [Fact]
    public async Task Login_ReturnsJwtWithCorrectClaims()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        result!.AccessToken.Should().NotBeNullOrEmpty();
        
        // JWT should have 3 parts separated by dots (header.payload.signature)
        var tokenParts = result.AccessToken.Split('.');
        tokenParts.Should().HaveCount(3);
        
        // Decode the payload (base64 decode the middle part)
        var payload = DecodeJwtPayload(tokenParts[1]);
        
        // Verify key claims are present
        payload.Should().Contain("email");
        payload.Should().Contain(AdminEmail);
        payload.Should().Contain("Administrator");
        payload.Should().Contain("iss"); // Issuer
        payload.Should().Contain("aud"); // Audience
        payload.Should().Contain("exp"); // Expiration
    }

    [Fact]
    public async Task Login_TokenExpiresInOneHour()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        result!.ExpiresIn.Should().Be(3600); // 1 hour in seconds
    }

    #endregion

    #region Multiple Login Tests

    [Fact]
    public async Task Login_MultipleConcurrentSessions_AllReceiveDifferentTokens()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act - Login 3 times
        var response1 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result1 = await response1.Content.ReadFromJsonAsync<LoginResponse>();

        var response2 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result2 = await response2.Content.ReadFromJsonAsync<LoginResponse>();

        var response3 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result3 = await response3.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert - All tokens should be different
        result1!.AccessToken.Should().NotBe(result2!.AccessToken);
        result1.AccessToken.Should().NotBe(result3!.AccessToken);
        result2.AccessToken.Should().NotBe(result3.AccessToken);

        result1.RefreshToken.Should().NotBe(result2.RefreshToken);
        result1.RefreshToken.Should().NotBe(result3.RefreshToken);
        result2.RefreshToken.Should().NotBe(result3.RefreshToken);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Decode JWT payload from base64
    /// </summary>
    private string DecodeJwtPayload(string base64Payload)
    {
        // Add padding if needed (JWT base64 is unpadded)
        var padding = base64Payload.Length % 4;
        if (padding > 0)
        {
            base64Payload += new string('=', 4 - padding);
        }

        var bytes = Convert.FromBase64String(base64Payload);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    #endregion
}
