# Database Schema Quick Reference

## Core Tables Overview

### 1. Authentication & Authorization

```
users (id, email, role, status, is_internal)
    |
    +-- user_entity_permissions (user_id, entity_id, permissions)
    |       |
    |       +-- entities (id, uknf_code, entity_name, ...)
    |
    +-- access_requests (id, user_id, status)
            |
            +-- access_request_lines (request_id, entity_id, permissions)
```

### 2. Communication Module

#### Reporting
```
entities
    |
    +-- reports (id, entity_id, status, file_path)
            |
            +-- report_registries (id, name, is_archived)
```

#### Messaging
```
messages (id, subject, status, entity_id)
    |
    +-- message_threads (id, message_id, content)
            |
            +-- message_attachments (id, thread_id, file_path)
```

#### Cases
```
cases (id, case_number, entity_id, status, category)
    |
    +-- case_attachments (id, case_id, file_path)
    |
    +-- case_history (id, case_id, action, old_value, new_value)
```

#### Library
```
library_files (id, file_name, version_status, is_public)
    |
    +-- library_file_permissions (id, file_id, entity_id/user_id/group_id)
    |
    +-- library_file_history (id, file_id, version_number)
```

#### Announcements
```
announcements (id, title, content, priority, requires_confirmation)
    |
    +-- announcement_recipients (id, announcement_id, entity_id/user_id/group_id)
    |
    +-- announcement_confirmations (id, announcement_id, user_id, confirmed_at)
    |
    +-- announcement_attachments (id, announcement_id, file_path)
```

#### FAQ
```
faq_questions (id, title, content, status)
    |
    +-- faq_answers (id, question_id, content)
    |   |
    |   +-- faq_ratings (id, answer_id, rating)
    |
    +-- faq_question_tags (question_id, tag_id)
            |
            +-- faq_tags (id, name)
```

### 3. Administration Module

#### Contacts
```
contact_groups (id, name, description)
    |
    +-- contact_group_members (group_id, contact_id)
            |
            +-- contacts (id, first_name, last_name, email, entity_id, user_id)
```

#### System Management
```
password_policy (id, min_length, require_uppercase, ...)
users
    |
    +-- user_password_history (id, user_id, password_hash, changed_at)
    |
    +-- notifications (id, user_id, type, title, is_read)
```

#### Audit & History
```
audit_log (id, user_id, action, entity_type, entity_id, old_value, new_value)
entity_history (id, entity_id, field_name, old_value, new_value, changed_by)
case_history (id, case_id, action, old_value, new_value, performed_by)
library_file_history (id, file_id, version_number, file_path)
```

## Key Enumerations (PostgreSQL ENUMs)

### User Management
- **user_role**: system_administrator, uknf_employee, entity_administrator, entity_employee
- **user_status**: active, inactive, blocked, pending_activation

### Access Requests
- **access_request_status**: draft, new, accepted, blocked, updated

### Reporting
- **report_status**: draft, submitted, in_progress, validation_success, validation_errors, technical_error, timeout_error, challenged_by_uknf

### Cases
- **case_status**: draft, new, in_progress, requires_completion, completed, cancelled
- **case_priority**: low, medium, high
- **case_category**: registration_data_change, personnel_change, entity_summons, system_permissions, reporting, other

### Messages
- **message_status**: awaiting_uknf_response, awaiting_user_response, closed
- **message_context**: general, report, case, access_request

### Announcements
- **announcement_priority**: low, medium, high

### Library
- **file_version_status**: current, archived

### FAQ
- **faq_status**: pending, answered, archived

### Notifications
- **notification_type**: new_message, new_report, report_validated, case_updated, new_announcement, access_request_updated, entity_data_confirmation

## Table Relationships Matrix

| Parent Table | Child Tables | Relationship Type | Notes |
|-------------|--------------|-------------------|-------|
| entities | reports, cases, user_entity_permissions | 1:M | Core entity reference |
| users | messages, reports, cases, notifications | 1:M | User actions |
| reports | messages (related_report_id) | 1:M | Report communication |
| cases | messages (related_case_id), case_attachments, case_history | 1:M | Case management |
| messages | message_threads | 1:M | Conversation threads |
| message_threads | message_attachments | 1:M | File attachments |
| announcements | announcement_recipients, announcement_confirmations | 1:M | Bulletin distribution |
| faq_questions | faq_answers, faq_question_tags | 1:M | Knowledge base |
| faq_answers | faq_ratings | 1:M | User feedback |
| contact_groups | contact_group_members | M:M (via junction) | Group membership |
| library_files | library_file_permissions, library_file_history | 1:M | File management |

## Important Indexes

