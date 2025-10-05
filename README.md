# UKNF Communication Platform

A demo communication platform for UKNF covering Communication, Identity & Access, and Administration domains. Built with Angular 20 SPA + ASP.NET Core 9 REST API + PostgreSQL.

---

## 🚀 Quick Start (Development)

**One command to rule them all:**

```bash
./dev-start.sh
```

Access:
- 🌐 Frontend: http://localhost:4200
- 📡 Backend API: http://localhost:5000
- 📚 Swagger UI: http://localhost:5000/swagger
- 🗄️ PostgreSQL: localhost:5432 (database: `uknf_db`, user: `uknf_user`, password: `uknf_password`)

**Stop:**
```bash
./dev-stop.sh
```

---

## 🔨 Build Instructions

### Backend (ASP.NET Core 9)

```bash
cd backend/UknfCommunicationPlatform.Api
dotnet build
```

### Frontend (Angular 20)

```bash
cd frontend/uknf-project
npm install
npm run build
```

### Docker Build

```bash
# Development (hot reload enabled)
docker-compose -f docker-compose.dev.yml up --build

# Production
docker-compose up --build
```

---

## ✅ Running Tests

### All Tests (Unit + Integration)

```bash
./run-tests-backend.sh
```

**Expected output:** 306 tests (227 unit + 79 integration) passing in ~5-10 seconds.

### Unit Tests Only

```bash
./run-tests.sh --unit-only
```

### Integration Tests Only

```bash
./run-tests.sh --integration-only
```

### With Code Coverage

```bash
./run-tests.sh --coverage
```

---

## 🚢 Deployment

### Azure Deployment (Automated)

**Linux/macOS/WSL:**
```bash
chmod +x azure-deploy/deploy-to-azure.sh
./azure-deploy/deploy-to-azure.sh
```

**Windows (PowerShell):**
```powershell
.\azure-deploy\deploy-to-azure.ps1
```

**Manual Azure deployment:** See [azure-deploy/README.md](azure-deploy/README.md) for detailed instructions.

### Docker Production Deployment

```bash
docker-compose up -d
```

---

## 🔐 Test Credentials (Seeded Database)

Use these credentials for manual testing via Swagger UI or frontend:

| Role | Email | Password | Permissions |
|------|-------|----------|------------|
| **Administrator** | admin@uknf.gov.pl | Admin123! | 9 permissions (full access) |
| **Administrator** | k.administratorska@uknf.gov.pl | Admin123! | 9 permissions |
| **Internal User** | jan.kowalski@uknf.gov.pl | User123! | 4 permissions |
| **Internal User** | piotr.wisniewski@uknf.gov.pl | User123! | 4 permissions |
| **Internal User** | marek.dabrowski@uknf.gov.pl | User123! | 4 permissions |
| **Internal User** | tomasz.lewandowski@uknf.gov.pl | User123! | 4 permissions |
| **Internal User** | krzysztof.zielinski@uknf.gov.pl | User123! | 4 permissions |
| **Supervisor** | anna.nowak@uknf.gov.pl | Supervisor123! | 7 permissions |
| **Supervisor** | magdalena.szymanska@uknf.gov.pl | Supervisor123! | 7 permissions |
| **Supervisor** | michal.wozniak@uknf.gov.pl | Supervisor123! | 7 permissions |

---

## 🧪 Manual API Testing

### Using Swagger UI

1. Navigate to http://localhost:5000/swagger
2. Click **Authorize** button (top right)
3. Login with credentials:
   ```
   POST /api/v1/Auth/login
   {
     "email": "admin@uknf.gov.pl",
     "password": "Admin123!"
   }
   ```
4. Copy the `accessToken` from response
5. Paste into authorization dialog: `Bearer <your-token>`
6. Test protected endpoints

### Using curl

**Login:**
```bash
curl -X POST http://localhost:5000/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@uknf.gov.pl","password":"Admin123!"}'
```

**Get FAQs (protected endpoint):**
```bash
TOKEN="<your-access-token>"

curl -X GET http://localhost:5000/api/v1/faqs \
  -H "Authorization: Bearer $TOKEN"
```

**Refresh Token:**
```bash
curl -X POST http://localhost:5000/api/v1/Auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "accessToken": "<your-access-token>",
    "refreshToken": "<your-refresh-token>"
  }'
```

---

## 📖 API Documentation

