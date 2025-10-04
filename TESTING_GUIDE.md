# Testing Your PostgreSQL Setup

## Quick Verification

Run the verification script:
```bash
chmod +x verify-setup.sh
./verify-setup.sh
```

## Manual Verification Steps

### 1. Check Running Containers

```bash
# View all running containers
docker-compose ps

# You should see:
# - postgres (healthy)
# - backend (running)
# - frontend (running)
```

Expected output:
```
NAME                  IMAGE               STATUS              PORTS
postgres              postgres:16-alpine  Up (healthy)        0.0.0.0:5432->5432/tcp
backend               backend:latest      Up                  0.0.0.0:5000->8080/tcp
frontend              frontend:latest     Up                  0.0.0.0:4200->80/tcp
```

### 2. Test PostgreSQL Connection

```bash
# Connect to PostgreSQL CLI
docker-compose exec postgres psql -U uknf_user -d uknf_db
```

Inside the PostgreSQL shell, try these commands:
```sql
-- Check version
SELECT version();

-- List databases
\l

-- List tables (empty initially)
\dt

-- Create a test table
CREATE TABLE test_table (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    created_at TIMESTAMP DEFAULT NOW()
);

-- Insert test data
INSERT INTO test_table (name) VALUES ('Test Entry 1'), ('Test Entry 2');

-- Query test data
SELECT * FROM test_table;

-- Drop test table
DROP TABLE test_table;

-- Exit
\q
```

### 3. Check PostgreSQL Logs

```bash
# View PostgreSQL logs
docker-compose logs postgres

# Follow logs in real-time
docker-compose logs -f postgres
```

Look for:
- `database system is ready to accept connections`
- No error messages

### 4. Test Backend API

```bash
# Test the weather forecast endpoint
curl http://localhost:5000/weatherforecast

# Or open in browser:
# http://localhost:5000/weatherforecast
```

Expected: JSON response with weather data

### 5. Test Frontend

Open your browser and navigate to:
- **http://localhost:4200**

You should see the Angular application running.

### 6. Check Database Connection from Backend

```bash
# View backend logs
docker-compose logs backend

# Follow backend logs
docker-compose logs -f backend
```

Look for:
- Application startup messages
- No database connection errors

### 7. Test Connection with External Tools

#### Using psql (if installed locally)
```bash
psql -h localhost -p 5432 -U uknf_user -d uknf_db
# Password: YourStrong@Passw0rd
```

#### Using DBeaver
1. Download DBeaver: https://dbeaver.io/
2. Create new PostgreSQL connection:
   - Host: `localhost`
   - Port: `5432`
   - Database: `uknf_db`
   - Username: `uknf_user`
   - Password: `YourStrong@Passw0rd`

#### Using pgAdmin
1. Download pgAdmin: https://www.pgadmin.org/
2. Add new server:
   - Host: `localhost`
   - Port: `5432`
   - Username: `uknf_user`
   - Password: `YourStrong@Passw0rd`
   - Database: `uknf_db`

#### Using Azure Data Studio
1. Install PostgreSQL extension
2. Create new connection:
   - Server: `localhost`
   - Port: `5432`
   - User: `uknf_user`
   - Password: `YourStrong@Passw0rd`
   - Database: `uknf_db`

### 8. Test with Entity Framework Core

Once you've configured your DbContext in `Program.cs`, test migrations:

```bash
# Navigate to API project
cd src/Backend/UknfCommunicationPlatform.Api

# Create initial migration
dotnet ef migrations add InitialCreate --project ../UknfCommunicationPlatform.Infrastructure

# Apply migration to database
dotnet ef database update --project ../UknfCommunicationPlatform.Infrastructure

# Verify in PostgreSQL
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "\dt"
```

You should see the `__EFMigrationsHistory` table created.

### 9. Performance Test

```bash
# Check PostgreSQL statistics
docker-compose exec postgres psql -U uknf_user -d uknf_db -c "
SELECT 
    datname as database,
    numbackends as connections,
    xact_commit as commits,
    xact_rollback as rollbacks,
    blks_read as blocks_read,
    blks_hit as blocks_hit
FROM pg_stat_database 
WHERE datname = 'uknf_db';
"
```

### 10. Check Resource Usage

```bash
# Check Docker container stats
docker stats

# Look for:
# - postgres CPU/Memory usage
# - backend CPU/Memory usage
# - frontend CPU/Memory usage
```

## Common Success Indicators

✅ **PostgreSQL is working if:**
- Container status shows "Up (healthy)"
- `pg_isready` returns "accepting connections"
- You can connect via psql or GUI tools
- No errors in `docker-compose logs postgres`
- Backend can connect (no connection errors in logs)

✅ **Backend is working if:**
- Returns JSON from `/weatherforecast` endpoint
- No startup errors in logs
- Can communicate with PostgreSQL

✅ **Frontend is working if:**
- Page loads at http://localhost:4200
- No console errors in browser

## Quick Health Check Commands

```bash
# One-liner health check
docker-compose exec postgres pg_isready -U uknf_user -d uknf_db && echo "✅ PostgreSQL is healthy!"

# Check if backend is responding
curl -f http://localhost:5000/weatherforecast && echo "✅ Backend is responding!"

# Check frontend
curl -f http://localhost:4200 && echo "✅ Frontend is responding!"

# All in one
docker-compose ps && \
docker-compose exec postgres pg_isready -U uknf_user -d uknf_db && \
curl -sf http://localhost:5000/weatherforecast > /dev/null && \
echo "🎉 Everything is working!"
```

## Troubleshooting

### Container not healthy?
```bash
docker-compose logs postgres
docker-compose restart postgres
```

### Cannot connect to database?
```bash
# Check if port 5432 is open
sudo lsof -i :5432

# Check PostgreSQL is listening
docker-compose exec postgres netstat -tulpn | grep 5432
```

### Backend cannot connect?
```bash
# Check connection string in docker-compose.yml
docker-compose config

# Check backend logs
docker-compose logs backend
```

### Need to reset everything?
```bash
# Stop and remove volumes
docker-compose down -v

# Rebuild and start
docker-compose up --build
```

## What's Next?

Once everything is verified:

1. **Configure your entities** in the Core project
2. **Add DbSets** to ApplicationDbContext
3. **Create migrations** for your data model
4. **Build your API** endpoints
5. **Connect frontend** to backend

See `POSTGRESQL_MIGRATION.md` for detailed EF Core usage examples.
