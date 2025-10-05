# Database Diagrams

This directory contains visual representations and documentation of the UKNF Communication Platform database schema.

## Contents

- **`database_schema.dot`** - Complete database schema in DOT (Graphviz) format for generating professional diagrams

## Overview

The database is designed for the UKNF (Polish Financial Supervision Authority) communication platform with three main modules:

### 1. Communication Module
- **Messages** - Internal and external communication with attachments
- **Message Attachments** - File attachments for messages
- **Announcements** - Public announcements with recipients and attachments
- **Announcement Recipients** - Target users/groups for announcements
- **Announcement Reads** - Tracking who read which announcements
- **Announcement Attachments** - File attachments for announcements
- **Announcement Histories** - Audit trail for announcement changes

### 2. Identity & Access Module
- **Users** - System users (internal UKNF staff and external entity representatives)
- **Roles** - User roles with permissions
- **Permissions** - Granular permissions for resources
- **Role Permissions** - Many-to-many relationship between roles and permissions
- **User Roles** - Many-to-many relationship between users and roles
- **Refresh Tokens** - JWT refresh tokens for authentication
- **Password Policies** - System-wide password requirements
- **Password Histories** - Tracks password changes for policy enforcement

### 3. Administration Module
- **Entities** - Supervised financial entities (banks, insurance companies, etc.)
- **Reports** - Periodic reports submitted by entities
- **Cases** - Administrative cases/requests
- **Case Documents** - Documents attached to cases
- **Case Histories** - Audit trail for case changes
- **File Libraries** - Document repository with categorization
- **File Library Permissions** - Access control for files
- **Contacts** - Contact registry for entities
- **Contact Groups** - Grouping of contacts
- **Contact Group Members** - Many-to-many relationship
- **FAQ Questions** - Frequently asked questions
- **FAQ Ratings** - User ratings for FAQ helpfulness

## Key Design Principles

### 1. Snake Case Naming
All database objects use snake_case naming convention:
- Tables: `users`, `supervised_entities`, `message_attachments`
- Columns: `first_name`, `created_at`, `is_active`
- Indexes: `i_x_users_email`, `i_x_messages_sender_id`
- Foreign Keys: `f_k_messages_users_sender_id`

### 2. Audit Trail
Most tables include audit columns:
- `created_at` - Timestamp of record creation
- `updated_at` - Timestamp of last update
- `created_by_user_id` - User who created the record
- `updated_by_user_id` - User who last updated the record

### 3. Soft Deletes
Key entities use soft delete pattern:
- `is_active` - Boolean flag instead of physical deletion
- Maintains referential integrity
- Enables audit trails and data recovery

### 4. Foreign Key Relationships
All relationships properly enforced with foreign key constraints:
- Cascade deletes where appropriate (e.g., message_attachments)
- Restrict deletes for core entities (e.g., users, entities)
- Proper indexing on foreign key columns

## Database Statistics

**Total Tables:** 29 tables

**By Module:**
- Communication: 7 tables
- Identity & Access: 8 tables  
- Administration: 14 tables

**Key Relationships:**
- `users` ↔ `entities` (supervised_entity_id)
- `messages` ↔ `users` (sender/recipient)
- `reports` ↔ `users` (submitted_by_user_id)
- `cases` ↔ `entities` (supervised_entity_id)
- `announcements` ↔ `announcement_recipients` (many-to-many via user/entity/podmiot)

## How to Use This Diagram

### Generating Visual Output

The DOT file can be converted to various image formats using Graphviz:

```bash
# Install Graphviz (if not already installed)
# Windows: choco install graphviz
# Mac: brew install graphviz
# Linux: apt-get install graphviz

# Generate PNG image (recommended for presentations)
dot -Tpng database_schema.dot -o database_schema.png

# Generate SVG image (scalable, best for web)
dot -Tsvg database_schema.dot -o database_schema.svg

# Generate PDF (best for documentation)
dot -Tpdf database_schema.dot -o database_schema.pdf

# Generate high-resolution PNG (for printing)
dot -Tpng -Gdpi=300 database_schema.dot -o database_schema_hires.png
```

### Online Viewers

