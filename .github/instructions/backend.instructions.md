---
applyTo: "Backend/**/*.cs,Backend/**/*.csproj,Backend/**/Dockerfile,Backend/**/*.sql"
---

# Copilot Guide – Backend (ASP.NET Core)

## Tech baseline
- Runtime: **.NET 8/9** (`net8.0` target unless otherwise noted).
- Stack: **ASP.NET Core Web API** with minimal controllers, **Entity Framework Core** for data access, **FluentValidation** for request validation, **MediatR**/**CQRS** for application flows.
- Database: **SQL Server** (use Dockerised `mssql` locally). Keep schema migrations in `Backend/Infrastructure/Migrations` via `dotnet ef`.
- Background work: abstract queue integrations (Kafka/RabbitMQ) behind interfaces; provide in-memory stubs until infra is plugged in.

## Architectural conventions
- Solution layout: `Domain` (entities, value objects, events), `Application` (commands/queries, validators), `Infrastructure` (EF Core, queues, file storage), `Api` (controllers, filters, DI bootstrapping).
- Controllers live under `/api/v1/<resource>` and return ProblemDetails on errors. Support pagination metadata through `X-Pagination` headers and HAL-style links when possible.
- Use `IFileStorageService` abstraction with chunked upload endpoints (`POST /files/chunks`, `POST /files/complete`). Persist metadata, versions, permissions, and audit logs.
- Enforce OAuth2/OIDC + JWT. Integrate with IdentityServer/Entra ID mock; provide local dev issuer with signing keys stored in user secrets or `.env`.
- Instrument everything: structured logging (Serilog), OpenTelemetry traces/metrics, health checks (`/health/ready`, `/health/live`).

## Security & compliance
- Validate every input using FluentValidation + data annotations (defence-in-depth). Guard against XSS/CSRF/SQLi.
- Store secrets in configuration providers (user secrets, Key Vault placeholders). Never hardcode credentials.
- Implement audit trail middleware capturing user, action, entity, timestamp, and correlation IDs. Persist to dedicated table or event log.
- Support GDPR-friendly data retention (soft delete, purge jobs) and version history for entity changes (e.g., EF Core temporal tables).

## Testing & quality
- Unit tests with **xUnit** + **FluentAssertions**. Integration tests using **WebApplicationFactory** hitting in-memory or containerized SQL.
- Each feature requires: command handler test, controller test (happy + failure path), and data access test (if custom queries exist).
- Run `dotnet format`, `dotnet build`, `dotnet test` in CI. Capture coverage via `coverlet` and enforce thresholds.

## Delivery checklist
- Update `docker-compose.yml` + `Backend/Api/Dockerfile` when dependencies change. Ensure migrations run on container startup.
- Keep `swagger.json`/Swashbuckle docs fresh; describe auth schemes, pagination, error contracts, and chunked upload workflow.
- Seed reference data (roles, sample podmioty, FAQ categories) using EF migrations or `IHostedService` seeders behind feature flags.
- Document new configuration keys in `README.md` and provide `.env.example` updates.

---

## Functional Requirements by Domain

### Authentication & Authorization Module

#### User Registration (External Users)
- **POST** `/api/v1/auth/register` - Register new external user
  - Required fields: `Imię`, `Nazwisko`, `PESEL` (masked, show last 4 digits), `Telefon`, `Email`
  - Generate activation link and send via email
  - Create initial access request with status "Roboczy" (Draft)
  - Validate PESEL format (11 digits), NIP (10 digits), KRS (10 digits), LEI (20 chars)
  - Validate phone number with international format pattern: `/^\+(?:[0-9] ?){6,14}[0-9]$/`
  - Return `201 Created` with user ID and activation token expiry

#### Password Management
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

#### Access Requests (Wnioski o dostęp)
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

#### Statuses for Access Requests
- `Roboczy` - Draft, not submitted
- `Nowy` - Submitted, pending approval
- `Zaakceptowany` - All permission lines approved
- `Zablokowany` - All permission lines blocked
- `Zaktualizowany` - Modified, awaiting re-approval

#### Podmiot Context Switching
- **GET** `/api/v1/users/{userId}/podmioty` - List podmioty accessible to user
  - Return podmioty with assigned permissions
  - Include current session podmiot
- **POST** `/api/v1/users/{userId}/select-podmiot` - Switch active podmiot context
  - Update session context
  - Return new JWT with podmiot claim
  - Trigger context reload for frontend

#### Role-Based Access Control (RBAC)
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

#### Permission Management
- Permissions by module: Sprawozdania (Read/Write/Manage), Sprawy (Read/Write), Wiadomości (Read/Write), Biblioteka (Read/Write/Manage), Komunikaty (Read/Write/Manage), FAQ (Read/Write/Answer), Kartoteka (Read/Write/Manage), Wnioski (Read/Approve/Manage), Admin Panel (Full)
- Administrator Podmiotu can modify permissions for Pracownik Podmiotu: Sprawozdawczość (access/no access), Sprawy (access/no access)
- Administrator Podmiotu can block/unblock user access to system
- UKNF can block Administrator Podmiotu (affects approval workflow)

---

### Communication Module

#### Reports (Sprawozdania)
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

#### Report Statuses
- `Robocze` - Draft state after file added
- `Przekazane` - Validation started with unique ID
- `W trakcie` - Validation in progress
- `Proces walidacji zakończony sukcesem` - No validation errors
- `Błędy z reguł walidacji` - Validation rule failures
- `Błąd techniczny w procesie walidacji` - Technical processing error
- `Błąd - przekroczono czas` - 24h timeout exceeded
- `Zakwestionowane przez UKNF` - Manually rejected by UKNF staff

#### Messages (Wiadomości)
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

#### Message Statuses
- `Oczekuje na odpowiedź UKNF` - External user sent message
- `Oczekuje na odpowiedź Użytkownika` - UKNF staff replied
- `Zamknięty` - Conversation completed

#### File Repository / Library (Biblioteka)
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

#### Administrative Cases (Sprawy)
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

#### Case Statuses
- `Wersja robocza` - Draft, not visible to UKNF
- `Nowa sprawa` - Submitted/launched, visible to UKNF
- `W toku` - Opened by staff or external user
- `Do uzupełnienia` - Requires user action/attachment (set by UKNF)
- `Zakończona` - Closed by UKNF
- `Anulowana` - Cancelled before user read

#### Announcements (Komunikaty)
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

#### Announcement Priority Levels
- `Niski` - Low priority
- `Średni` - Medium priority
- `Wysoki` - High priority (requires read confirmation)

#### Recipients, Contact Groups, Contacts
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

#### FAQ (Baza pytań i odpowiedzi)
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

#### FAQ Question Statuses
- `Draft` - Not submitted
- `Submitted` - Pending answer
- `Answered` - UKNF staff provided answer
- `Closed` - Question closed/archived

#### Podmiot Registry (Kartoteka Podmiotów)
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

#### Podmiot Data Schema
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

#### Podmiot Data Updater Service (Aktualizator danych podmiotu)
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

### Administration Module

#### User Management
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

#### Password Policy Configuration
- **GET** `/api/v1/admin/password-policy` - Get current password policy
  - Return: min length, max length, complexity requirements, expiration days, history length, lockout threshold
- **PUT** `/api/v1/admin/password-policy` - Update password policy (Admin only)
  - Fields: minLength (8-32), requireUppercase, requireLowercase, requireNumbers, requireSpecialChars, expirationDays (0 = never), historyLength, failedLoginLockout
  - Validate configuration
  - Apply policy to new password changes
- **POST** `/api/v1/admin/users/{id}/force-password-change` - Force password change for user
  - Set flag requiring password change on next login

#### Role Management
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

#### Permission Matrix
Modules: Sprawozdania, Sprawy, Wiadomości, Biblioteka, Komunikaty, FAQ, Kartoteka, Wnioski, Admin Panel
Permission Levels: None, Read, Write, Delete, Manage

#### Audit Trail
- **GET** `/api/v1/admin/audit-log` - Get audit log with filters
  - Query params: `userId`, `action`, `entity`, `dateFrom`, `dateTo`, `correlationId`
  - Return: timestamp, user, action, entity type, entity ID, changes, IP address, correlation ID
- **GET** `/api/v1/admin/audit-log/{id}` - Get audit log entry details
  - Include before/after snapshots for data changes

---

### File Management

#### Chunked File Upload
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

#### File Storage Abstraction
- Interface: `IFileStorageService`
- Methods: `StoreAsync`, `RetrieveAsync`, `DeleteAsync`, `GetMetadataAsync`
- Support: local disk, Azure Blob Storage, AWS S3 (future)
- Virus scanning hook: `IScanService.ScanFileAsync(fileId)` - returns scan status

---

### Background Jobs & Async Processing

#### Queue Integration
- Abstract behind `IMessageQueue` interface
- Implementations: In-memory (dev), Kafka, RabbitMQ
- Use cases:
  - Report validation workflow (trigger external service)
  - Email notifications (registration, access request status, komunikat publishing)
  - Periodic data confirmation alerts for podmioty
  - File virus scanning
  - Bulk operations (export, bulk user actions)

#### Scheduled Jobs
- Report validation timeout checker (runs every 1h, flags reports > 24h as "Błąd - przekroczono czas")
- Password expiration notifier (runs daily, warns users N days before expiration)
- Data confirmation alert generator (runs weekly, prompts users to confirm podmiot data)
- Komunikat expiration handler (runs daily, unpublishes expired announcements)

---

### Health Checks & Monitoring

#### Health Endpoints
- **GET** `/health/ready` - Readiness probe
  - Check database connectivity
  - Check queue connectivity
  - Check file storage access
  - Return 200 if all healthy, 503 if any unhealthy
- **GET** `/health/live` - Liveness probe
  - Simple ping response
  - Return 200 always (if process is running)

#### Metrics & Telemetry
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

---

### Data Validation Rules

#### General Validation
- All inputs: XSS prevention, SQL injection guards (parameterized queries only)
- File uploads: MIME type validation, magic number checks, size limits
- Email: RFC 5322 format validation
- Dates: ISO 8601 format, range validation (not future for historical dates)

#### Polish-Specific Validators
- **PESEL**: 11-digit format, checksum validation, mask to show only last 4 digits in API responses
- **NIP**: 10-digit format, checksum validation
- **KRS**: 10-digit format
- **LEI**: 20-character alphanumeric ISO 17442 format
- **Phone**: International format pattern `/^\+(?:[0-9] ?){6,14}[0-9]$/`

#### FluentValidation Rules
Create validators for all command/query DTOs:
- `RegisterUserCommandValidator`
- `CreateAccessRequestCommandValidator`
- `UploadReportCommandValidator`
- `CreateCaseCommandValidator`
- `CreateAnnouncementCommandValidator`
- `UpdatePodmiotCommandValidator`
- etc.

---

### API Conventions

#### RESTful URL Structure
- `/api/v1/<resource>` for all endpoints
- Plural nouns for collections: `/api/v1/reports`, `/api/v1/messages`
- Nested resources for relationships: `/api/v1/cases/{id}/messages`
- Query parameters for filters, pagination, sorting

#### HTTP Methods
- **GET**: Retrieve resource(s), idempotent, cacheable
- **POST**: Create resource, non-idempotent
- **PUT**: Update resource (full replacement), idempotent
- **PATCH**: Partial update (use sparingly)
- **DELETE**: Remove resource, idempotent

#### Response Formats
- Success: `200 OK` (GET, PUT, PATCH), `201 Created` (POST with Location header), `204 No Content` (DELETE)
- Client errors: `400 Bad Request` (validation), `401 Unauthorized` (auth required), `403 Forbidden` (insufficient permissions), `404 Not Found`
- Server errors: `500 Internal Server Error`, `503 Service Unavailable`
- Use RFC 7807 ProblemDetails for error responses:
```json
{
  "type": "https://api.uknf.gov.pl/errors/validation-error",
  "title": "Validation Error",
  "status": 400,
  "detail": "PESEL format is invalid",
  "instance": "/api/v1/auth/register",
  "errors": {
    "PESEL": ["Must be 11 digits"]
  }
}
```

#### Pagination
- Use query parameters: `?page=1&pageSize=25`
- Return pagination metadata in `X-Pagination` header:
```json
{
  "currentPage": 1,
  "pageSize": 25,
  "totalCount": 150,
  "totalPages": 6,
  "hasPrevious": false,
  "hasNext": true
}
```
- Include HAL-style links in response body (optional):
```json
{
  "data": [...],
  "_links": {
    "self": "/api/v1/reports?page=1&pageSize=25",
    "next": "/api/v1/reports?page=2&pageSize=25"
  }
}
```

#### Filtering & Sorting
- Filters: `?status=Nowy&priority=Wysoki`
- Sorting: `?sortBy=createdDate&sortOrder=desc`
- Search: `?search=Instytucja+Pożyczkowa`

---

### Security Implementation

#### Authentication Flow
1. User registers → receives activation email
2. User activates account → sets password → receives JWT access token + refresh token
3. Access token expires after 15 minutes
4. Refresh token expires after 7 days
5. Client refreshes access token using refresh token before expiry
6. On logout: invalidate refresh token

#### JWT Claims
- `sub` (subject): User ID
- `email`: User email
- `roles`: Array of role names
- `podmiotId`: Current active podmiot (for external users)
- `iat` (issued at): Timestamp
- `exp` (expiration): Timestamp
- `jti` (JWT ID): Unique token identifier (for revocation)

#### CSRF Protection
- Include anti-forgery tokens in state-changing requests (POST, PUT, DELETE)
- Validate token in middleware
- Use `SameSite=Strict` cookie attribute for refresh tokens

#### Input Validation
- FluentValidation for command/query DTOs (business rules)
- Data Annotations for model binding (simple format checks)
- Defence-in-depth: validate at API boundary AND before database persistence

#### Secrets Management
- Store secrets in: User Secrets (dev), Azure Key Vault (production)
- Never hardcode: connection strings, API keys, signing keys, passwords
- Use configuration providers: `IConfiguration`, `IOptionsSnapshot<T>`

#### GDPR Compliance
- Soft delete for user data (set `IsDeleted` flag, preserve audit trail)
- Purge jobs for permanent deletion after retention period
- Data export endpoint: **GET** `/api/v1/users/{id}/export-data` (return all user data in JSON)
- Consent tracking for data processing (future enhancement)

---

### Testing Strategy

#### Unit Tests (xUnit + FluentAssertions)
- Test all command/query handlers (CQRS)
- Test validators (FluentValidation)
- Test domain entities and value objects
- Mock dependencies: `IRepository<T>`, `IFileStorageService`, `IMessageQueue`
- Coverage target: 80%+ for new code

#### Integration Tests (WebApplicationFactory)
- Test API endpoints end-to-end
- Use in-memory database or containerized SQL Server (Testcontainers)
- Test happy paths AND failure scenarios
- Test authentication/authorization (valid/invalid JWT, insufficient permissions)
- Test data access (custom queries, projections)

#### Test Data
- Use provided test data: podmioty CSV import, sample reports (Q1_2025.xlsx correct, Q2_2025.xlsx incorrect)
- Seed test database with: roles, sample podmioty, sample users, FAQ categories
- Use builders/factories for test entity creation

#### Example Test Structure
```csharp
public class CreateAccessRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesAccessRequestWithStatusRoboczy()
    {
        // Arrange
        var handler = new CreateAccessRequestCommandHandler(mockRepo, mockMediator);
        var command = new CreateAccessRequestCommand { UserId = 1, ... };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(AccessRequestStatus.Roboczy);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<AccessRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidPESEL_ThrowsValidationException()
    {
        // Arrange, Act, Assert
        ...
    }
}
```

---

### Database Schema Guidelines

#### Entity Framework Core
- Use Code-First migrations: `dotnet ef migrations add <name>`
- Configure entities with Fluent API in `DbContext.OnModelCreating`
- Use temporal tables for audit history (EF Core 6+):
```csharp
modelBuilder.Entity<Podmiot>()
    .ToTable("Podmioty", tb => tb.IsTemporal());
```

#### Naming Conventions
- Tables: PascalCase plural nouns (e.g., `Podmioty`, `AccessRequests`)
- Columns: PascalCase (e.g., `NazwaPodmiotu`, `CreatedDate`)
- Foreign keys: `<EntityName>Id` (e.g., `PodmiotId`, `UserId`)
- Indexes: `IX_<TableName>_<ColumnName>`

#### Soft Delete Pattern
- Add `IsDeleted` (bit) and `DeletedDate` (datetime2) columns to all entities
- Configure global query filter:
```csharp
modelBuilder.Entity<User>()
    .HasQueryFilter(u => !u.IsDeleted);
```

#### Audit Columns
- All entities: `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`
- Populate automatically in `SaveChangesAsync` override

---

### Sample API Endpoint Signatures

#### Authentication
```csharp
[HttpPost("api/v1/auth/register")]
public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    => CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);

[HttpPost("api/v1/auth/activate")]
public async Task<IActionResult> Activate([FromBody] ActivateAccountCommand command)
    => Ok(new { accessToken, refreshToken, expiresIn = 900 });
```

#### Access Requests
```csharp
[HttpGet("api/v1/access-requests")]
[Authorize]
public async Task<IActionResult> GetAccessRequests([FromQuery] GetAccessRequestsQuery query)
    => Ok(new { data = result.Items, _links = result.Links });

[HttpPost("api/v1/access-requests/{id}/approve")]
[Authorize(Roles = "Pracownik UKNF,Administrator Podmiotu")]
public async Task<IActionResult> ApproveAccessRequest(long id)
    => NoContent();
```

#### Reports
```csharp
[HttpPost("api/v1/reports")]
[Authorize]
[RequestSizeLimit(100_000_000)] // 100MB
public async Task<IActionResult> UploadReport([FromForm] UploadReportCommand command)
    => Accepted(new { reportId, validationJobId });

[HttpGet("api/v1/reports/{id}/validation-report")]
[Authorize]
public async Task<IActionResult> DownloadValidationReport(long id)
    => File(fileStream, "application/pdf", $"validation_report_{id}.pdf");
```

#### Messages
```csharp
[HttpPost("api/v1/messages")]
[Authorize]
public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand command)
    => CreatedAtAction(nameof(GetMessage), new { id = result.MessageId }, result);
```

#### Podmioty
```csharp
[HttpGet("api/v1/podmioty")]
[Authorize]
public async Task<IActionResult> GetPodmioty([FromQuery] GetPodmiotyQuery query)
    => Ok(new { data = result.Items, _links = result.Links });

[HttpPost("api/v1/podmioty/{id}/request-change")]
[Authorize(Roles = "Administrator Podmiotu,Pracownik Podmiotu")]
public async Task<IActionResult> RequestPodmiotDataChange(long id, [FromBody] RequestDataChangeCommand command)
    => Accepted(new { caseId = result.CaseId });
```

#### Admin
```csharp
[HttpPut("api/v1/admin/password-policy")]
[Authorize(Roles = "Administrator systemu")]
public async Task<IActionResult> UpdatePasswordPolicy([FromBody] UpdatePasswordPolicyCommand command)
    => NoContent();
```

---

### OpenAPI / Swagger Documentation

#### Swashbuckle Configuration
- Enable XML comments: `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Document all public endpoints with summary and remarks
- Include request/response examples
- Document authentication schemes (Bearer JWT)
- Group endpoints by tags (Auth, Reports, Messages, Cases, etc.)
- Generate `swagger.json` for client code generation

#### Example Documentation
```csharp
/// <summary>
/// Upload a new report (XLSX format)
/// </summary>
/// <remarks>
/// Uploads a report file for validation. The file must be in XLSX format
/// and follow the template downloaded from the Library.
/// After upload, an asynchronous validation job is triggered.
/// Status will transition: Robocze → Przekazane → W trakcie → Final status.
/// Maximum file size: 100MB.
/// </remarks>
/// <param name="command">Upload command with file and metadata</param>
/// <returns>Report ID and validation job ID</returns>
/// <response code="202">Accepted - validation job started</response>
/// <response code="400">Bad Request - invalid file format or size</response>
/// <response code="401">Unauthorized - JWT required</response>
[HttpPost("api/v1/reports")]
[ProducesResponseType(typeof(UploadReportResponse), StatusCodes.Status202Accepted)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UploadReport([FromForm] UploadReportCommand command) { ... }
```
