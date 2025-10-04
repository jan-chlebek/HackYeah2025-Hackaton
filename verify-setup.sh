#!/bin/bash
# Quick verification script for PostgreSQL setup

echo "═══════════════════════════════════════════════════════"
echo "  PostgreSQL Verification Script"
echo "═══════════════════════════════════════════════════════"
echo ""

# Check if containers are running
echo "📦 Checking Docker Containers..."
docker-compose ps
echo ""

# Check PostgreSQL health
echo "🏥 Checking PostgreSQL Health..."
docker-compose exec postgres pg_isready -U uknf_user -d uknf_db
echo ""

# Check PostgreSQL version
echo "📌 PostgreSQL Version:"
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "SELECT version();"
echo ""

# List databases
echo "💾 Available Databases:"
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "\l"
echo ""

# Check active connections
echo "🔗 Active Connections:"
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "SELECT datname, usename, application_name, client_addr FROM pg_stat_activity WHERE datname = 'uknf_db';"
echo ""

# List tables (if any exist)
echo "📋 Tables in Database:"
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "\dt"
echo ""

# Check backend API
echo "🌐 Checking Backend API..."
echo "Attempting to reach backend at http://localhost:5000..."
curl -s http://localhost:5000/weatherforecast | head -c 200
echo ""
echo ""

# Check frontend
echo "🎨 Checking Frontend..."
echo "Attempting to reach frontend at http://localhost:4200..."
curl -s -o /dev/null -w "HTTP Status: %{http_code}\n" http://localhost:4200
echo ""

echo "═══════════════════════════════════════════════════════"
echo "✅ Verification Complete!"
echo ""
echo "Access URLs:"
echo "  Frontend:  http://localhost:4200"
echo "  Backend:   http://localhost:5000/weatherforecast"
echo "  Swagger:   http://localhost:5000/swagger (if configured)"
echo ""
echo "Database Connection:"
echo "  Host:      localhost"
echo "  Port:      5432"
echo "  Database:  uknf_db"
echo "  Username:  uknf_user"
echo "  Password:  YourStrong@Passw0rd"
echo ""
echo "═══════════════════════════════════════════════════════"
