# 🔥 Auto-Reload Development Setup

This document explains how to run the UKNF platform with automatic reloading on code changes.

## Quick Start

```bash
# Make scripts executable (first time only)
chmod +x dev-start.sh dev-stop.sh

# Start development environment with hot reload
./dev-start.sh

# In another terminal, view logs
docker-compose -f docker-compose.dev.yml logs -f backend

# Stop development environment
./dev-stop.sh
```

## What's Different in Dev Mode?

### Backend (ASP.NET Core)
- Uses `dotnet watch run` for automatic compilation on file changes
- Source code is mounted as a volume (`./backend:/app`)
- Changes to `.cs` files trigger automatic rebuild and restart
- Swagger UI updates automatically
- EF Core migrations can be run inside the container

### Frontend (Angular)
- Uses `npm start` (ng serve) with hot module replacement
- Source code is mounted as a volume
- Changes to `.ts`, `.html`, `.scss` files update instantly
- No page refresh needed for most changes

### Database (PostgreSQL)
- Data persists in Docker volume `postgres_data`
- Accessible on `localhost:5432`

## Architecture

```
┌─────────────────────────────────────────────┐
│  Host Machine (Your Code Editor)           │
│  - Edit files in backend/              │
│  - Changes detected by file watcher        │
└──────────────────┬──────────────────────────┘
                   │ Volume Mount
                   ▼
┌─────────────────────────────────────────────┐
│  Docker Container: backend                  │
│  - dotnet watch run monitors files          │
│  - Auto-compiles on change                  │
│  - Restarts application                     │
└─────────────────────────────────────────────┘
```

## Development Workflow

### 1. Start Environment
```bash
./dev-start.sh
```

This will:
1. Stop any existing containers
2. Start PostgreSQL and wait for it to be ready
3. Build and start backend with hot reload enabled
4. Show live logs in the terminal

### 2. Make Code Changes

**Backend (C#):**
- Edit any `.cs` file in `backend/`
- Save the file
- Watch the terminal - you'll see:
  ```
  watch : File changed: /app/UknfCommunicationPlatform.Api/Controllers/v1/ReportsController.cs
  watch : Building...
  watch : Build succeeded
  watch : Started
  ```
- API restarts automatically (takes 2-5 seconds)

**Frontend (TypeScript/Angular):**
- Edit any `.ts`, `.html`, or `.scss` file
- Save the file
- Browser updates automatically

### 3. View Logs

In a separate terminal:
```bash
# Backend logs
docker-compose -f docker-compose.dev.yml logs -f backend

# Database logs
docker-compose -f docker-compose.dev.yml logs -f postgres

# All logs
docker-compose -f docker-compose.dev.yml logs -f
```

### 4. Access Services

- **Backend API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger
- **Frontend:** http://localhost:4200
- **PostgreSQL:** localhost:5432

### 5. Database Migrations

Run migrations inside the container:
```bash
# Enter backend container
docker exec -it uknf-backend-dev bash

# Add migration
dotnet ef migrations add MigrationName --project ../UknfCommunicationPlatform.Infrastructure

# Apply migrations
dotnet ef database update --project ../UknfCommunicationPlatform.Infrastructure

# Exit container
exit
```

Or from host (if you have dotnet-ef installed):
```bash
cd backend/UknfCommunicationPlatform.Api
dotnet ef migrations add MigrationName --project ../UknfCommunicationPlatform.Infrastructure
```

## Troubleshooting

### Backend doesn't reload
1. Check volume mounts: `docker inspect uknf-backend-dev`
2. Ensure files are saved (not just in editor buffer)
3. Check logs: `docker logs -f uknf-backend-dev`
4. Rebuild: `docker-compose -f docker-compose.dev.yml up --build backend`

### "Port already in use"
```bash
# Find process using port 5000
lsof -i :5000

# Kill it (replace PID)
kill -9 <PID>

# Or use different port in docker-compose.dev.yml
ports:
  - "5001:8080"  # Use 5001 instead
```

### Changes not detected
```bash
# Increase file watch limit (Linux)
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Or set polling mode (already enabled in docker-compose.dev.yml)
DOTNET_USE_POLLING_FILE_WATCHER=true
```

### Container build fails
```bash
# Clean everything and rebuild
docker-compose -f docker-compose.dev.yml down -v
docker system prune -f
./dev-start.sh
```

### Database connection errors
```bash
# Reset database
docker-compose -f docker-compose.dev.yml down -v
docker volume rm hackyeah2025-hackaton_postgres_data
./dev-start.sh
```

## Production Deployment

For production, use the standard docker-compose.yml:
```bash
docker-compose up --build -d
```

This uses optimized Dockerfiles without hot reload and volume mounts.

## File Watching Configuration

The development setup uses:

**Backend:**
- `dotnet watch run` - Built-in ASP.NET Core file watcher
- Monitors: `*.cs`, `*.csproj`, `*.json`, `*.cshtml`
- Triggers: Compilation + restart

**Frontend:**
- `ng serve --host 0.0.0.0` - Angular CLI dev server
- Hot Module Replacement (HMR) enabled
- Monitors: All source files
- Triggers: Instant browser update

**Environment Variables:**
- `DOTNET_USE_POLLING_FILE_WATCHER=true` - Cross-platform file watching
- `ASPNETCORE_ENVIRONMENT=Development` - Dev mode behaviors

## Performance Tips

1. **Use SSD** - File watching is I/O intensive
2. **Exclude directories** - obj/bin/node_modules already excluded
3. **Increase resources** - Docker Desktop: 4GB RAM minimum
4. **Use WSL2** (Windows) - Better file system performance
5. **Close unused apps** - Free up system resources

## Next Steps

- [ ] Add Redis for session caching
- [ ] Add message queue (RabbitMQ/Kafka) for background jobs
- [ ] Add health check endpoints monitoring
- [ ] Set up multi-stage debugging in VS Code
- [ ] Configure HTTPS certificates for local dev

## Related Files

- `docker-compose.dev.yml` - Development compose file
- `backend/Dockerfile.dev` - Development backend image
- `dev-start.sh` - Start script
- `dev-stop.sh` - Stop script

---

**Last Updated:** 2025-10-04  
**Tested On:** Docker 24.x, .NET 9.0, Angular 20
