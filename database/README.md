# UKNF Communication Platform - Database Schema

This directory contains the PostgreSQL database schema for the UKNF Communication Platform.

## Files

- **init-schema.sql** - Complete database schema creation script
- **seed-data.sql** - Initial test data (5 entities, 8 users, reports, messages, cases)
- **additional-seed-data.sql** - Extended test data (10 more entities, 10 users, etc.)
- **apply-snake-case-columns.md** - Guide for EF Core snake_case column mapping

## Database Overview

The database schema is designed to support three main modules:

### 1. Authentication & Authorization Module
- User management (internal UKNF employees and external entity representatives)
- Access requests and permission management
- Role-based access control (RBAC)
- Password policy enforcement

### 2. Communication Module
- **Reporting System (Sprawozdania)** - Report submission, validation, and tracking
- **Messaging** - Bidirectional communication with file attachments
- **File Library (Biblioteka)** - Shared document repository
- **Cases (Sprawy)** - Administrative case management
- **Announcements** - Bulletin board with read confirmations
- **FAQ** - Question and answer knowledge base

### 3. Administration Module
- User account management
- Entity (Podmioty) registry
- Contact groups and addressees
- System audit logging
- Password policy configuration

## Key Entities

### Core Tables

| Table | Description |
|-------|-------------|
| `entities` | Supervised entities (Podmioty Nadzorowane) |
| `users` | All system users (internal and external) |
| `user_entity_permissions` | User-to-entity associations with permissions |
| `access_requests` | External user access requests |
| `reports` | Financial reports submitted by entities |
| `messages` | Communication threads between users |
| `cases` | Administrative cases |
| `announcements` | System-wide bulletins |
| `library_files` | Shared file repository |
| `faq_questions` / `faq_answers` | Knowledge base |

### Enumerations

The schema uses PostgreSQL ENUMs for type safety:

- `user_role` - System roles (administrator, UKNF employee, entity admin, etc.)
- `user_status` - Account status (active, blocked, pending)
- `report_status` - Report validation states
- `case_status` - Administrative case states
- `message_status` - Message thread states
- And more...

## Features

### Audit & History
- Entity change history tracking
- Case activity audit log
- Library file versioning
- System-wide audit log

### Security
- Password history tracking
- Configurable password policy
- User session tracking
- Failed login attempt monitoring

### Performance
- Comprehensive indexes on frequently queried columns
- Automatic timestamp updates via triggers
- Optimized foreign key relationships

## Usage

### Running the Schema Script

#### Using Docker Compose
If you're using the project's Docker setup:

```bash
# Start the PostgreSQL container
docker-compose up -d db

# Run the schema script
docker exec -i uknf-db psql -U postgres -d uknf_platform < database/init-schema.sql

# Run the initial seed data
docker exec -i uknf-db psql -U postgres -d uknf_platform < database/seed-data.sql

# (Optional) Run additional seed data for more test records
docker exec -i uknf-db psql -U postgres -d uknf_platform < database/additional-seed-data.sql
```

#### Direct PostgreSQL Connection
```bash
psql -h localhost -U postgres -d uknf_platform -f database/init-schema.sql
psql -h localhost -U postgres -d uknf_platform -f database/seed-data.sql
psql -h localhost -U postgres -d uknf_platform -f database/additional-seed-data.sql
```

#### PowerShell (Windows)
```powershell
# Create schema
Get-Content database/init-schema.sql | docker exec -i uknf-db psql -U postgres -d uknf_platform

# Load initial test data
Get-Content database/seed-data.sql | docker exec -i uknf-db psql -U postgres -d uknf_platform

# Load additional test data (optional)
Get-Content database/additional-seed-data.sql | docker exec -i uknf-db psql -U postgres -d uknf_platform
```

### Verifying Installation

```sql
-- Check if all tables were created
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;

-- Count should be 40+ tables
SELECT COUNT(*) as table_count
FROM information_schema.tables 
WHERE table_schema = 'public';
```

## Data Model Highlights

### User & Permission Model

```
users (1) ──< user_entity_permissions (M) >── (M) entities
                        │
                        └── Permissions:
                            - has_reporting_permission
                            - has_cases_permission
                            - is_entity_admin
```

### Reporting Workflow

```
entities ──< reports
              │
              ├── status: draft → submitted → in_progress → validation_success/errors
              ├── validation_report_path
              └── corrects_report_id (for corrections)
```

### Communication Flow

```
messages (1) ──< message_threads (M)
                       │
                       └──< message_attachments
```

### Case Management

```
cases (1) ──< case_attachments
       │
       ├──< case_history (audit)
       └── messages (related_case_id)
```

## Default Data

The schema includes:
- Default password policy (12 chars min, all character types required)
- UUID extension enabled for potential use

## Maintenance

### Backing Up

```bash
pg_dump -U postgres -d uknf_platform -F c -f backup_$(date +%Y%m%d).dump
```

### Restoring

```bash
pg_restore -U postgres -d uknf_platform -c backup_20251004.dump
```

## Schema Evolution

When modifying the schema:

1. Create migration scripts in `database/migrations/`
2. Use timestamped filenames: `YYYYMMDD_HHMMSS_description.sql`
3. Always include both UP and DOWN migration paths
4. Test migrations on a copy of production data

## Requirements Source

This schema is based on requirements from:
- `.requirements/DETAILS_UKNF_Prompt2Code2.md`
- `.requirements/RULES_UKNF_Prompt2Code2.md`

Key requirement sections:
- Communication Module (pp. 6-12)
- Authentication & Authorization Module (pp. 12-14)
- Administration Module (pp. 14-15)
- Entity data structure (p. 11)

## Related Documentation

- [Project README](../README.md)
- [Testing Guide](../TESTING_GUIDE.md)
- [Backend Instructions](../.github/instructions/backend.instructions.md)