### Performance-Critical Indexes
- `idx_entities_uknf_code` - Entity lookups by UKNF code (unique)
- `idx_users_email` - User authentication (unique)
- `idx_reports_entity` + `idx_reports_status` - Report filtering
- `idx_cases_entity` + `idx_cases_status` - Case filtering
- `idx_messages_entity` + `idx_messages_status` - Message filtering
- `idx_notifications_user` + `idx_notifications_read` - Notification queries
- `idx_audit_log_performed_at` - Audit log chronological queries

## Triggers

All tables with `updated_at` columns have automatic update triggers:
- entities
- users
- access_requests
- reports
- messages
- library_files
- cases
- announcements
- faq_questions
- faq_answers

Trigger function: `update_updated_at_column()`

## Storage Estimates (per 1000 entities)

| Table | Estimated Rows | Storage/Row | Total |
|-------|---------------|-------------|-------|
| entities | 1,000 | ~2 KB | 2 MB |
| users | ~5,000 | ~1 KB | 5 MB |
| reports | ~20,000/year | ~1 KB | 20 MB/year |
| messages | ~50,000/year | ~500 B | 25 MB/year |
| cases | ~10,000/year | ~1 KB | 10 MB/year |
| announcements | ~500/year | ~2 KB | 1 MB/year |
| audit_log | ~500,000/year | ~500 B | 250 MB/year |

**Note**: File attachments stored on filesystem, only metadata in database.

## Query Patterns

### Common Queries

#### 1. User's Entities
```sql
SELECT e.* 
FROM entities e
JOIN user_entity_permissions uep ON e.id = uep.entity_id
WHERE uep.user_id = ? AND uep.is_active = TRUE;
```

#### 2. UKNF Employee's Assigned Entities ("Moje podmioty")
```sql
SELECT e.*
FROM entities e
JOIN uknf_employee_entities uee ON e.id = uee.entity_id
WHERE uee.uknf_employee_id = ?;
```

#### 3. Entity's Pending Reports
```sql
SELECT * FROM reports
WHERE entity_id = ? 
  AND status IN ('draft', 'submitted', 'in_progress')
ORDER BY submitted_at DESC;
```

#### 4. Unread Notifications
```sql
SELECT * FROM notifications
WHERE user_id = ? AND is_read = FALSE
ORDER BY created_at DESC
LIMIT 20;
```

#### 5. Active Cases for Entity
```sql
SELECT * FROM cases
WHERE entity_id = ?
  AND status IN ('new', 'in_progress', 'requires_completion')
ORDER BY priority DESC, created_at DESC;
```

## Maintenance Queries

### Archive Old Data
```sql
-- Archive old announcements
UPDATE announcements 
SET is_published = FALSE 
WHERE expires_at < CURRENT_TIMESTAMP;

-- Archive old reports
UPDATE reports 
SET registry_id = (SELECT id FROM report_registries WHERE is_archived = TRUE LIMIT 1)
WHERE report_period < '2024-Q1';
```

### Cleanup
```sql
-- Remove old notifications (older than 90 days)
DELETE FROM notifications 
WHERE created_at < CURRENT_TIMESTAMP - INTERVAL '90 days' 
  AND is_read = TRUE;

-- Remove old audit logs (older than 2 years)
DELETE FROM audit_log 
WHERE performed_at < CURRENT_TIMESTAMP - INTERVAL '2 years';
```

## Backup Strategy

### Full Backup (Daily)
```bash
pg_dump -U postgres -d uknf_platform -F c -f /backups/full_$(date +%Y%m%d).dump
```

### Incremental Backup (Hourly)
```bash
# Using WAL archiving
pg_basebackup -D /backups/incremental -F tar -X stream
```

### Table-Specific Backup (On-Demand)
```bash
pg_dump -U postgres -d uknf_platform -t entities -F c -f entities_backup.dump
pg_dump -U postgres -d uknf_platform -t reports -F c -f reports_backup.dump
```

## Performance Tuning

### Recommended PostgreSQL Settings (postgresql.conf)
```ini
shared_buffers = 4GB
effective_cache_size = 12GB
maintenance_work_mem = 1GB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 100
random_page_cost = 1.1
effective_io_concurrency = 200
work_mem = 20MB
min_wal_size = 2GB
max_wal_size = 8GB
max_worker_processes = 8
max_parallel_workers_per_gather = 4
max_parallel_workers = 8
```

### Analyze Tables (Weekly)
```sql
ANALYZE entities;
ANALYZE users;
ANALYZE reports;
ANALYZE messages;
ANALYZE cases;
```

## Version History

- **v1.0** (2025-10-04): Initial schema based on UKNF requirements
  - 40+ tables
  - Complete module coverage
  - Comprehensive indexing
  - Audit logging
  - Seed data included