If you don't have Graphviz installed, use online tools:
- **WebGraphviz:** http://www.webgraphviz.com/
- **Graphviz Online:** https://dreampuf.github.io/GraphvizOnline/
- **Viz.js:** http://viz-js.com/

Simply copy the contents of `database_schema.dot` and paste into the online editor.

### VS Code Integration

Install the "Graphviz Interactive Preview" extension:
1. Open Extensions (Ctrl+Shift+X)
2. Search for "Graphviz Preview"
3. Install the extension
4. Right-click on `database_schema.dot` → "Open Preview"

## For Development

1. **Quick Reference:** Generate PNG or SVG for visual overview
2. **Database Changes:** Edit the DOT file to add/modify tables and relationships
3. **Documentation:** Include generated diagrams in project documentation

## For Database Changes

1. Update the DOT file to reflect schema changes:
   - Add new table nodes in appropriate cluster (subgraph)
   - Add relationship edges with proper labels
   - Update statistics in legend
2. Regenerate visual diagram (PNG/SVG/PDF)
3. Create migration script in `backend/Infrastructure/Data/Migrations/`
4. Update this README with change notes

## For New Team Members

1. Generate a visual diagram from the DOT file
2. Study the three modules (Identity & Access, Communication, Administration)
3. Review relationships (solid lines = FK, dashed = optional FK)
4. Reference entity models in `backend/UknfCommunicationPlatform.Core/Entities/`

## Diagram Features

### Color Coding
- **Yellow** (`#FFEB3B`) - Core Users table
- **Orange** (`#FF9800`) - Roles & Permissions
- **Light Blue** (`#81D4FA`) - Communication module tables
- **Purple** (`#CE93D8`) - Administration module tables
- **Light shades** - Junction/detail tables

### Relationship Indicators
- **Solid lines** - Foreign Key constraints
- **Dashed lines** - Optional foreign keys
- **1:N labels** - One-to-many relationships
- **Crow's foot arrows** - Many side of relationship
- **Self-referencing** - Indicates hierarchical structure (e.g., message threads)

### Layout
- **Top-to-bottom** (TB) layout for better readability
- **Clustered** by module (Identity, Communication, Administration)
- **Legend** included in diagram for quick reference

## Connection Information

**Development Environment:**
- Host: localhost
- Port: 5432
- Database: uknf_db
- Username: uknf_user
- Password: uknf_password (from docker-compose.yml)

**Docker Container:**
```bash
# Connect to database
docker exec -it uknf-postgres psql -U uknf_user -d uknf_db

# List all tables
\dt

# Describe table structure
\d+ table_name

# View foreign keys
\d+ table_name
```

## Maintenance Notes

### Last Updated
- **Date:** October 5, 2025
- **Format:** DOT (Graphviz) diagram
- **Schema Version:** Based on migration `20251005044626_AddReportingPeriodTypeColumn`
- **Total Migrations:** 5 migrations applied
- **Total Tables:** 29 (8 Identity & Access, 7 Communication, 14 Administration)

### Recent Changes
1. Added `reporting_period_type` to `file_libraries` table
2. Added `priority` column to `messages` table
3. Simplified report entity structure (removed unnecessary columns)
4. Simplified FAQ question structure

### Known Issues
- None currently

### Future Enhancements
- Add table for notification preferences
- Implement message threading improvements
- Add advanced search capabilities
- Document versioning for file libraries

## References

- **Schema Reference:** `../SCHEMA_REFERENCE.md`
- **Database README:** `../README.md`
- **Migration Files:** `../../backend/UknfCommunicationPlatform.Infrastructure/Data/Migrations/`
- **Entity Models:** `../../backend/UknfCommunicationPlatform.Core/Entities/`

## Contributing

When updating the database diagram:
1. Edit `database_schema.dot` file to reflect schema changes
2. Add new tables in appropriate subgraph (cluster) by module
3. Add relationship edges with descriptive labels
4. Update statistics in the legend section
5. Regenerate visual output (PNG/SVG/PDF)
6. Update this README with change notes
7. Include comments in DOT file explaining complex relationships

---

**Note:** This diagram is a living document. Keep it updated with every schema change to maintain accuracy. The DOT format provides professional-quality output suitable for presentations and documentation.
