using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UknfCommunicationPlatform.Core.Configuration;
using UknfCommunicationPlatform.Core.Authorization;
using UknfCommunicationPlatform.Infrastructure.Data;
using UknfCommunicationPlatform.Infrastructure.Extensions;
using UknfCommunicationPlatform.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Infrastructure (Database)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings not found in configuration");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization with custom policies
builder.Services.AddAuthorization(options =>
{
    // Entity context policies
    options.AddPolicy("EntityContext_AllowInternal", policy =>
        policy.Requirements.Add(new EntityOwnershipRequirement(allowInternalUsers: true)));

    options.AddPolicy("EntityContext_Strict", policy =>
        policy.Requirements.Add(new EntityOwnershipRequirement(allowInternalUsers: false)));

    // Common permission policies
    options.AddPolicy("Permission_users.read", policy =>
        policy.Requirements.Add(new PermissionRequirement("users.read")));

    options.AddPolicy("Permission_users.write", policy =>
        policy.Requirements.Add(new PermissionRequirement("users.write")));

    options.AddPolicy("Permission_users.delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("users.delete")));

    options.AddPolicy("Permission_entities.read", policy =>
        policy.Requirements.Add(new PermissionRequirement("entities.read")));

    options.AddPolicy("Permission_entities.write", policy =>
        policy.Requirements.Add(new PermissionRequirement("entities.write")));

    options.AddPolicy("Permission_entities.delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("entities.delete")));

    options.AddPolicy("Permission_reports.read", policy =>
        policy.Requirements.Add(new PermissionRequirement("reports.read")));

    options.AddPolicy("Permission_reports.write", policy =>
        policy.Requirements.Add(new PermissionRequirement("reports.write")));

    options.AddPolicy("Permission_reports.create", policy =>
        policy.Requirements.Add(new PermissionRequirement("reports.create")));

    options.AddPolicy("Permission_messages.read", policy =>
        policy.Requirements.Add(new PermissionRequirement("messages.read")));

    options.AddPolicy("Permission_messages.write", policy =>
        policy.Requirements.Add(new PermissionRequirement("messages.write")));

    options.AddPolicy("Permission_cases.read", policy =>
        policy.Requirements.Add(new PermissionRequirement("cases.read")));

    options.AddPolicy("Permission_cases.write", policy =>
        policy.Requirements.Add(new PermissionRequirement("cases.write")));

    options.AddPolicy("Permission_cases.delete", policy =>
        policy.Requirements.Add(new PermissionRequirement("cases.delete")));

    // Role-based policies
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("InternalUsersOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Administrator") ||
            context.User.IsInRole("InternalUser") ||
            context.User.IsInRole("Supervisor")));
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "UKNF Communication Platform API",
        Version = "v1",
        Description = "REST API for secure communication between UKNF and supervised entities",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "UKNF",
            Email = "contact@uknf.gov.pl"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
    });

    // Use operation filter to respect [AllowAnonymous] attributes
    options.OperationFilter<UknfCommunicationPlatform.Api.Swagger.AuthorizeCheckOperationFilter>();

    // Include XML comments for Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Map IFormFile to file upload in Swagger
    options.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // Add file upload support for Swagger
    options.OperationFilter<UknfCommunicationPlatform.Api.Filters.FileUploadOperationFilter>();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UKNF Communication Platform API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

// Auto-apply migrations in development (but not in Testing environment)
if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        // Seed database with sample data
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        var seeder = new DatabaseSeeder(dbContext, passwordHasher, logger);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        // Log but don't fail - migrations might fail for valid reasons in dev
        Console.WriteLine($"Warning: Migration/Seeding failed: {ex.Message}");
    }
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
