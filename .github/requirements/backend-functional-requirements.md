# Backend Functional Requirements

This document contains detailed functional requirements for the UKNF Communication Platform backend API.

---

## Authentication & Authorization Module

### User Registration (External Users)
- **POST** `/api/v1/auth/register` - Register new external user
  - Required fields: `Imię`, `Nazwisko`, `PESEL` (masked, show last 4 digits), `Telefon`, `Email`
  - Generate activation link and send via email
  - Create initial access request with status "Roboczy" (Draft)
  - Validate PESEL format (11 digits), NIP (10 digits), KRS (10 digits), LEI (20 chars)
  - Validate phone number with international format pattern: `/^\+(?:[0-9] ?){6,14}[0-9]$/`
  - Return `201 Created` with user ID and activation token expiry

### Password Management
- **POST** `/api/v1/auth/activate` - Activate account and set initial password
  - Accept activation token from email link
  - Enforce password policy: min/max length, complexity (uppercase, lowercase, numbers, special chars)
  - Hash passwords with bcrypt/Argon2 before storage
  - Return JWT access token + refresh token on success
- **POST** `/api/v1/auth/reset-password` - Request password reset
- **POST** `/api/v1/auth/change-password` - Change password (authenticated)
  - Validate current password
  - Check password history (prevent reuse of last N passwords)
  - Enforce password policy rules
- **GET** `/api/v1/auth/password-policy` - Retrieve current password policy configuration

### Access Requests (Wnioski o dostęp)
- **GET** `/api/v1/access-requests` - List access requests with filters
  - Query params: `status`, `podmiotId`, `userId`, `myPodmioty` (UKNF staff filter)
  - Support quick filters: "Wymaga działania UKNF", "Obsługiwany przez UKNF"
  - Return paginated results with metadata
- **GET** `/api/v1/access-requests/{id}` - Get access request details
  - Include permission lines (Sprawozdawczość, Sprawy, Administrator podmiotu)
  - Include message thread history
  - Include attachment list
- **POST** `/api/v1/access-requests` - Create new access request
  - Auto-generated after registration with status "Roboczy"
  - Allow multiple permission lines (one per podmiot)
- **PUT** `/api/v1/access-requests/{id}` - Update access request
  - Add/modify permission lines (uprawnienia)
  - Assign podmiot email for notifications
  - Change status: Roboczy → Nowy (on submit)
- **POST** `/api/v1/access-requests/{id}/submit` - Submit request for approval
  - Validate all required fields
  - Send confirmation email
  - Change status to "Nowy"
- **POST** `/api/v1/access-requests/{id}/approve` - Approve request (UKNF/Admin Podmiotu)
  - Grant permissions per permission line
  - Send notification email
  - Change status to "Zaakceptowany" when all lines approved
- **POST** `/api/v1/access-requests/{id}/block` - Block/reject request
  - Change status to "Zablokowany"
  - Send rejection notification
- **POST** `/api/v1/access-requests/{id}/messages` - Add message to request thread
  - Support attachments (PDF, DOC/DOCX, XLS/XLSX, CSV/TXT, MP3, ZIP)
  - Reject files exceeding 100MB total (before compression)
  - Scan for viruses (prepare integration hook)

### Statuses for Access Requests
- `Roboczy` - Draft, not submitted
- `Nowy` - Submitted, pending approval
- `Zaakceptowany` - All permission lines approved
- `Zablokowany` - All permission lines blocked
- `Zaktualizowany` - Modified, awaiting re-approval

### Podmiot Context Switching
- **GET** `/api/v1/users/{userId}/podmioty` - List podmioty accessible to user
  - Return podmioty with assigned permissions
  - Include current session podmiot
- **POST** `/api/v1/users/{userId}/select-podmiot` - Switch active podmiot context
  - Update session context
  - Return new JWT with podmiot claim
  - Trigger context reload for frontend

