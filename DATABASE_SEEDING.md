# Database Seeding Guide

## Overview

The UKNF Communication Platform includes an automatic database seeding mechanism that populates the database with sample data for development and testing purposes.

## Automatic Seeding

When you start the development environment using `./dev-start.sh`, the database is automatically seeded with sample data **if the database is empty**. This happens automatically during backend startup.

## What Gets Seeded

### 1. Roles and Permissions
- **Roles:** Administrator, Internal User, Supervisor, External User
- **Permissions:** users.read/write/delete, entities.read/write, messages.read/write, reports.read/write, library.read/write
- **Role-Permission Mappings:** Each role is assigned appropriate permissions

### 2. Users
Default users with the following credentials:

| Role | Email | Password | Description |
|------|-------|----------|-------------|
| Administrator | admin@uknf.gov.pl | Admin123! | Full system access |
| Internal User | jan.kowalski@uknf.gov.pl | User123! | UKNF staff member |
| Supervisor | anna.nowak@uknf.gov.pl | Supervisor123! | UKNF supervisor |
| External User | kontakt@pkobp.pl | External123! | Bank representative |

### 3. Supervised Entities
Four sample supervised entities:
- **PKO Bank Polski S.A.** (BANK001)
- **Bank Pekao S.A.** (BANK002)
- **PZU S.A.** (INS001)
- **Nationale-Nederlanden TU** (INS002)

Each entity has an associated external user account.

### 4. Messages
Sample messages between internal and external users demonstrating:
- Request for quarterly report
- Report delivery confirmation
- Inspection schedule notification

### 5. FAQ Questions
Five frequently asked questions covering:
- Financial reporting frequency
- Report format requirements
- Message response times
- Password change procedure
- File upload restrictions

### 6. Announcements
Three sample announcements:
- New ESG reporting requirements (High priority)
- System maintenance notice (Normal priority)
- Online training invitation (Normal priority)

## Manual Seeding

If you need to re-seed the database (for example, after clearing it), you can use:

```bash
./seed-database.sh
```

This script will restart the backend container, which will trigger the seeding process if the database is empty.

## Clearing and Re-seeding

To completely reset and re-seed the database:

```bash
# Stop the environment
./dev-stop.sh

# Clean everything including database volumes
docker compose -f docker-compose.dev.yml down -v

# Start fresh - this will automatically seed
./dev-start.sh
```

## Testing the Seeded Data

### Using Swagger UI

1. Open http://localhost:5000/swagger
2. Click "Authorize" button
3. Login with any of the default users (e.g., `admin@uknf.gov.pl` / `Admin123!`)
4. Copy the access token from the response
5. Paste it in the authorization dialog (without "Bearer" prefix)
6. Test various endpoints with the seeded data

### Using curl

```bash
# Login
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@uknf.gov.pl","password":"Admin123!"}'

# Get messages (use token from login response)
curl http://localhost:5000/api/v1/messages \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

# Get supervised entities
curl http://localhost:5000/api/v1/entities \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

# Get FAQ questions
curl http://localhost:5000/api/v1/faq \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Customizing Seed Data

To modify the seed data, edit the file:
```
backend/UknfCommunicationPlatform.Infrastructure/Data/DatabaseSeeder.cs
```

The seeder is organized into separate methods:
- `SeedRolesAndPermissionsAsync()` - Security configuration
- `SeedUsersAsync()` - User accounts
- `SeedSupervisedEntitiesAsync()` - Supervised entities and their representatives
- `SeedMessagesAsync()` - Sample messages
- `SeedFaqAsync()` - FAQ questions
- `SeedAnnouncementsAsync()` - System announcements

After modifying the seeder:
1. Rebuild the backend: `docker compose -f docker-compose.dev.yml up --build -d backend`
2. The changes will take effect on next seeding

## Troubleshooting

### Seeding doesn't happen
- **Cause:** Database already contains data
- **Solution:** The seeder only runs on empty databases. Clear the database first (see "Clearing and Re-seeding" above)

### Permission errors
- **Cause:** Database connection issues
- **Solution:** Ensure PostgreSQL is running and accessible

### Data inconsistencies
- **Cause:** Partial seeding failure
- **Solution:** Clear the database and re-seed completely

### Check seeding logs
```bash
docker logs uknf-backend-dev | grep -i seed
```

## Production Considerations

⚠️ **Important:** The automatic seeding is **only enabled in Development environment**. It will not run in Production or Testing environments to prevent accidental data modification.

In production, initial data should be seeded through proper migration scripts or administrative tools.
