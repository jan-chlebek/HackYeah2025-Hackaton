#!/bin/bash

# UKNF Development Environment - Auto-reload Setup
# This script starts the development environment with hot reload enabled

set -e

echo "🚀 Starting UKNF Development Environment with Hot Reload..."
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Stop any existing containers
echo "🧹 Cleaning up existing containers..."
docker-compose -f docker-compose.dev.yml down

# Build and start PostgreSQL
echo "🔨 Building and starting PostgreSQL..."
docker-compose -f docker-compose.dev.yml up -d postgres

echo "⏳ Waiting for PostgreSQL to be ready..."
until docker-compose -f docker-compose.dev.yml exec -T postgres pg_isready -U uknf_user -d uknf_db > /dev/null 2>&1; do
    echo "   Waiting for database..."
    sleep 2
done
echo "✅ PostgreSQL is ready!"

# Build and start backend with hot reload
echo "🔨 Building and starting backend (hot reload enabled)..."
docker-compose -f docker-compose.dev.yml up -d --build backend

echo "⏳ Waiting for backend to start..."
sleep 5

# Check if backend is responding
until curl -s http://localhost:5000/health > /dev/null 2>&1; do
    echo "   Waiting for backend..."
    sleep 2
done
echo "✅ Backend is ready!"

# Build and start frontend with hot reload
echo "🔨 Building and starting frontend (hot reload enabled)..."
docker-compose -f docker-compose.dev.yml up -d --build frontend

echo "⏳ Waiting for frontend to compile..."
echo "   This may take 20-30 seconds on first start..."
sleep 15

# Check if frontend is responding
FRONTEND_READY=false
for i in {1..10}; do
    if curl -s http://localhost:4200 > /dev/null 2>&1; then
        FRONTEND_READY=true
        break
    fi
    echo "   Still compiling... ($i/10)"
    sleep 3
done

if [ "$FRONTEND_READY" = true ]; then
    echo "✅ Frontend is ready!"
else
    echo "⚠️  Frontend is still starting (this is normal)"
    echo "   Check logs: docker logs -f uknf-frontend-dev"
fi

echo ""
echo "=========================================="
echo "🎉 UKNF Platform is Running!"
echo "=========================================="
echo ""
echo "📍 Access URLs:"
echo "   🔵 Backend Swagger: http://localhost:5000/swagger"
echo "   🔵 Backend Health:  http://localhost:5000/health"
echo "   🔵 Backend API:     http://localhost:5000/api/v1/"
echo "   🟢 Frontend:        http://localhost:4200"
echo "   🟣 PostgreSQL:      localhost:5432"
echo ""
echo "📊 View logs:"
echo "   All:      docker-compose -f docker-compose.dev.yml logs -f"
echo "   Backend:  docker logs -f uknf-backend-dev"
echo "   Frontend: docker logs -f uknf-frontend-dev"
echo ""
echo "🛑 Stop services:"
echo "   ./dev-stop.sh"
echo ""
echo "=========================================="