### Role-Based Access Control (RBAC)
- **GET** `/api/v1/roles` - List all system roles
  - Return predefined roles: Administrator systemu, Pracownik UKNF, Administrator Podmiotu, Pracownik Podmiotu
  - Return custom roles if any
- **GET** `/api/v1/roles/{id}` - Get role details with permissions
- **POST** `/api/v1/roles` - Create custom role (Admin only)
- **PUT** `/api/v1/roles/{id}` - Update role permissions
- **DELETE** `/api/v1/roles/{id}` - Delete custom role
- **GET** `/api/v1/roles/{id}/users` - List users assigned to role
- **POST** `/api/v1/users/{userId}/roles` - Assign role to user
- **DELETE** `/api/v1/users/{userId}/roles/{roleId}` - Remove role from user

### Permission Management
- Permissions by module: Sprawozdania (Read/Write/Manage), Sprawy (Read/Write), Wiadomości (Read/Write), Biblioteka (Read/Write/Manage), Komunikaty (Read/Write/Manage), FAQ (Read/Write/Answer), Kartoteka (Read/Write/Manage), Wnioski (Read/Approve/Manage), Admin Panel (Full)
- Administrator Podmiotu can modify permissions for Pracownik Podmiotu: Sprawozdawczość (access/no access), Sprawy (access/no access)
- Administrator Podmiotu can block/unblock user access to system
- UKNF can block Administrator Podmiotu (affects approval workflow)

---

## Communication Module

### Reports (Sprawozdania)
- **GET** `/api/v1/reports` - List reports with filters
  - Query params: `status`, `period`, `podmiotId`, `myPodmioty`, `reportType`, `isArchived`
  - Support registries: "Sprawozdania kwartalne", "Sprawozdania roczne", "Aktualne", "Archiwalne"
  - Return report metadata: file name, number, period, submitter details, podmiot, validation status
- **GET** `/api/v1/reports/{id}` - Get report details
  - Include validation report attachment
  - Include unique identifier (for "Przekazane" status)
  - Include correction (korekta) relationship if applicable
- **POST** `/api/v1/reports` - Upload report (XLSX file)
  - Accept multipart/form-data with file + metadata
  - Validate XLSX format
  - Set status to "Robocze" after file added
  - Start validation workflow asynchronously
  - Return report ID
- **POST** `/api/v1/reports/{id}/validate` - Trigger validation (async)
  - Change status to "Przekazane" with unique ID
  - Invoke external validation service (prepare integration hook)
  - Set status to "W trakcie" during processing
  - Set timeout: "Błąd - przekroczono czas" if not completed in 24h
  - Attach validation report on completion
- **GET** `/api/v1/reports/{id}/validation-report` - Download validation report
  - Return PDF/XLSX with validation results
  - Include UKNF markers, dates, podmiot name, error details
- **POST** `/api/v1/reports/{id}/reject` - Reject report manually (UKNF staff)
  - Set status to "Zakwestionowane przez UKNF"
  - Require "Opis nieprawidłowości" field
  - Send notification to submitter
- **POST** `/api/v1/reports/{id}/correct` - Submit correction (korekta)
  - Link to original report
  - Start new validation workflow
- **POST** `/api/v1/reports/{id}/archive` - Archive report (UKNF staff)
  - Set `isArchived` flag
  - Move to archival registry
- **GET** `/api/v1/reports/missing` - List podmioty that haven't submitted for selected period
  - Query params: `period`, `reportType`
  - Return podmioty list
  - Support komunikat generation from this list

### Report Statuses
- `Robocze` - Draft state after file added
- `Przekazane` - Validation started with unique ID
- `W trakcie` - Validation in progress
- `Proces walidacji zakończony sukcesem` - No validation errors
- `Błędy z reguł walidacji` - Validation rule failures
- `Błąd techniczny w procesie walidacji` - Technical processing error
- `Błąd - przekroczono czas` - 24h timeout exceeded
- `Zakwestionowane przez UKNF` - Manually rejected by UKNF staff

