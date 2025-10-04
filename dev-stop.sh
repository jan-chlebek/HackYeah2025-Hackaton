#!/bin/bash

# Stop all development containers

echo "🛑 Stopping UKNF Development Environment..."
echo ""

# Check if any containers are running
RUNNING=$(docker-compose -f docker-compose.dev.yml ps -q 2>/dev/null | wc -l)

if [ "$RUNNING" -eq 0 ]; then
    echo "ℹ️  No containers are currently running"
else
    echo "Stopping services:"
    echo "   🔵 Backend (uknf-backend-dev)"
    echo "   🟢 Frontend (uknf-frontend-dev)"
    echo "   🟣 PostgreSQL (uknf-postgres-dev)"
    echo ""

    docker-compose -f docker-compose.dev.yml down

    echo ""
    echo "✅ All services stopped successfully"
fi

echo ""
echo "💾 Database data preserved in Docker volume"
echo "💡 To restart: ./dev-start.sh"
echo "🗑️  To remove all data: docker-compose -f docker-compose.dev.yml down -v"
echo ""
