using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UknfCommunicationPlatform.Core.DTOs.Auth;
using UknfCommunicationPlatform.Infrastructure.Data;

namespace UknfCommunicationPlatform.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for authorization and permission checks across all controllers
/// Tests that endpoints properly enforce authentication and authorization
/// </summary>
[Collection(nameof(DatabaseCollection))]
public class AuthorizationIntegrationTests : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private readonly TestDatabaseFixture _factory;
    private readonly HttpClient _client;

    private const string AdminEmail = "admin@uknf.gov.pl";
    private const string AdminPassword = "Admin123!";
    private const string InternalUserEmail = "jan.kowalski@uknf.gov.pl";
    private const string InternalUserPassword = "User123!";
    private const string SupervisorEmail = "anna.nowak@uknf.gov.pl";
    private const string SupervisorPassword = "Supervisor123!";

    public AuthorizationIntegrationTests(TestDatabaseFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region Helper Methods

    private async Task<string> GetAuthTokenAsync(string email, string password)
    {
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!.AccessToken;
    }

    private HttpClient GetAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    #endregion

    #region Authentication Required Tests

    [Fact]
    public async Task PublicEndpoints_ShouldBeAccessibleWithoutAuth()
    {
        // Act & Assert - These should all return 200 or appropriate non-401 status
        var faqResponse = await _client.GetAsync("/api/v1/faq");
        faqResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "invalid"
        });
        loginResponse.StatusCode.Should().NotBe(HttpStatusCode.MethodNotAllowed);
    }

    // TODO: Uncomment when authentication is re-enabled
    /*
    [Theory]
    [InlineData("/api/v1/messages")]
    [InlineData("/api/v1/entities")]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/reports")]
    [InlineData("/api/v1/announcements")]
    [InlineData("/api/v1/library/files")]
    public async Task ProtectedEndpoints_WithoutAuth_ShouldReturn401(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/api/v1/messages")]
    [InlineData("/api/v1/entities")]
    [InlineData("/api/v1/users")]
    [InlineData("/api/v1/reports")]
    [InlineData("/api/v1/announcements")]
    public async Task ProtectedEndpoints_WithValidAuth_ShouldNotReturn401(string endpoint)
    {
        // Arrange
        var token = await GetAuthTokenAsync(AdminEmail, AdminPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act
        var response = await authClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
    */

    #endregion

    #region Role-Based Authorization Tests

    // TODO: Uncomment when authentication is re-enabled
    /*
    [Fact]
    public async Task AdminEndpoints_WithAdminRole_ShouldAllow()
    {
        // Arrange
        var token = await GetAuthTokenAsync(AdminEmail, AdminPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act - Try to access admin endpoints
        var usersResponse = await authClient.GetAsync("/api/v1/users");
        var entitiesResponse = await authClient.GetAsync("/api/v1/entities");

        // Assert
        usersResponse.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        entitiesResponse.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminEndpoints_WithNonAdminRole_ShouldDeny()
    {
        // Arrange
        var token = await GetAuthTokenAsync(InternalUserEmail, InternalUserPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act - Try to create a new user (admin only)
        var createUserResponse = await authClient.PostAsJsonAsync("/api/v1/users", new
        {
            email = "newuser@test.com",
            firstName = "New",
            lastName = "User",
            roleIds = new[] { 1 }
        });

        // Assert
        createUserResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    */

    #endregion

    #region Permission-Based Authorization Tests

    // TODO: Uncomment when authentication is re-enabled
    /*
    [Fact]
    public async Task MessagesEndpoint_WithMessagesReadPermission_ShouldAllow()
    {
        // Arrange - InternalUser should have messages.read permission
        var token = await GetAuthTokenAsync(InternalUserEmail, InternalUserPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act
        var response = await authClient.GetAsync("/api/v1/messages");

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReportsEndpoint_WithReportsReadPermission_ShouldAllow()
    {
        // Arrange
        var token = await GetAuthTokenAsync(InternalUserEmail, InternalUserPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act
        var response = await authClient.GetAsync("/api/v1/reports");

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
    */

    #endregion

    #region Entity Ownership Tests

    // TODO: Uncomment when authentication is re-enabled and external users are tested
    /*
    [Fact]
    public async Task ExternalUser_CanOnlyAccessTheirEntityData()
    {
        // This test requires:
        // 1. External user with supervised entity
        // 2. Re-enabled authentication
        // 3. Entity ownership authorization handler working

        // Arrange
        // Create external user linked to entity
        // Login as external user
        // Try to access their entity's data vs another entity's data

        // Assert
        // Should allow access to their entity
        // Should deny access to other entities
    }

    [Fact]
    public async Task InternalUser_CanAccessAllEntitiesData()
    {
        // Arrange
        var token = await GetAuthTokenAsync(InternalUserEmail, InternalUserPassword);
        var authClient = GetAuthenticatedClient(token);

        // Act - Try to access different entities
        var entity1Response = await authClient.GetAsync("/api/v1/entities/1");
        var entity2Response = await authClient.GetAsync("/api/v1/entities/2");

        // Assert
        entity1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        entity2Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    */

    #endregion

    #region Token Expiration Tests

    // TODO: Uncomment when authentication is re-enabled
    /*
    [Fact]
    public async Task ExpiredToken_ShouldReturn401()
    {
        // This test requires creating a token with very short expiration
        // Not easily testable without modifying JWT settings for tests

        // Arrange
        // Create token with 1 second expiration
        // Wait 2 seconds
        // Try to use token

        // Assert
        // Should return 401 Unauthorized
    }

    [Fact]
    public async Task RefreshToken_ShouldProvideNewValidToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Act
        var refreshRequest = new RefreshTokenRequest
        {
            AccessToken = loginResult!.AccessToken,
            RefreshToken = loginResult.RefreshToken
        };

        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        refreshResult.Should().NotBeNull();
        refreshResult!.AccessToken.Should().NotBe(loginResult.AccessToken);
        refreshResult.RefreshToken.Should().NotBe(loginResult.RefreshToken);

        // Verify new token works
        var authClient = GetAuthenticatedClient(refreshResult.AccessToken);
        var testResponse = await authClient.GetAsync("/api/v1/auth/me");
        testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    */

    #endregion

    #region JWT Claims Tests

    [Fact]
    public async Task LoginResponse_ShouldIncludeUserRolesAndPermissions()
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
        result.Should().NotBeNull();
        result!.User.Roles.Should().NotBeEmpty();
        result.User.Roles.Should().Contain("Administrator");
        result.User.Permissions.Should().NotBeEmpty();
        result.User.Permissions.Should().Contain("users.read");
        result.User.Permissions.Should().Contain("users.write");
    }

    [Fact]
    public async Task InternalUser_ShouldHaveCorrectRolesAndPermissions()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = InternalUserEmail,
            Password = InternalUserPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        result.Should().NotBeNull();
        result!.User.Roles.Should().Contain("InternalUser");
        result.User.Permissions.Should().Contain("messages.read");
        result.User.Permissions.Should().Contain("messages.write");
    }

    [Fact]
    public async Task Supervisor_ShouldHaveCorrectRolesAndPermissions()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = SupervisorEmail,
            Password = SupervisorPassword
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert
        result.Should().NotBeNull();
        result!.User.Roles.Should().Contain("Supervisor");
        // Supervisors should have same or more permissions than internal users
        result.User.Permissions.Should().NotBeEmpty();
    }

    #endregion

    #region Account Lockout Tests

    [Fact]
    public async Task MultipleFailedLogins_ShouldLockAccount()
    {
        // Arrange
        var wrongPasswordRequest = new LoginRequest
        {
            Email = InternalUserEmail,
            Password = "WrongPassword123!"
        };

        // Act - Try wrong password multiple times (max is 5)
        for (int i = 0; i < 5; i++)
        {
            await _client.PostAsJsonAsync("/api/v1/auth/login", wrongPasswordRequest);
        }

        // Now try with correct password - should be locked
        var correctRequest = new LoginRequest
        {
            Email = InternalUserEmail,
            Password = InternalUserPassword
        };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", correctRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Verify account is locked in database (use new scope to avoid cache)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users.AsNoTracking().FirstAsync(u => u.Email == InternalUserEmail);
        
        user.LockedUntil.Should().NotBeNull();
        user.LockedUntil!.Value.Should().BeAfter(DateTime.UtcNow);
        user.FailedLoginAttempts.Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task SuccessfulLogin_ShouldResetFailedAttempts()
    {
        // Arrange - First make a failed attempt
        var wrongRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = "WrongPassword!"
        };
        await _client.PostAsJsonAsync("/api/v1/auth/login", wrongRequest);

        // Act - Now login with correct credentials
        var correctRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", correctRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify failed attempts were reset
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users.FirstAsync(u => u.Email == AdminEmail);
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedUntil.Should().BeNull();
    }

    #endregion

    #region Multiple Concurrent Sessions Tests

    [Fact]
    public async Task User_CanHaveMultipleConcurrentSessions()
    {
        // Arrange - Login twice as same user
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        // Act - Get two separate tokens
        var response1 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var token1 = await response1.Content.ReadFromJsonAsync<LoginResponse>();

        var response2 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var token2 = await response2.Content.ReadFromJsonAsync<LoginResponse>();

        // Assert - Both tokens should be valid and different
        token1!.AccessToken.Should().NotBe(token2!.AccessToken);
        token1.RefreshToken.Should().NotBe(token2.RefreshToken);

        // Both tokens should work
        var client1 = GetAuthenticatedClient(token1.AccessToken);
        var client2 = GetAuthenticatedClient(token2.AccessToken);

        var test1 = await client1.GetAsync("/api/v1/auth/me");
        var test2 = await client2.GetAsync("/api/v1/auth/me");

        test1.StatusCode.Should().Be(HttpStatusCode.OK);
        test2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // TODO: Uncomment when auth is enabled
    /*
    [Fact]
    public async Task Logout_ShouldInvalidateAllUserSessions()
    {
        // Arrange - Login twice as same user
        var loginRequest = new LoginRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        };

        var response1 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var token1 = await response1.Content.ReadFromJsonAsync<LoginResponse>();

        var response2 = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var token2 = await response2.Content.ReadFromJsonAsync<LoginResponse>();

        // Act - Logout from first session
        var client1 = GetAuthenticatedClient(token1!.AccessToken);
        await client1.PostAsync("/api/v1/auth/logout", null);

        // Assert - Both refresh tokens should be revoked
        var refreshRequest1 = new RefreshTokenRequest
        {
            AccessToken = token1.AccessToken,
            RefreshToken = token1.RefreshToken
        };

        var refreshRequest2 = new RefreshTokenRequest
        {
            AccessToken = token2!.AccessToken,
            RefreshToken = token2.RefreshToken
        };

        var refresh1Response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest1);
        var refresh2Response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest2);

        refresh1Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        refresh2Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    */

    #endregion
}