- **Swagger UI:** http://localhost:5000/swagger (interactive API docs)
- **OpenAPI JSON:** http://localhost:5000/swagger/v1/swagger.json
- **Health Check:** http://localhost:5000/health

---

## 🛠️ Development Workflow

### Hot Reload (Auto-restart on code changes)

Development environment uses `dotnet watch` for automatic reload:

```bash
./dev-start.sh
# Edit files in backend/
# Changes auto-compile and restart
# Check logs: docker-compose -f docker-compose.dev.yml logs -f backend
```

### Database Migrations

**Add migration:**
```bash
docker exec -it uknf-backend-dev dotnet ef migrations add <MigrationName> \
  --project ../UknfCommunicationPlatform.Infrastructure
```

**Apply migrations:**
```bash
docker exec -it uknf-backend-dev dotnet ef database update \
  --project ../UknfCommunicationPlatform.Infrastructure
```

Or use VS Code tasks: `Database: Add Migration`, `Database: Update`

### Reset Database (Fresh Start)

```bash
docker-compose -f docker-compose.dev.yml down -v
./dev-start.sh
```

---

## 📁 Project Structure

```
.
├── backend/                          # ASP.NET Core 9 API
│   ├── UknfCommunicationPlatform.Api/           # Web API layer
│   ├── UknfCommunicationPlatform.Core/          # Domain models & interfaces
│   ├── UknfCommunicationPlatform.Infrastructure/ # Data access & services
│   ├── UknfCommunicationPlatform.Tests.Unit/    # Unit tests
│   └── UknfCommunicationPlatform.Tests.Integration/ # Integration tests
├── frontend/                         # Angular 20 SPA
│   └── uknf-project/
├── database/                         # Database scripts & docs
├── azure-deploy/                     # Azure deployment scripts
├── prompts/                          # AI prompt engineering history
├── dev-start.sh                      # Start development environment
├── dev-stop.sh                       # Stop development environment
├── run-tests-backend.sh              # Run all backend tests
└── docker-compose.dev.yml            # Development Docker config
```

---

## 📋 Additional Documentation

- **Quick Development Guide:** [QUICKSTART_DEV.md](QUICKSTART_DEV.md)
- **Web Access Guide:** [WEB_ACCESS_GUIDE.md](WEB_ACCESS_GUIDE.md)
- **Testing Guide:** [TESTING_GUIDE.md](TESTING_GUIDE.md)
- **Database Schema:** [database/SCHEMA_REFERENCE.md](database/SCHEMA_REFERENCE.md)
- **Azure Deployment:** [azure-deploy/README.md](azure-deploy/README.md)
- **Prompt Engineering Log:** [prompts.md](prompts.md)

---

## 🎯 Key Features Implemented

✅ **Authentication & Authorization:** JWT-based with roles & permissions  
✅ **Communication Module:** Messages, attachments, case folders  
✅ **FAQ Management:** CRUD operations with categories  
✅ **File Library:** Upload, download, metadata search  
✅ **Contact Registry:** Supervised entities management  
✅ **Admin Module:** User management, announcements, reports  
✅ **RESTful API:** OpenAPI/Swagger documented  
✅ **Database Seeding:** Test data pre-loaded  
✅ **Hot Reload:** Fast development iteration  
✅ **Docker Support:** Containerized environment  
✅ **Automated Tests:** 306 tests (unit + integration)

---

## 🔒 Security Notes

- **JWT Configuration:** HS256, 1-hour access tokens, Bearer scheme
- **HTTPS Only:** Production deployment assumes HTTPS
- **Secrets Management:** Never commit secrets; use `.env` files (see `azure-deploy/sample.env`)
- **Input Validation:** All endpoints validate inputs
- **Audit Trails:** User actions logged for compliance

---

## 🐛 Troubleshooting

**Port conflicts:**
```bash
lsof -i :5000  # Find process using port
kill -9 <PID>
```

**Docker issues:**
```bash
./dev-stop.sh
docker system prune -f
./dev-start.sh
```

**Tests failing:**
```bash
# Ensure test database exists
./ensure-test-db.sh

# Run tests with verbose output
./run-tests.sh --verbose
```

---

## 📝 License

This is a demo project created for HackYeah 2025 hackathon.

---

## 👥 Contributing

This project emphasizes **prompt engineering** - all AI interactions are logged in:
- `prompts.md` - Main prompt log
- `prompts/` - Individual prompt sessions

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for development guidelines.