### Messages (Wiadomości)
- **GET** `/api/v1/messages` - List messages (inbox/sent/drafts)
  - Query params: `folder`, `status`, `podmiotId`, `myPodmioty`, `threadId`, `unreadOnly`
  - Support grouping by thread
  - Return message metadata with read/unread status
- **GET** `/api/v1/messages/{id}` - Get message details
  - Include thread history
  - Include attachments list
  - Mark as read on retrieval
- **POST** `/api/v1/messages` - Send message
  - Support recipients: single user, user groups, podmiot types, all external users
  - Support attachments (PDF, DOC/DOCX, XLS/XLSX, CSV/TXT, MP3, ZIP max 100MB)
  - Validate file formats, reject unexpected formats
  - Scan for viruses and SPAM (prepare integration hook)
  - Set status: "Oczekuje na odpowiedź UKNF" (if from external) or "Oczekuje na odpowiedź Użytkownika" (if from UKNF)
- **POST** `/api/v1/messages/{id}/reply` - Reply to message
  - Maintain thread continuity
  - Update status to "Zamknięty" if conversation complete
- **DELETE** `/api/v1/messages/{id}` - Delete message (draft only)
- **GET** `/api/v1/messages/unread-count` - Get unread message count
  - Return count per folder (inbox, sprawozdania, sprawy, wnioski)

### Message Statuses
- `Oczekuje na odpowiedź UKNF` - External user sent message
- `Oczekuje na odpowiedź Użytkownika` - UKNF staff replied
- `Zamknięty` - Conversation completed

### File Repository / Library (Biblioteka)
- **GET** `/api/v1/library/files` - List files with filters
  - Query params: `category`, `period`, `fileType`, `isArchived`, `search`
  - Return metadata: name, period, update date, version, uploader, permissions
- **GET** `/api/v1/library/files/{id}` - Get file metadata
  - Include version history
  - Include change log
  - Include permission assignments
- **POST** `/api/v1/library/files` - Upload file (UKNF staff only)
  - Accept metadata: "Nazwa pliku", "Okres sprawozdawczy", "Data aktualizacji wzoru"
  - Support versioning (auto-increment version on update)
  - Set permissions: all users, specific podmiot types, specific podmioty, user groups, individual users
- **PUT** `/api/v1/library/files/{id}` - Update file metadata
  - Track change history
- **DELETE** `/api/v1/library/files/{id}` - Delete file (UKNF staff only)
- **GET** `/api/v1/library/files/{id}/download` - Download file
  - Check user permissions
  - Return file stream with proper content-type
  - Support ZIP compression/decompression
- **GET** `/api/v1/library/files/{id}/versions` - Get version history
  - Return all versions with download links
  - Mark current vs. archival versions
- **POST** `/api/v1/library/files/{id}/permissions` - Manage file permissions
  - Grant/revoke access to users/groups/podmioty

### Administrative Cases (Sprawy)
- **GET** `/api/v1/cases` - List cases with filters
  - Query params: `status`, `category`, `priority`, `podmiotId`, `myPodmioty`, `assignedStaff`
  - Return case metadata: number, podmiot, category, priority, status, created date
- **GET** `/api/v1/cases/{id}` - Get case details
  - Include "Teczka sprawy" (case folder) structure
  - Include message thread
  - Include attachment list
  - Include change history
- **POST** `/api/v1/cases` - Create new case
  - Required: category, priority, single podmiot association
  - Categories: "Zmiana danych rejestrowych", "Zmiana składu osobowego", "Wezwanie do Podmiotu Nadzorowanego", "Uprawnienia do Systemu", "Sprawozdawczość", "Inne"
  - Priority: Niski, Średni, Wysoki
  - Auto-generate case number
  - Set status to "Wersja robocza" (not visible to UKNF)
