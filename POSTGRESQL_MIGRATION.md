# PostgreSQL Migration Guide

## Summary of Changes

This project has been migrated from Microsoft SQL Server to PostgreSQL. Below are all the changes made:

## Docker Configuration

### docker-compose.yml
- **Database Service**: Changed from `sqlserver` to `postgres`
  - Image: `mcr.microsoft.com/mssql/server:2022-latest` → `postgres:16-alpine`
  - Port: `1433` → `5432`
  - Volume: `sqldata` → `pgdata`
  - Environment variables updated for PostgreSQL
  - Added healthcheck for PostgreSQL

### Connection String
- **Old (MSSQL)**: `Server=sqlserver;Database=UknfDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True`
- **New (PostgreSQL)**: `Host=postgres;Database=uknf_db;Username=uknf_user;Password=YourStrong@Passw0rd`

## Backend Configuration

### NuGet Packages Added
Added to `UknfCommunicationPlatform.Infrastructure.csproj`:
- `Npgsql.EntityFrameworkCore.PostgreSQL` - EF Core provider for PostgreSQL
- `Microsoft.EntityFrameworkCore.Design` - EF Core design-time components

### New Files Created

1. **ApplicationDbContext.cs**
   - Location: `src/Backend/UknfCommunicationPlatform.Infrastructure/Data/ApplicationDbContext.cs`
   - Purpose: Entity Framework Core DbContext for PostgreSQL

2. **ServiceCollectionExtensions.cs**
   - Location: `src/Backend/UknfCommunicationPlatform.Infrastructure/Extensions/ServiceCollectionExtensions.cs`
   - Purpose: Dependency injection extensions for infrastructure services

### Configuration Files Updated

1. **appsettings.json** - Added connection string for PostgreSQL
2. **appsettings.Development.json** - Added development connection string

## Database Credentials

### PostgreSQL (New)
- **Host**: localhost (or `postgres` in Docker network)
- **Port**: 5432
- **Database**: uknf_db
- **Username**: uknf_user
- **Password**: YourStrong@Passw0rd

### SQL Server (Old - Removed)
- ~~Host: localhost~~
- ~~Port: 1433~~
- ~~Database: UknfDb~~
- ~~Username: sa~~
- ~~Password: YourStrong@Passw0rd~~

## Database Management Tools

### Recommended PostgreSQL Tools
- **pgAdmin** - Web-based GUI (https://www.pgadmin.org/)
- **DBeaver** - Universal database tool (https://dbeaver.io/)
- **Azure Data Studio** - With PostgreSQL extension
- **psql** - Command-line tool (included with PostgreSQL)

### Connection Examples

#### Using psql (Command Line)
```bash
# Connect to PostgreSQL in Docker
docker exec -it <container_name> psql -U uknf_user -d uknf_db

# Connect to local PostgreSQL
psql -h localhost -U uknf_user -d uknf_db
```

#### Using Docker Exec
```bash
# List running containers
docker ps

# Connect to PostgreSQL container
docker exec -it <postgres_container_id> bash

# Inside container, connect to database
psql -U uknf_user -d uknf_db
```

## Entity Framework Core Migrations

### Creating Migrations
```bash
# Navigate to the API project
cd src/Backend/UknfCommunicationPlatform.Api

# Add a new migration
dotnet ef migrations add InitialCreate --project ../UknfCommunicationPlatform.Infrastructure

# Update database
dotnet ef database update --project ../UknfCommunicationPlatform.Infrastructure
```

### Useful EF Core Commands
```bash
# List all migrations
dotnet ef migrations list --project ../UknfCommunicationPlatform.Infrastructure

# Remove last migration
dotnet ef migrations remove --project ../UknfCommunicationPlatform.Infrastructure

# Generate SQL script
dotnet ef migrations script --project ../UknfCommunicationPlatform.Infrastructure
```

## Using the Infrastructure in Program.cs

To use the PostgreSQL database in your API, add this to `Program.cs`:

```csharp
using UknfCommunicationPlatform.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services (including PostgreSQL)
builder.Services.AddInfrastructure(builder.Configuration);

// ... rest of your configuration

var app = builder.Build();

// ... rest of your app configuration

app.Run();
```

## Starting the Application

### Using Docker Compose
```bash
# Start all services (PostgreSQL, Backend, Frontend)
docker-compose up --build

# Start in detached mode
docker-compose up -d --build

# Stop all services
docker-compose down

# Stop and remove volumes (clean database)
docker-compose down -v
```

### Local Development

#### Start PostgreSQL Only
```bash
docker-compose up postgres
```

#### Run Backend Locally
```bash
cd src/Backend/UknfCommunicationPlatform.Api
dotnet run
```

## Key Differences: PostgreSQL vs SQL Server

### Syntax Differences
| Feature | SQL Server | PostgreSQL |
|---------|-----------|------------|
| Identity Column | `IDENTITY(1,1)` | `SERIAL` or `GENERATED ALWAYS AS IDENTITY` |
| String Concatenation | `+` | `\|\|` |
| Top N Records | `SELECT TOP N` | `SELECT ... LIMIT N` |
| Date Functions | `GETDATE()` | `NOW()` or `CURRENT_TIMESTAMP` |
| Case Sensitivity | Case-insensitive | Case-sensitive (by default) |

### Data Type Mappings
| .NET Type | SQL Server | PostgreSQL |
|-----------|-----------|------------|
| string | NVARCHAR | VARCHAR or TEXT |
| int | INT | INTEGER |
| long | BIGINT | BIGINT |
| decimal | DECIMAL | NUMERIC |
| DateTime | DATETIME2 | TIMESTAMP |
| bool | BIT | BOOLEAN |
| Guid | UNIQUEIDENTIFIER | UUID |

## Security Considerations

### Production Deployment
When deploying to production:

1. **Change Default Password**: Update `POSTGRES_PASSWORD` to a strong, unique password
2. **Use Environment Variables**: Store credentials in environment variables or secrets manager
3. **Enable SSL/TLS**: Configure PostgreSQL to require encrypted connections
4. **Restrict Network Access**: Use firewall rules to limit database access
5. **Regular Backups**: Implement automated backup strategy
6. **Update Connection String**: Use SSL parameters: `Host=postgres;Database=uknf_db;Username=uknf_user;Password=<strong_password>;SSL Mode=Require`

### Connection String Security
```csharp
// In production, use User Secrets or Azure Key Vault
// Development: appsettings.Development.json
// Production: Environment variables or secrets manager
```

## Troubleshooting

### Database Connection Issues
```bash
# Check if PostgreSQL container is running
docker ps

# Check PostgreSQL logs
docker logs <postgres_container_name>

# Test connection from backend container
docker exec -it <backend_container_name> bash
# Then try to ping postgres
ping postgres
```

### Port Already in Use
```bash
# Check what's using port 5432
sudo lsof -i :5432

# Or use docker-compose with different port
# Edit docker-compose.yml: "5433:5432"
```

### Data Persistence
```bash
# List Docker volumes
docker volume ls

# Inspect volume
docker volume inspect <volume_name>

# Remove volume (WARNING: deletes data)
docker volume rm <volume_name>
```

## Additional Resources

- [Npgsql Documentation](https://www.npgsql.org/efcore/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [EF Core with PostgreSQL](https://learn.microsoft.com/en-us/ef/core/providers/npgsql/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
