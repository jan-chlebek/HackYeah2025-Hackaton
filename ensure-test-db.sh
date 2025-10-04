#!/bin/bash

# Script to ensure PostgreSQL test database is available before running integration tests
# This script checks if PostgreSQL is running and starts it if needed

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# PostgreSQL connection settings
POSTGRES_HOST="${POSTGRES_HOST:-localhost}"
POSTGRES_PORT="${POSTGRES_PORT:-5432}"
POSTGRES_USER="${POSTGRES_USER:-uknf_user}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-uknf_password}"

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "  PostgreSQL Test Database Setup"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

# Function to check if PostgreSQL is accessible
check_postgres() {
    PGPASSWORD=$POSTGRES_PASSWORD psql -h $POSTGRES_HOST -p $POSTGRES_PORT -U $POSTGRES_USER -d postgres -c '\q' 2>/dev/null
    return $?
}

# Function to start PostgreSQL via docker-compose
start_postgres() {
    echo -e "${YELLOW}Starting PostgreSQL via docker-compose...${NC}"
    docker-compose -f docker-compose.dev.yml up -d postgres

    # Wait for PostgreSQL to be ready
    echo -e "${YELLOW}Waiting for PostgreSQL to be ready...${NC}"
    local retries=30
    local count=0

    while ! check_postgres; do
        count=$((count + 1))
        if [ $count -ge $retries ]; then
            echo -e "${RED}Failed to connect to PostgreSQL after $retries attempts${NC}"
            return 1
        fi
        echo -n "."
        sleep 1
    done
    echo ""
    echo -e "${GREEN}PostgreSQL is ready!${NC}"
    return 0
}

# Check if PostgreSQL is already running
if check_postgres; then
    echo -e "${GREEN}✓ PostgreSQL is already running on ${POSTGRES_HOST}:${POSTGRES_PORT}${NC}"
else
    echo -e "${YELLOW}PostgreSQL is not accessible on ${POSTGRES_HOST}:${POSTGRES_PORT}${NC}"

    # Try to start it via docker-compose
    if start_postgres; then
        echo -e "${GREEN}✓ PostgreSQL started successfully${NC}"
    else
        echo -e "${RED}✗ Failed to start PostgreSQL${NC}"
        echo ""
        echo "Please ensure either:"
        echo "  1. PostgreSQL is running locally on port 5432, or"
        echo "  2. Docker is installed and running for docker-compose"
        echo ""
        echo "You can also set environment variables:"
        echo "  export POSTGRES_HOST=your_host"
        echo "  export POSTGRES_PORT=your_port"
        echo "  export POSTGRES_USER=your_user"
        echo "  export POSTGRES_PASSWORD=your_password"
        exit 1
    fi
fi

echo ""
echo -e "${GREEN}PostgreSQL test environment is ready!${NC}"
echo ""
echo "Connection details:"
echo "  Host: $POSTGRES_HOST"
echo "  Port: $POSTGRES_PORT"
echo "  User: $POSTGRES_USER"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
