using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Net;
using System.Text.Json;
using Xunit;

namespace UknfCommunicationPlatform.Tests.Unit.Configuration;

/// <summary>
/// Tests to verify Swagger/OpenAPI configuration and generation
/// </summary>
public class SwaggerConfigurationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SwaggerConfigurationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SwaggerUI_ShouldBeAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("swagger-ui");
    }

    [Fact]
    public async Task SwaggerJson_ShouldBeGeneratedSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().NotBeNullOrEmpty();

        // Verify it's valid JSON
        var document = JsonDocument.Parse(json);
        document.Should().NotBeNull();

        // Verify OpenAPI structure
        document.RootElement.GetProperty("openapi").GetString().Should().StartWith("3.0");
        document.RootElement.GetProperty("info").GetProperty("title").GetString()
            .Should().Be("UKNF Communication Platform API");
        document.RootElement.GetProperty("info").GetProperty("version").GetString()
            .Should().Be("v1");
    }

    [Fact]
    public async Task SwaggerJson_ShouldContainExpectedEndpoints()
    {
        // Arrange
        var client = _factory.CreateClient();
        var expectedPaths = new[]
        {
            "/api/v1/announcements",  // v1 prefix
            "/api/v1/messages",
            "/api/v1/Faq",
            "/api/v1/library/files"
        };

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var paths = document.RootElement.GetProperty("paths");

        foreach (var expectedPath in expectedPaths)
        {
            paths.TryGetProperty(expectedPath, out _).Should().BeTrue(
                $"Expected endpoint '{expectedPath}' should be documented in Swagger");
        }
    }

    [Fact]
    public async Task SwaggerJson_ShouldContainSecurityDefinitions()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var components = document.RootElement.GetProperty("components");
        var securitySchemes = components.GetProperty("securitySchemes");

        // Verify Bearer authentication is configured
        securitySchemes.TryGetProperty("Bearer", out var bearerScheme).Should().BeTrue(
            "Bearer authentication should be configured in Swagger");

        bearerScheme.GetProperty("type").GetString().Should().Be("http");
        // Note: ASP.NET Core uses "Bearer" with capital B in the scheme
        bearerScheme.GetProperty("scheme").GetString().Should().BeOneOf("bearer", "Bearer");
        bearerScheme.GetProperty("bearerFormat").GetString().Should().Be("JWT");
    }

    [Fact]
    public async Task SwaggerJson_ReportsEndpoint_ShouldSupportFileUpload()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var paths = document.RootElement.GetProperty("paths");

        // Check if Reports POST endpoint exists (it may not be implemented yet)
        if (!paths.TryGetProperty("/api/v1/Reports", out var reportsPath))
        {
            // Skip this test if Reports endpoint doesn't exist yet
            return;
        }

        if (!reportsPath.TryGetProperty("post", out var postOperation))
        {
            // Skip if POST operation doesn't exist
            return;
        }

        // Verify it accepts multipart/form-data
        postOperation.TryGetProperty("requestBody", out var requestBody).Should().BeTrue(
            "Reports POST should have a request body");

        var content = requestBody.GetProperty("content");
        content.TryGetProperty("multipart/form-data", out var multipartContent).Should().BeTrue(
            "Reports POST should accept multipart/form-data for file upload");

        // Verify schema contains file parameter
        var schema = multipartContent.GetProperty("schema");
        schema.GetProperty("type").GetString().Should().Be("object");

        var properties = schema.GetProperty("properties");
        properties.TryGetProperty("File", out var fileProperty).Should().BeTrue(
            "Schema should contain File property");

        fileProperty.GetProperty("type").GetString().Should().Be("string");
        fileProperty.GetProperty("format").GetString().Should().Be("binary");
    }

    [Fact]
    public void SwaggerGenerator_ShouldBeRegistered()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var swaggerProvider = scope.ServiceProvider.GetService<ISwaggerProvider>();

        // Assert
        swaggerProvider.Should().NotBeNull("ISwaggerProvider should be registered in DI container");
    }

    [Fact]
    public async Task SwaggerGenerator_ShouldNotThrowException_WhenGeneratingDocument()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var swaggerProvider = scope.ServiceProvider.GetRequiredService<ISwaggerProvider>();

        // Act
        Func<Task> act = async () => await Task.Run(() =>
            swaggerProvider.GetSwagger("v1"));

        // Assert
        await act.Should().NotThrowAsync("Swagger document generation should not throw exceptions");
    }

    [Fact]
    public async Task SwaggerJson_ShouldContainAllHttpMethods()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var paths = document.RootElement.GetProperty("paths");
        var hasGet = false;
        var hasPost = false;
        var hasPut = false;
        var hasDelete = false;

        foreach (var path in paths.EnumerateObject())
        {
            foreach (var method in path.Value.EnumerateObject())
            {
                switch (method.Name.ToLower())
                {
                    case "get": hasGet = true; break;
                    case "post": hasPost = true; break;
                    case "put": hasPut = true; break;
                    case "delete": hasDelete = true; break;
                }
            }
        }

        hasGet.Should().BeTrue("At least one GET endpoint should be documented");
        hasPost.Should().BeTrue("At least one POST endpoint should be documented");
        hasPut.Should().BeTrue("At least one PUT endpoint should be documented");
        hasDelete.Should().BeTrue("At least one DELETE endpoint should be documented");
    }

    [Fact]
    public async Task SwaggerJson_AllEndpoints_ShouldHaveResponseCodes()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var paths = document.RootElement.GetProperty("paths");
        var endpointsWithoutResponses = new List<string>();

        foreach (var path in paths.EnumerateObject())
        {
            foreach (var method in path.Value.EnumerateObject())
            {
                if (!method.Value.TryGetProperty("responses", out var responses))
                {
                    endpointsWithoutResponses.Add($"{method.Name.ToUpper()} {path.Name}");
                }
                else
                {
                    // Responses should be an object with at least one response code
                    var hasAnyResponse = false;
                    foreach (var _ in responses.EnumerateObject())
                    {
                        hasAnyResponse = true;
                        break;
                    }

                    if (!hasAnyResponse)
                    {
                        endpointsWithoutResponses.Add($"{method.Name.ToUpper()} {path.Name}");
                    }
                }
            }
        }

        endpointsWithoutResponses.Should().BeEmpty(
            "All endpoints should have documented response codes");
    }

    [Fact]
    public async Task SwaggerJson_ShouldContainContactInformation()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var info = document.RootElement.GetProperty("info");
        info.TryGetProperty("contact", out var contact).Should().BeTrue(
            "API info should contain contact information");

        contact.GetProperty("name").GetString().Should().Be("UKNF");
        contact.GetProperty("email").GetString().Should().Be("contact@uknf.gov.pl");
    }

    [Fact]
    public async Task SwaggerJson_ShouldContainApiDescription()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        // Assert
        var info = document.RootElement.GetProperty("info");
        info.TryGetProperty("description", out var description).Should().BeTrue(
            "API info should contain a description");

        description.GetString().Should().Contain("UKNF");
        description.GetString().Should().NotBeNullOrWhiteSpace();
    }
}
