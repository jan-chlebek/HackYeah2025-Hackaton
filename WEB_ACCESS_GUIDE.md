# 🌐 UKNF Platform - Web Access Guide

## Quick Access URLs

Once the services are running, you can access them from your web browser:

### 🔵 Backend API (ASP.NET Core)

| Service | URL | Description |
|---------|-----|-------------|
| **Swagger UI** | http://localhost:5000/swagger | Interactive API documentation |
| **API Base** | http://localhost:5000/api/v1/ | REST API endpoints |
| **Health Check** | http://localhost:5000/health | Service health status (if configured) |

**Example API Endpoints:**
- `GET` http://localhost:5000/api/v1/reports - List all reports
- `GET` http://localhost:5000/api/v1/reports/{id} - Get specific report
- `POST` http://localhost:5000/api/v1/reports - Submit new report

### 🟢 Frontend (Angular SPA)

| Service | URL | Description |
|---------|-----|-------------|
| **Web Application** | http://localhost:4200 | Main user interface |

### 🟣 Database (PostgreSQL)

| Service | Connection | Description |
|---------|------------|-------------|
| **PostgreSQL** | `localhost:5432` | Database server (not web accessible) |

**Connection Details:**
- Host: `localhost`
- Port: `5432`
- Database: `uknf_db`
- Username: `uknf_user`
- Password: `uknf_password`

---

## 🚀 Starting the Services

### Option 1: Development Mode (Hot Reload)

```bash
# Start with auto-reload enabled
./dev-start.sh
```

**Services will be available at:**
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger ⭐ **Start here!**
- Frontend: http://localhost:4200

### Option 2: Production Mode

```bash
# Start optimized build
docker-compose up -d

# View logs
docker-compose logs -f
```

**Same URLs as development mode.**

---

## 🎯 Recommended First Steps

### 1. **Start the Backend** (Most Important First)

```bash
./dev-start.sh
```

Wait for this message:
```
✅ Backend started successfully
Now listening on: http://0.0.0.0:8080
```

### 2. **Open Swagger UI**

Open your browser and go to:
```
http://localhost:5000/swagger
```

You should see the interactive API documentation with all endpoints.

### 3. **Test the API**

In Swagger UI:
1. Find the `GET /api/v1/reports` endpoint
2. Click "Try it out"
3. Click "Execute"
4. See the response (initially empty list `[]`)

### 4. **Open the Frontend** (If Available)

```
http://localhost:4200
```

> **Note:** Frontend UI may not be fully implemented yet. Swagger is your main interface for now.

---

## 📊 Service Status Check

### Check if Services are Running

```bash
# List all containers
docker-compose -f docker-compose.dev.yml ps

# Or
docker ps
```

**Expected output:**
```
NAME                 STATUS              PORTS
uknf-postgres-dev    Up 2 minutes        5432:5432
uknf-backend-dev     Up 1 minute         5000:8080
uknf-frontend-dev    Up 1 minute         4200:4200
```

### Check Backend Logs

```bash
# Follow backend logs
docker-compose -f docker-compose.dev.yml logs -f backend

# Or via VS Code: Ctrl+Shift+P → "Tasks: Run Task" → "View Backend Logs"
```

Look for:
```
✅ Application started. Press Ctrl+C to shut down.
Now listening on: http://0.0.0.0:8080
```

### Test Backend Connectivity

```bash
# Quick health check
curl http://localhost:5000/api/v1/reports

# Expected response (empty array initially):
# []
```

---

## 🔍 Troubleshooting Access Issues

### "Cannot Connect" or "Connection Refused"

**1. Check if containers are running:**
```bash
docker ps | grep uknf
```

**2. If not running, start them:**
```bash
./dev-start.sh
```

**3. Check for port conflicts:**
```bash
# Check if port 5000 is already in use
lsof -i :5000

# Check if port 4200 is already in use
lsof -i :4200
```

**4. Kill conflicting processes:**
```bash
# Replace <PID> with the process ID from lsof
kill -9 <PID>
```

### "404 Not Found" on Swagger

