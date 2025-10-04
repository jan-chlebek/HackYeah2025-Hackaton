---
applyTo: "backend/**/*.cs,backend/**/*.csproj,backend/**/Dockerfile,backend/**/*.sql"
---

# Copilot Guide – Backend (ASP.NET Core)

## Tech baseline
- Runtime: **.NET 9** (`net9.0` target unless otherwise noted).
- Stack: **ASP.NET Core Web API** with minimal controllers, **Entity Framework Core** for data access, **FluentValidation** for request validation, **MediatR**/**CQRS** for application flows.
- Database: **SQL Server** (use Dockerised `mssql` locally). Keep schema migrations in `Backend/Infrastructure/Migrations` via `dotnet ef`.
- Background work: abstract queue integrations (Kafka/RabbitMQ) behind interfaces; provide in-memory stubs until infra is plugged in.

## Architectural conventions
- Solution layout: `Domain` (entities, value objects, events), `Application` (commands/queries, validators), `Infrastructure` (EF Core, queues, file storage), `Api` (controllers, filters, DI bootstrapping).
- Controllers live under `/api/v1/<resource>` and return ProblemDetails on errors. Support pagination metadata through `X-Pagination` headers and HAL-style links when possible.
- Use `IFileStorageService` abstraction with chunked upload endpoints (`POST /files/chunks`, `POST /files/complete`). Persist metadata, versions, permissions, and audit logs.
- Enforce OAuth2/OIDC + JWT. Integrate with IdentityServer/Entra ID mock; provide local dev issuer with signing keys stored in user secrets or `.env`.
- Instrument everything: structured logging (Serilog), OpenTelemetry traces/metrics, health checks (`/health/ready`, `/health/live`).

## Security & compliance
- Validate every input using FluentValidation + data annotations (defence-in-depth). Guard against XSS/CSRF/SQLi.
- Store secrets in configuration providers (user secrets, Key Vault placeholders). Never hardcode credentials.
- Implement audit trail middleware capturing user, action, entity, timestamp, and correlation IDs. Persist to dedicated table or event log.
- Support GDPR-friendly data retention (soft delete, purge jobs) and version history for entity changes (e.g., EF Core temporal tables).

## Testing & quality
- Unit tests with **xUnit** + **FluentAssertions**. Integration tests using **WebApplicationFactory** hitting in-memory or containerized SQL.
- Each feature requires: command handler test, controller test (happy + failure path), and data access test (if custom queries exist).
- Run `dotnet format`, `dotnet build`, `dotnet test` in CI. Capture coverage via `coverlet` and enforce thresholds.

## Delivery checklist
- Update `docker-compose.yml` + `Backend/Api/Dockerfile` when dependencies change. Ensure migrations run on container startup.
- Keep `swagger.json`/Swashbuckle docs fresh; describe auth schemes, pagination, error contracts, and chunked upload workflow.
- Seed reference data (roles, sample podmioty, FAQ categories) using EF migrations or `IHostedService` seeders behind feature flags.
- Document new configuration keys in `README.md` and provide `.env.example` updates.

---

## Functional Requirements

For detailed functional requirements by domain (Authentication, Communication, Administration, File Management, Background Jobs, Health Checks), see:

📄 **[Backend Functional Requirements](../requirements/backend-functional-requirements.md)**

This includes all API endpoints, request/response schemas, statuses, validation rules, and business logic specifications.

---

### Data Validation Rules

#### General Validation
- All inputs: XSS prevention, SQL injection guards (parameterized queries only)
- File uploads: MIME type validation, magic number checks, size limits
- Email: RFC 5322 format validation
- Dates: ISO 8601 format, range validation (not future for historical dates)

#### Polish-Specific Validators
- **PESEL**: 11-digit format, checksum validation, mask to show only last 4 digits in API responses
- **NIP**: 10-digit format, checksum validation
- **KRS**: 10-digit format
- **LEI**: 20-character alphanumeric ISO 17442 format
- **Phone**: International format pattern `/^\+(?:[0-9] ?){6,14}[0-9]$/`

#### FluentValidation Rules
Create validators for all command/query DTOs:
- `RegisterUserCommandValidator`
- `CreateAccessRequestCommandValidator`
- `UploadReportCommandValidator`
- `CreateCaseCommandValidator`
- `CreateAnnouncementCommandValidator`
- `UpdatePodmiotCommandValidator`
- etc.

---

### API Conventions

#### RESTful URL Structure
- `/api/v1/<resource>` for all endpoints
- Plural nouns for collections: `/api/v1/reports`, `/api/v1/messages`
- Nested resources for relationships: `/api/v1/cases/{id}/messages`
- Query parameters for filters, pagination, sorting

#### HTTP Methods
- **GET**: Retrieve resource(s), idempotent, cacheable
- **POST**: Create resource, non-idempotent
- **PUT**: Update resource (full replacement), idempotent
- **PATCH**: Partial update (use sparingly)
- **DELETE**: Remove resource, idempotent

#### Response Formats
- Success: `200 OK` (GET, PUT, PATCH), `201 Created` (POST with Location header), `204 No Content` (DELETE)
- Client errors: `400 Bad Request` (validation), `401 Unauthorized` (auth required), `403 Forbidden` (insufficient permissions), `404 Not Found`
- Server errors: `500 Internal Server Error`, `503 Service Unavailable`
- Use RFC 7807 ProblemDetails for error responses:
```json
{
  "type": "https://api.uknf.gov.pl/errors/validation-error",
  "title": "Validation Error",
  "status": 400,
  "detail": "PESEL format is invalid",
  "instance": "/api/v1/auth/register",
  "errors": {
    "PESEL": ["Must be 11 digits"]
  }
}
```

#### Pagination
- Use query parameters: `?page=1&pageSize=25`
- Return pagination metadata in `X-Pagination` header:
```json
{
  "currentPage": 1,
  "pageSize": 25,
  "totalCount": 150,
  "totalPages": 6,
  "hasPrevious": false,
  "hasNext": true
}
```
- Include HAL-style links in response body (optional):
```json
{
  "data": [...],
  "_links": {
    "self": "/api/v1/reports?page=1&pageSize=25",
    "next": "/api/v1/reports?page=2&pageSize=25"
  }
}
```

#### Filtering & Sorting
- Filters: `?status=Nowy&priority=Wysoki`
- Sorting: `?sortBy=createdDate&sortOrder=desc`
- Search: `?search=Instytucja+Pożyczkowa`

---

### Security Implementation

#### Authentication Flow
1. User registers → receives activation email
2. User activates account → sets password → receives JWT access token + refresh token
3. Access token expires after 15 minutes
4. Refresh token expires after 7 days
5. Client refreshes access token using refresh token before expiry
6. On logout: invalidate refresh token

#### JWT Claims
- `sub` (subject): User ID
- `email`: User email
- `roles`: Array of role names
- `podmiotId`: Current active podmiot (for external users)
- `iat` (issued at): Timestamp
- `exp` (expiration): Timestamp
- `jti` (JWT ID): Unique token identifier (for revocation)

#### CSRF Protection
- Include anti-forgery tokens in state-changing requests (POST, PUT, DELETE)
- Validate token in middleware
- Use `SameSite=Strict` cookie attribute for refresh tokens

#### Input Validation
- FluentValidation for command/query DTOs (business rules)
- Data Annotations for model binding (simple format checks)
- Defence-in-depth: validate at API boundary AND before database persistence

#### Secrets Management
- Store secrets in: User Secrets (dev), Azure Key Vault (production)
- Never hardcode: connection strings, API keys, signing keys, passwords
- Use configuration providers: `IConfiguration`, `IOptionsSnapshot<T>`

#### GDPR Compliance
- Soft delete for user data (set `IsDeleted` flag, preserve audit trail)
- Purge jobs for permanent deletion after retention period
- Data export endpoint: **GET** `/api/v1/users/{id}/export-data` (return all user data in JSON)
- Consent tracking for data processing (future enhancement)

---

### Testing Strategy

#### Unit Tests (xUnit + FluentAssertions)
- Test all command/query handlers (CQRS)
- Test validators (FluentValidation)
- Test domain entities and value objects
- Mock dependencies: `IRepository<T>`, `IFileStorageService`, `IMessageQueue`
- Coverage target: 80%+ for new code

#### Integration Tests (WebApplicationFactory)
- Test API endpoints end-to-end
- Use in-memory database or containerized SQL Server (Testcontainers)
- Test happy paths AND failure scenarios
- Test authentication/authorization (valid/invalid JWT, insufficient permissions)
- Test data access (custom queries, projections)

#### Test Data
- Use provided test data: podmioty CSV import, sample reports (Q1_2025.xlsx correct, Q2_2025.xlsx incorrect)
- Seed test database with: roles, sample podmioty, sample users, FAQ categories
- Use builders/factories for test entity creation

#### Example Test Structure
```csharp
public class CreateAccessRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesAccessRequestWithStatusRoboczy()
    {
        // Arrange
        var handler = new CreateAccessRequestCommandHandler(mockRepo, mockMediator);
        var command = new CreateAccessRequestCommand { UserId = 1, ... };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(AccessRequestStatus.Roboczy);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<AccessRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidPESEL_ThrowsValidationException()
    {
        // Arrange, Act, Assert
        ...
    }
}
```

---

### Database Schema Guidelines

#### Entity Framework Core
- Use Code-First migrations: `dotnet ef migrations add <name>`
- Configure entities with Fluent API in `DbContext.OnModelCreating`
- Use temporal tables for audit history (EF Core 6+):
```csharp
modelBuilder.Entity<Podmiot>()
    .ToTable("Podmioty", tb => tb.IsTemporal());
```

#### Naming Conventions
- Tables: PascalCase plural nouns (e.g., `Podmioty`, `AccessRequests`)
- Columns: PascalCase (e.g., `NazwaPodmiotu`, `CreatedDate`)
- Foreign keys: `<EntityName>Id` (e.g., `PodmiotId`, `UserId`)
- Indexes: `IX_<TableName>_<ColumnName>`

#### Soft Delete Pattern
- Add `IsDeleted` (bit) and `DeletedDate` (datetime2) columns to all entities
- Configure global query filter:
```csharp
modelBuilder.Entity<User>()
    .HasQueryFilter(u => !u.IsDeleted);
```

#### Audit Columns
- All entities: `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`
- Populate automatically in `SaveChangesAsync` override

---

### Sample API Endpoint Signatures

#### Authentication
```csharp
[HttpPost("api/v1/auth/register")]
public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    => CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);

[HttpPost("api/v1/auth/activate")]
public async Task<IActionResult> Activate([FromBody] ActivateAccountCommand command)
    => Ok(new { accessToken, refreshToken, expiresIn = 900 });
```

#### Access Requests
```csharp
[HttpGet("api/v1/access-requests")]
[Authorize]
public async Task<IActionResult> GetAccessRequests([FromQuery] GetAccessRequestsQuery query)
    => Ok(new { data = result.Items, _links = result.Links });

[HttpPost("api/v1/access-requests/{id}/approve")]
[Authorize(Roles = "Pracownik UKNF,Administrator Podmiotu")]
public async Task<IActionResult> ApproveAccessRequest(long id)
    => NoContent();
```

#### Reports
```csharp
[HttpPost("api/v1/reports")]
[Authorize]
[RequestSizeLimit(100_000_000)] // 100MB
public async Task<IActionResult> UploadReport([FromForm] UploadReportCommand command)
    => Accepted(new { reportId, validationJobId });

[HttpGet("api/v1/reports/{id}/validation-report")]
[Authorize]
public async Task<IActionResult> DownloadValidationReport(long id)
    => File(fileStream, "application/pdf", $"validation_report_{id}.pdf");
```

#### Messages
```csharp
[HttpPost("api/v1/messages")]
[Authorize]
public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand command)
    => CreatedAtAction(nameof(GetMessage), new { id = result.MessageId }, result);
```

#### Podmioty
```csharp
[HttpGet("api/v1/podmioty")]
[Authorize]
public async Task<IActionResult> GetPodmioty([FromQuery] GetPodmiotyQuery query)
    => Ok(new { data = result.Items, _links = result.Links });

[HttpPost("api/v1/podmioty/{id}/request-change")]
[Authorize(Roles = "Administrator Podmiotu,Pracownik Podmiotu")]
public async Task<IActionResult> RequestPodmiotDataChange(long id, [FromBody] RequestDataChangeCommand command)
    => Accepted(new { caseId = result.CaseId });
```

#### Admin
```csharp
[HttpPut("api/v1/admin/password-policy")]
[Authorize(Roles = "Administrator systemu")]
public async Task<IActionResult> UpdatePasswordPolicy([FromBody] UpdatePasswordPolicyCommand command)
    => NoContent();
```

---

### OpenAPI / Swagger Documentation

#### Swashbuckle Configuration
- Enable XML comments: `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Document all public endpoints with summary and remarks
- Include request/response examples
- Document authentication schemes (Bearer JWT)
- Group endpoints by tags (Auth, Reports, Messages, Cases, etc.)
- Generate `swagger.json` for client code generation

#### Example Documentation
```csharp
/// <summary>
/// Upload a new report (XLSX format)
/// </summary>
/// <remarks>
/// Uploads a report file for validation. The file must be in XLSX format
/// and follow the template downloaded from the Library.
/// After upload, an asynchronous validation job is triggered.
/// Status will transition: Robocze → Przekazane → W trakcie → Final status.
/// Maximum file size: 100MB.
/// </remarks>
/// <param name="command">Upload command with file and metadata</param>
/// <returns>Report ID and validation job ID</returns>
/// <response code="202">Accepted - validation job started</response>
/// <response code="400">Bad Request - invalid file format or size</response>
/// <response code="401">Unauthorized - JWT required</response>
[HttpPost("api/v1/reports")]
[ProducesResponseType(typeof(UploadReportResponse), StatusCodes.Status202Accepted)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UploadReport([FromForm] UploadReportCommand command) { ... }
```
