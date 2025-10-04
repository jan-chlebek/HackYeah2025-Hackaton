# UKNF Communication Platform

## Quick Start

```bash
# Start all services
docker-compose up --build

# Access:
# Frontend: http://localhost:4200
# Backend API: http://localhost:5000/swagger
# PostgreSQL: localhost:5432 (uknf_user / YourStrong@Passw0rd)
```

## Development

### Backend
```bash
cd src/Backend/UknfCommunicationPlatform.Api
dotnet run
```

### Frontend
```bash
cd src/Frontend/uknf-portal
ng serve
```