- **POST** `/api/v1/cases/{id}/submit` - Submit/launch case
  - Change status to "Nowa sprawa" (visible to UKNF)
  - Send notification to UKNF and podmiot
- **PUT** `/api/v1/cases/{id}/status` - Update case status
  - Allowed transitions: Nowa sprawa → W toku, W toku → Do uzupełnienia, W toku → Zakończona
  - UKNF staff only for "Do uzupełnienia", "Zakończona"
  - Auto-set "W toku" when opened by staff or external user
- **POST** `/api/v1/cases/{id}/cancel` - Cancel case (UKNF staff only)
  - Only if status is "Nowa sprawa" and not yet read by Pracownik Podmiotu
  - Replace content with "wiadomość anulowana" placeholder
  - Send cancellation notification to podmiot
  - Set status to "Anulowana"
  - Block further edits
- **POST** `/api/v1/cases/{id}/messages` - Add message to case
  - Support attachments
  - Update case status automatically
- **POST** `/api/v1/cases/{id}/attachments` - Add document to case
  - Support file upload (same formats as messages)
  - Track in case folder
- **GET** `/api/v1/cases/{id}/history` - Get case change history
  - Return timeline of status changes, messages, attachments

### Case Statuses
- `Wersja robocza` - Draft, not visible to UKNF
- `Nowa sprawa` - Submitted/launched, visible to UKNF
- `W toku` - Opened by staff or external user
- `Do uzupełnienia` - Requires user action/attachment (set by UKNF)
- `Zakończona` - Closed by UKNF
- `Anulowana` - Cancelled before user read

### Announcements (Komunikaty)
- **GET** `/api/v1/announcements` - List announcements with filters
  - Query params: `category`, `priority`, `isExpired`, `recipientType`
  - Return komunikat metadata: title, category, priority, expiration date, read stats
- **GET** `/api/v1/announcements/{id}` - Get announcement details
  - Include WYSIWYG rich text content
  - Include attachment list
  - Include read confirmations (for high-priority messages)
  - Include change history
- **POST** `/api/v1/announcements` - Create announcement (UKNF staff only)
  - WYSIWYG editor content (sanitize HTML to prevent XSS)
  - Set priority: Niski, Średni, Wysoki
  - Set category
  - Set expiration date
  - Define recipients: single users, user groups, podmiot types, all external users
  - Support attachments
- **PUT** `/api/v1/announcements/{id}` - Update announcement
  - Track version history
- **POST** `/api/v1/announcements/{id}/publish` - Publish announcement
  - Send notifications to recipients
  - Mark as unread for recipients
- **POST** `/api/v1/announcements/{id}/unpublish` - Unpublish announcement
- **DELETE** `/api/v1/announcements/{id}` - Delete announcement
- **POST** `/api/v1/announcements/{id}/confirm-read` - Confirm reading (for high-priority)
  - Capture: date/time, user name (Imię, Nazwisko), podmiot name
  - Required for priority "Wysoki"
  - Return confirmation details
- **GET** `/api/v1/announcements/{id}/read-stats` - Get read statistics
  - Return count: "71/100 podmiotów odczytało"
  - Return list of confirmations with timestamps, user names, podmiot names

### Announcement Priority Levels
- `Niski` - Low priority
- `Średni` - Medium priority
- `Wysoki` - High priority (requires read confirmation)

### Recipients, Contact Groups, Contacts
- **GET** `/api/v1/recipients` - List available recipient types
  - Return: podmiot types, podmioty list, external users list, contact groups
- **POST** `/api/v1/recipients/resolve` - Resolve recipients for message/komunikat
  - Input: recipient definition (types, podmioty, users, groups)
  - Return: actual user list with email addresses
- **GET** `/api/v1/contact-groups` - List contact groups
- **GET** `/api/v1/contact-groups/{id}` - Get contact group details
  - Include member list (system users + external contacts)
