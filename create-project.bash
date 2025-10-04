#!/bin/bash
# filepath: create-project.sh

set -e

PROJECT_NAME="UknfCommunicationPlatform"

echo "=== Creating Project Structure ==="

# Create solution
dotnet new sln -n $PROJECT_NAME

# Create backend projects
mkdir -p src/Backend
cd src/Backend

# API layer
dotnet new webapi -n ${PROJECT_NAME}.Api --framework net9.0
dotnet sln ../../${PROJECT_NAME}.sln add ${PROJECT_NAME}.Api

# Core layer (business logic)
dotnet new classlib -n ${PROJECT_NAME}.Core --framework net9.0
dotnet sln ../../${PROJECT_NAME}.sln add ${PROJECT_NAME}.Core

# Infrastructure layer (data access)
dotnet new classlib -n ${PROJECT_NAME}.Infrastructure --framework net9.0
dotnet sln ../../${PROJECT_NAME}.sln add ${PROJECT_NAME}.Infrastructure

# Add project references
dotnet add ${PROJECT_NAME}.Api reference ${PROJECT_NAME}.Core
dotnet add ${PROJECT_NAME}.Api reference ${PROJECT_NAME}.Infrastructure
dotnet add ${PROJECT_NAME}.Infrastructure reference ${PROJECT_NAME}.Core

cd ../..

# Create frontend project
mkdir -p src/Frontend
cd src/Frontend
ng new uknf-portal --standalone --routing --style=scss --skip-git=true

cd ../..

# Create Docker Compose file
cat > docker-compose.yml << 'EOF'
services:
  postgres:
    image: postgres:16-alpine
    environment:
      - POSTGRES_USER=uknf_user
      - POSTGRES_PASSWORD=YourStrong@Passw0rd
      - POSTGRES_DB=uknf_db
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    networks:
      - uknf-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U uknf_user -d uknf_db"]
      interval: 10s
      timeout: 5s
      retries: 5

  backend:
    build:
      context: ./src/Backend
      dockerfile: Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=uknf_db;Username=uknf_user;Password=YourStrong@Passw0rd
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - uknf-network

  frontend:
    build:
      context: ./src/Frontend/uknf-portal
      dockerfile: Dockerfile
    ports:
      - "4200:80"
    depends_on:
      - backend
    networks:
      - uknf-network

volumes:
  pgdata:

networks:
  uknf-network:
    driver: bridge
EOF

# Create backend Dockerfile
cat > src/Backend/Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["UknfCommunicationPlatform.Api/UknfCommunicationPlatform.Api.csproj", "UknfCommunicationPlatform.Api/"]
COPY ["UknfCommunicationPlatform.Core/UknfCommunicationPlatform.Core.csproj", "UknfCommunicationPlatform.Core/"]
COPY ["UknfCommunicationPlatform.Infrastructure/UknfCommunicationPlatform.Infrastructure.csproj", "UknfCommunicationPlatform.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "UknfCommunicationPlatform.Api/UknfCommunicationPlatform.Api.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/UknfCommunicationPlatform.Api"
RUN dotnet publish "UknfCommunicationPlatform.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "UknfCommunicationPlatform.Api.dll"]
EOF

# Create frontend Dockerfile
cat > src/Frontend/uknf-portal/Dockerfile << 'EOF'
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/uknf-portal/browser /usr/share/nginx/html
EXPOSE 80
EOF

# Create README
cat > README.md << 'EOF'
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
EOF

echo ""
echo "✅ Project structure created successfully!"
echo ""
echo "Next steps:"
echo "1. cd $PROJECT_NAME"
echo "2. docker-compose up --build"
echo "3. Open http://localhost:4200"
