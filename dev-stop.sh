#!/bin/bash

# Stop all development containers

echo "🛑 Stopping UKNF Development Environment..."

docker-compose -f docker-compose.dev.yml down

echo "✅ Development environment stopped"
echo ""
echo "💡 To restart: ./dev-start.sh"