- **POST** `/api/v1/contact-groups` - Create contact group (UKNF staff only)
- **PUT** `/api/v1/contact-groups/{id}` - Update contact group
  - Add/remove members
- **DELETE** `/api/v1/contact-groups/{id}` - Delete contact group
- **GET** `/api/v1/contacts` - List contacts (non-system users)
  - Return: Imię, Nazwisko, Email, Telefon, Podmiot (optional)
- **POST** `/api/v1/contacts` - Add new contact
  - Email-only notification capability
  - Optional podmiot association
- **PUT** `/api/v1/contacts/{id}` - Update contact
- **DELETE** `/api/v1/contacts/{id}` - Delete contact
- **POST** `/api/v1/contacts/bulk-import` - Bulk import contacts from CSV

### FAQ (Baza pytań i odpowiedzi)
- **GET** `/api/v1/faq/questions` - List questions with filters
  - Query params: `category`, `tag`, `status`, `sortBy` (popularity, date, rating)
  - Support search by keyword (title, content, tags)
  - Return: title, category, tags, date, status, rating, view count
- **GET** `/api/v1/faq/questions/{id}` - Get question details
  - Include answer (if answered)
  - Include rating statistics
  - Increment view counter
- **POST** `/api/v1/faq/questions` - Submit new question
  - Support anonymous and authenticated submission
  - Fields: Tytuł, Treść, Kategoria, Etykiety
  - Set status to "Submitted"
- **PUT** `/api/v1/faq/questions/{id}` - Update question (UKNF staff only)
  - Modify title, content, category, tags, status
- **DELETE** `/api/v1/faq/questions/{id}` - Delete question (UKNF staff only)
- **POST** `/api/v1/faq/questions/{id}/answer` - Add answer (UKNF staff only)
  - WYSIWYG editor content (sanitize HTML)
  - Set status to "Answered"
  - Send notification to question submitter
- **PUT** `/api/v1/faq/questions/{id}/answer` - Update answer
- **POST** `/api/v1/faq/questions/{id}/publish` - Publish Q&A
- **POST** `/api/v1/faq/questions/{id}/unpublish` - Unpublish Q&A
- **POST** `/api/v1/faq/questions/{id}/rate` - Rate answer (1-5 stars)
  - One rating per user per answer
  - Update average rating
- **GET** `/api/v1/faq/analytics` - Get FAQ analytics (UKNF staff only)
  - Most viewed questions
  - Average ratings per category
  - Unanswered questions count

### FAQ Question Statuses
- `Draft` - Not submitted
- `Submitted` - Pending answer
- `Answered` - UKNF staff provided answer
- `Closed` - Question closed/archived

### Podmiot Registry (Kartoteka Podmiotów)
- **GET** `/api/v1/podmioty` - List podmioty with filters
  - Query params: `typPodmiotu`, `statusPodmiotu`, `kategoria`, `sektor`, `podsektor`, `podmiotTransgraniczny`, `search`
  - Search by: Nazwa podmiotu, Kod UKNF, LEI, NIP, KRS
  - Return all metadata fields
  - Support export to XLSX/CSV
- **GET** `/api/v1/podmioty/{id}` - Get podmiot details
  - Return all fields: ID, Typ podmiotu, Kod UKNF, Nazwa, LEI, NIP, KRS, Address fields, Contact fields, Classification fields, Timestamps
  - Include associated users list with roles
  - Include change history timeline
- **POST** `/api/v1/podmioty` - Create podmiot (UKNF Admin only)
  - Validate LEI (20 chars), NIP (10 digits), KRS (10 digits), Phone (international format)
  - Auto-generate Kod UKNF (non-editable)
- **PUT** `/api/v1/podmioty/{id}` - Update podmiot (UKNF Admin/Staff only)
  - Track version history with timestamps
  - Preserve audit trail
- **GET** `/api/v1/podmioty/{id}/users` - Get users assigned to podmiot
- **GET** `/api/v1/podmioty/{id}/history` - Get change history

