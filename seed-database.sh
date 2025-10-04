#!/bin/bash

# Database Seeder Script
# This script runs the database seeder to populate the database with sample data

set -e

echo "🌱 Starting database seeding..."
echo ""

# Check if PostgreSQL container is running
if ! docker ps | grep -q uknf-postgres-dev; then
    echo "❌ PostgreSQL container is not running. Please start it first with ./dev-start.sh"
    exit 1
fi

# Check if backend container is running
if ! docker ps | grep -q uknf-backend-dev; then
    echo "❌ Backend container is not running. Please start it first with ./dev-start.sh"
    exit 1
fi

echo "✅ Containers are running"
echo ""

# The seeding happens automatically when the backend starts in development mode
# This script just restarts the backend to trigger the seeding process

echo "🔄 Restarting backend to trigger seeding..."
docker compose -f docker-compose.dev.yml restart backend || docker-compose -f docker-compose.dev.yml restart backend

echo "⏳ Waiting for backend to start and seed database..."
sleep 5

# Check backend logs for seeding confirmation
echo ""
echo "📋 Checking backend logs for seeding status..."
docker logs uknf-backend-dev 2>&1 | grep -i "seed" | tail -5 || echo "   (No seeding logs found - check if data already exists)"

echo ""
echo "✅ Database seeding process completed!"
echo ""
echo "📌 Default credentials:"
echo "   Admin:      admin@uknf.gov.pl / Admin123!"
echo "   Internal:   jan.kowalski@uknf.gov.pl / User123!"
echo "   Supervisor: anna.nowak@uknf.gov.pl / Supervisor123!"
echo "   External:   kontakt@pkobp.pl / External123!"
echo ""
echo "🔗 You can now login at: http://localhost:4200"
echo "📖 API Documentation: http://localhost:5000/swagger"
echo ""
