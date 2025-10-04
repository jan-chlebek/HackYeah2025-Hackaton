# Copilot Guide – Backend (ASP.NET Core)

## Tech baseline
- Runtime: **.NET 8/9** (`net8.0` target unless otherwise noted).
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