### Podmiot Data Schema
```csharp
public class PodmiotEntity
{
    public long ID { get; set; }
    public string TypPodmiotu { get; set; } // max 250
    public string KodUKNF { get; set; } // max 250, non-editable
    public string NazwaPodmiotu { get; set; } // max 500
    public string LEI { get; set; } // max 20
    public string NIP { get; set; } // max 10
    public string KRS { get; set; } // max 10
    public string Ulica { get; set; } // max 250
    public string NumerBudynku { get; set; } // max 250
    public string NumerLokalu { get; set; } // max 250
    public string KodPocztowy { get; set; } // max 250
    public string Miejscowosc { get; set; } // max 250
    public string Telefon { get; set; } // max 250, validate international format
    public string Email { get; set; } // max 500, validate @ symbol
    public string NumerWpisuDoRejestru { get; set; } // max 100
    public string StatusPodmiotu { get; set; } // max 250, e.g., Wpisany, Wykreślony
    public string KategoriaPodmiotu { get; set; } // max 500
    public string SektorPodmiotu { get; set; } // max 500
    public string PodsektorPodmiotu { get; set; } // max 500
    public bool PodmiotTransgraniczny { get; set; } // checkbox
    public DateTime DataUtworzenia { get; set; }
    public DateTime DataAktualizacji { get; set; }
}
```

### Podmiot Data Updater Service (Aktualizator danych podmiotu)
- **GET** `/api/v1/podmioty/{id}/editable-fields` - Get fields editable by external users
  - Return: Nazwa podmiotu, Ulica, Kod pocztowy, Miejscowość, Telefon, E-mail
- **POST** `/api/v1/podmioty/{id}/confirm-data` - Confirm current data is accurate
  - Log confirmation with timestamp and user
- **POST** `/api/v1/podmioty/{id}/request-change` - Request data change
  - Create new case with category "Zmiana danych rejestrowych"
  - Include requested changes in case details
  - Send notification to UKNF
- **POST** `/api/v1/podmioty/{id}/verify-change` - Verify change request (UKNF staff)
  - Side-by-side comparison: current vs. requested
  - Approve/reject actions
  - On approval: write to Baza Podmiotów, preserve version history
  - Send notification to requesting user
- **GET** `/api/v1/podmioty/data-confirmation-alerts` - Get pending confirmation alerts
  - Return list of users due for periodic data confirmation prompt

---

## Administration Module

### User Management
- **GET** `/api/v1/admin/users` - List all users (internal + external)
  - Query params: `role`, `status`, `podmiotId`, `search` (name, email)
  - Return: user details with assigned roles and podmiot associations
- **GET** `/api/v1/admin/users/{id}` - Get user details
  - Include role assignments
  - Include podmiot associations with permissions
  - Include activity log
- **POST** `/api/v1/admin/users` - Create user (Admin only)
  - Support internal and external users
  - Assign initial roles
- **PUT** `/api/v1/admin/users/{id}` - Update user
  - Modify profile fields
  - Activate/deactivate account
- **DELETE** `/api/v1/admin/users/{id}` - Delete user (soft delete)
- **POST** `/api/v1/admin/users/{id}/reset-password` - Force password reset
  - Generate reset token
  - Send reset email
  - Override account lock if needed
- **POST** `/api/v1/admin/users/bulk-action` - Bulk user actions
  - Support: activate, deactivate, assign role
  - Input: user IDs array + action type

### Password Policy Configuration
- **GET** `/api/v1/admin/password-policy` - Get current password policy
  - Return: min length, max length, complexity requirements, expiration days, history length, lockout threshold
- **PUT** `/api/v1/admin/password-policy` - Update password policy (Admin only)
  - Fields: minLength (8-32), requireUppercase, requireLowercase, requireNumbers, requireSpecialChars, expirationDays (0 = never), historyLength, failedLoginLockout
  - Validate configuration
  - Apply policy to new password changes
