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
docker compose -f docker-compose.dev.yml down || docker-compose -f docker-compose.dev.yml down

# Build and start services
echo "🔨 Building and starting services..."
docker compose -f docker-compose.dev.yml up --build -d postgres || docker-compose -f docker-compose.dev.yml up --build -d postgres

echo "⏳ Waiting for PostgreSQL to be ready..."
sleep 5

echo "🔨 Starting backend with hot reload..."
docker compose -f docker-compose.dev.yml up --build backend || docker-compose -f docker-compose.dev.yml up --build backend

# Note: This will run in foreground with logs visible
# Press Ctrl+C to stop