**Swagger URL is case-sensitive:**
- ✅ Correct: http://localhost:5000/swagger
- ❌ Wrong: http://localhost:5000/Swagger

**Check if backend is ready:**
```bash
docker logs uknf-backend-dev --tail 50
```

Look for startup completion message.

### Backend Shows Errors

**View full logs:**
```bash
docker-compose -f docker-compose.dev.yml logs backend
```

**Common issues:**
- Database not ready → Wait 10-20 seconds for PostgreSQL health check
- Migration errors → Run `docker-compose down -v` and restart
- Build errors → Check `docker-compose -f docker-compose.dev.yml up --build backend`

### Frontend Not Loading

**Check if Angular dev server started:**
```bash
docker logs uknf-frontend-dev
```

Look for:
```
✔ Browser application bundle generation complete.
Local: http://localhost:4200/
```

**Frontend build might take 1-2 minutes on first start.**

---

## 🎨 Using Swagger UI

### What is Swagger UI?

Swagger UI is an interactive API documentation interface where you can:
- See all available endpoints
- Read request/response schemas
- Test endpoints directly from browser
- View example data

### How to Use It

**1. Navigate to Swagger:**
```
http://localhost:5000/swagger
```

**2. Explore Endpoints:**
- Expand any endpoint (e.g., `GET /api/v1/reports`)
- Click "Try it out"
- Modify parameters if needed
- Click "Execute"
- View the response

**3. Submit a Report (POST):**
- Find `POST /api/v1/reports`
- Click "Try it out"
- Upload an Excel file
- Fill in required fields:
  - `reportingPeriod`: "Q1 2025"
  - `reportType`: "Quarterly Financial Report"
  - `isCorrection`: false
- Click "Execute"
- Get back a report ID

**4. Retrieve Report (GET):**
- Find `GET /api/v1/reports/{id}`
- Click "Try it out"
- Enter the ID from previous step
- Click "Execute"
- See full report details

---

## 🔐 Authentication (Coming Soon)

Currently, the API is open (no authentication required for development).

**When JWT is implemented, you'll need to:**
1. POST credentials to `/api/v1/auth/login`
2. Copy the JWT token
3. Click "Authorize" button in Swagger
4. Paste token
5. All requests will include authentication

---

## 📱 Access from Other Devices (Same Network)

### From Phone/Tablet on Same WiFi

**1. Find your computer's IP address:**
```bash
# Linux
ip addr show | grep "inet " | grep -v 127.0.0.1

# Output example: inet 192.168.1.100/24
```

**2. Access from mobile device:**
- Backend: http://192.168.1.100:5000/swagger
- Frontend: http://192.168.1.100:4200

**3. Update CORS (if needed):**

If you get CORS errors, update `backend/UknfCommunicationPlatform.Api/Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "http://192.168.1.100:4200"  // Add your IP
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
```

---

## 📋 Summary - Your Access URLs

**Copy-paste these into your browser:**

```
🔵 Backend Swagger UI (API Docs):
   http://localhost:5000/swagger

🔵 Backend API Endpoint:
   http://localhost:5000/api/v1/reports

🟢 Frontend Web App:
   http://localhost:4200

🟣 Database (pgAdmin/DBeaver):
   Host: localhost
   Port: 5432
   Database: uknf_db
   User: uknf_user
   Password: uknf_password
```

---

## 🎬 Quick Demo

**Complete workflow in browser:**

1. **Start services:**
   ```bash
   ./dev-start.sh
   ```

2. **Open Swagger:**
   - Browser: http://localhost:5000/swagger

3. **Get all reports:**
   - Endpoint: `GET /api/v1/reports`
   - Click "Try it out" → "Execute"
   - Response: `[]` (empty initially)

4. **Submit a report:**
   - Endpoint: `POST /api/v1/reports`
   - Upload Excel file
   - Fill form fields
   - Execute
   - Get report ID in response

5. **Verify submission:**
   - Endpoint: `GET /api/v1/reports`
   - Execute again
   - See your report in the list!

---

**Need help?** Check logs:
```bash
docker-compose -f docker-compose.dev.yml logs -f
```

**Everything clear?** Open http://localhost:5000/swagger and start exploring! 🚀