- **POST** `/api/v1/admin/users/{id}/force-password-change` - Force password change for user
  - Set flag requiring password change on next login

### Role Management
- **GET** `/api/v1/admin/roles` - List all roles
  - Return predefined and custom roles
  - Include permission matrix
  - Include usage analytics (user count per role)
- **GET** `/api/v1/admin/roles/{id}` - Get role details
  - Return permission assignments per module
  - Return assigned users
- **POST** `/api/v1/admin/roles` - Create custom role (Admin only)
  - Fields: Role name, Description
  - Define permission matrix (modules × permission levels)
- **PUT** `/api/v1/admin/roles/{id}` - Update role permissions
  - Modify permission matrix
  - Track change history
- **DELETE** `/api/v1/admin/roles/{id}` - Delete custom role
  - Prevent deletion if users assigned
  - Audit role change log

### Permission Matrix
Modules: Sprawozdania, Sprawy, Wiadomości, Biblioteka, Komunikaty, FAQ, Kartoteka, Wnioski, Admin Panel
Permission Levels: None, Read, Write, Delete, Manage

### Audit Trail
- **GET** `/api/v1/admin/audit-log` - Get audit log with filters
  - Query params: `userId`, `action`, `entity`, `dateFrom`, `dateTo`, `correlationId`
  - Return: timestamp, user, action, entity type, entity ID, changes, IP address, correlation ID
- **GET** `/api/v1/admin/audit-log/{id}` - Get audit log entry details
  - Include before/after snapshots for data changes

---

## File Management

### Chunked File Upload
- **POST** `/api/v1/files/chunks` - Upload file chunk
  - Support resumable uploads
  - Return chunk ID and offset for next chunk
  - Validate file type and total size limit (100MB before ZIP)
- **POST** `/api/v1/files/complete` - Complete chunked upload
  - Assemble chunks into final file
  - Generate file metadata
  - Trigger virus scan (prepare integration hook)
  - Return file ID
- **DELETE** `/api/v1/files/chunks/{uploadId}` - Cancel upload and cleanup chunks

### File Storage Abstraction
- Interface: `IFileStorageService`
- Methods: `StoreAsync`, `RetrieveAsync`, `DeleteAsync`, `GetMetadataAsync`
- Support: local disk, Azure Blob Storage, AWS S3 (future)
- Virus scanning hook: `IScanService.ScanFileAsync(fileId)` - returns scan status

---

## Background Jobs & Async Processing

### Queue Integration
- Abstract behind `IMessageQueue` interface
- Implementations: In-memory (dev), Kafka, RabbitMQ
- Use cases:
  - Report validation workflow (trigger external service)
  - Email notifications (registration, access request status, komunikat publishing)
  - Periodic data confirmation alerts for podmioty
  - File virus scanning
  - Bulk operations (export, bulk user actions)

### Scheduled Jobs
- Report validation timeout checker (runs every 1h, flags reports > 24h as "Błąd - przekroczono czas")
- Password expiration notifier (runs daily, warns users N days before expiration)
- Data confirmation alert generator (runs weekly, prompts users to confirm podmiot data)
- Komunikat expiration handler (runs daily, unpublishes expired announcements)

---

## Health Checks & Monitoring

### Health Endpoints
- **GET** `/health/ready` - Readiness probe
  - Check database connectivity
  - Check queue connectivity
  - Check file storage access
  - Return 200 if all healthy, 503 if any unhealthy
- **GET** `/health/live` - Liveness probe
  - Simple ping response
  - Return 200 always (if process is running)

### Metrics & Telemetry
- Instrument with OpenTelemetry (traces, metrics, logs)
- Structured logging with Serilog (JSON format)
- Correlation IDs for request tracing across services
- Capture custom metrics:
  - Report upload count by status
  - Message send/receive count
  - Access request approval rate
  - FAQ question submission rate
  - Login success/failure rate
  - File upload success/failure rate
