# 🚀 Quick Start - Development with Auto-Reload

## Fastest Way to Start

```bash
./dev-start.sh
```

That's it! The backend will now auto-reload on every code change.

## What You Get

✅ **Backend auto-reload** - Save a `.cs` file, API restarts in ~3 seconds  
✅ **Frontend hot reload** - Save `.ts`/`.html`/`.scss`, browser updates instantly  
✅ **PostgreSQL** - Database ready on `localhost:5432`  
✅ **Swagger UI** - Interactive API docs at http://localhost:5000/swagger  

## Access URLs

| Service | URL |
|---------|-----|
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| Frontend | http://localhost:4200 |
| PostgreSQL | localhost:5432 |

## VS Code Tasks (Press `Ctrl+Shift+P` → "Run Task")

- **Start Dev Environment (Hot Reload)** ← Main task
- **Stop Dev Environment**
- **View Backend Logs**
- **View All Logs**
- **Open Swagger UI**
- **Database: Add Migration**
- **Database: Update**
- **Rebuild Backend (Dev)**
- **Clean Docker (Reset Everything)**

## Example Workflow

1. **Start environment:**
   ```bash
   ./dev-start.sh
   ```

2. **Edit code:**
   ```bash
   # Open your editor
   code src/Backend/UknfCommunicationPlatform.Api/Controllers/v1/ReportsController.cs
   
   # Make changes, save file
   # Watch terminal - API restarts automatically!
   ```

3. **Test in Swagger:**
   - Open http://localhost:5000/swagger
   - Try the endpoints
   - Changes appear immediately

4. **View logs (in another terminal):**
   ```bash
   docker-compose -f docker-compose.dev.yml logs -f backend
   ```

5. **Stop when done:**
   ```bash
   ./dev-stop.sh
   ```

## How It Works

**Magic Sauce:**
- Your source code is mounted into the Docker container as a volume
- `dotnet watch` monitors file changes inside the container
- When you save a file, it auto-compiles and restarts the app
- No manual `docker build` needed!

**Volume Mounts:**
```yaml
volumes:
  - ./src/Backend:/app:delegated  # Your code → Container
  - /app/**/obj                   # Exclude build artifacts
  - /app/**/bin
```

**File Watcher:**
```dockerfile
ENTRYPOINT ["dotnet", "watch", "run", ...]
```

## Troubleshooting

**"Port 5000 already in use":**
```bash
lsof -i :5000
kill -9 <PID>
```

**Changes not detected:**
```bash
# Already configured in docker-compose.dev.yml:
DOTNET_USE_POLLING_FILE_WATCHER=true
```

**Container won't start:**
```bash
./dev-stop.sh
docker system prune -f
./dev-start.sh
```

**Need fresh database:**
```bash
docker-compose -f docker-compose.dev.yml down -v
./dev-start.sh
```

## Performance Tips

⚡ **First start is slow** (2-3 min) - builds Docker image  
⚡ **Subsequent starts are fast** (~10 sec) - reuses image  
⚡ **Code changes** (2-5 sec) - just compilation  

## Production Deployment

For production (without hot reload):
```bash
docker-compose up --build -d
```

## Full Documentation

See [AUTO_RELOAD_GUIDE.md](.documentation/AUTO_RELOAD_GUIDE.md) for:
- Architecture details
- Database migrations
- Advanced troubleshooting
- Performance tuning

---

**Need help?** Check logs:
```bash
docker-compose -f docker-compose.dev.yml logs -f
```
