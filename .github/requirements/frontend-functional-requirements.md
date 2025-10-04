# Frontend Functional Requirements

This document contains detailed UI/UX specifications and feature requirements for the UKNF Communication Platform frontend.

---

## Role-Based UI Customization

Adapt UI visibility and functionality based on user roles:

### Użytkownicy zewnętrzni (External Users)
- **Pracownik Podmiotu Nadzorowanego**: Access to sprawozdania submission, sprawy, wiadomości, biblioteka (read), komunikaty (read), FAQ, own podmiot data view
- **Administrator Podmiotu Nadzorowanego**: All Pracownik rights + manage access requests for their podmiot users, modify user permissions (Sprawozdawczość, Sprawy), block/unblock users

### Użytkownicy wewnętrzni (Internal Users)
- **Pracownik UKNF**: Full read access, manage own "Moje podmioty" list, respond to messages/cases, review sprawozdania, create komunikaty, answer FAQ, limited admin actions
- **Administrator systemu**: Full system access including user management, role assignment, password policy, podmiot data editing, all UKNF staff capabilities

### UI Elements to Toggle
- Navigation menu items (show/hide based on role)
- Action buttons (Create, Edit, Delete, Approve, Reject, Archive, Zakwestionuj)
- Table columns (e.g., "Assigned Staff" only for UKNF)
- Quick filters (e.g., "Moje podmioty" only for UKNF staff)
- Form fields (read-only vs. editable based on role and ownership)

### Context Switching (External Users)
- Users with multiple podmiot assignments see podmiot switcher
- Switching podmiot reloads relevant data scoped to selected podmiot
- Current podmiot/role displayed prominently in header
- Breadcrumb updates to reflect podmiot context

---

## Dashboard & Navigation

### Dashboard Layout
Use kafelki (tile/card) pattern with PrimeNG `p-card` for:

- **Dostępne podmioty** (available entities) with quick access
  - Display list of entities user represents with click-to-switch functionality
  
- **Status wniosków o dostęp** (access request statuses) with counts
  - Show numbers for: Roboczy, Nowy, Zaakceptowany, requiring action
  
- **Nowe wiadomości i powiadomienia** (messages/notifications)
  - Use `p-badge` counters showing unread counts
  
- **Statusy sprawozdań** (report statuses) with color-coded `p-tag` labels:
  - "Proces walidacji zakończony sukcesem" (success/green)
  - "W trakcie" (warning/orange)
  - "Błędy z reguł walidacji" (danger/red)
  - "Błąd techniczny" (danger/red)
  - "Zakwestionowane przez UKNF" (danger/red)
  
- **Tablica ogłoszeń** (announcement board)
  - Recent komunikaty with priority badges and unread indicators
  
- **Panel zadań** (task panel) for pending actions
  - "do zrobienia" items with timestamps
  
- **Ostatnie sprawy**
  - Recent administrative cases with status and podmiot filter
  
- **Wskaźniki bezpieczeństwa**
  - Last successful login, last password change, account activity

### Navigation
- Implement `p-breadcrumb` on all subpages for context
- Main menu with role-based visibility:
  - Dashboard
  - Sprawozdania
  - Wiadomości
  - Sprawy
  - Biblioteka
  - Komunikaty
  - FAQ
  - Kartoteka Podmiotów (admin)
  - Wnioski o dostęp (admin)

### Header
Include podmiot/role switcher (`p-dropdown`) permanently visible in header:
- Display current podmiot name and user role
- Allow instant switching between represented entities
- Update context across entire application when switched
- Show full name, email, and podmiot in user profile menu

### Timeline
Use `p-timeline` for activity feeds showing recent events with specific format:
- "10.09.2025, 11:00: Nowy komunikat w tablicy ogłoszeń"
- "10.09.2025, 09:45: Złożono sprawozdanie 'XYZ'"
- "09.09.2025, 17:30: Zmieniono uprawnienia użytkownika"
- Include event icons, timestamps, and contextual colors

---

## Interactive Tables & Lists

Implement list screens with PrimeNG `p-table` that support ALL of the following:

### Quick Search
- Visible search field above table for global filtering across all columns

### Column Sorting
- Click header to sort ascending/descending
- Double-click to reset to default order
- Show sort indicators

### Advanced Filtering
- Header filters with `p-dropdown`, `p-multiSelect`, `p-calendar` for date ranges
- Support multiple simultaneous filters with clear "Reset Filters" button
- Filter by status labels, types, date ranges, categories

### Sticky Headers
- Use `[scrollable]="true"` and `scrollHeight` to keep headers visible during scroll

### Pagination
- `p-paginator` with configurable rows per page (10, 25, 50, 100 options)

### Row Actions
- Contextual actions (Podgląd, Edytuj, Pobierz, Usuń) as icons or `p-menu` hamburger in each row

### Export Functionality
- Dedicated "Eksportuj" button with `p-menu` offering XLSX, CSV, JSON formats
- Export respects current filters, sorting, and selected columns
- Show `p-progressSpinner` during export and `p-toast` notification on completion
- Include export limit information if applicable

### Selection
- Support multi-row selection with checkboxes when batch operations are needed

---

## Forms & Validation

### Polish-Specific Validators
- **PESEL**: Mask to show only last 4 digits (e.g., `*******1234`), validate 11-digit format and checksum
- **NIP**: Validate 10-digit format with proper checksum
- **KRS**: Validate 10-digit format
- **LEI**: Validate 20-character alphanumeric ISO 17442 format
- **Phone**: International format validation using pattern `/^\+(?:[0-9] ?){6,14}[0-9]$/`
- **Email**: Standard email validation with `@` symbol

### Validation Feedback
- Surface field-level validation errors immediately (on blur)
- Server validation feedback after submission
- Use `p-message` or `p-messages` for inline validation feedback

---

## Rich Text Editing

Use **PrimeNG Editor** (`p-editor` - Quill-based WYSIWYG) for:
- Komunikaty (announcements) composition
- Message bodies with formatting
- FAQ answers

### Configuration
- Toolbar with: bold, italic, underline, lists, links, headings
- Sanitize HTML output to prevent XSS attacks using Angular's DomSanitizer
- Validate content length and warn users approaching limits

---

## File Management

### Upload Features
File flows must support chunked uploads, progress indicators, resumable retry, and accessible drag-and-drop.

Use `p-fileUpload` with:
- Multiple file selection
- Drag-and-drop zone with visual feedback and keyboard accessibility
- Progress bar (`p-progressBar`) for each file
- File type and size validation (PDF, DOC/DOCX, XLS/XLSX, CSV/TXT, MP3, ZIP)
- Reject files over 100MB total (before ZIP compression)
- Reject unexpected file formats with clear error message
- Preview thumbnails for images
- Cancel/retry for failed uploads
- Virus scanning preparation - show "Scanning..." state, integrate with backend scanning service hooks

### File Operations
- Support ZIP compression/decompression on download
- Display file metadata (name, size, upload date, version, uploader)
- Implement file versioning UI with history view showing all versions with download links
- Show hierarchical folder structure with expandable tree (`p-tree` or `p-treeTable`)

### Permissions Management
Include permissions management UI for file sharing:
- Share with all users, selected podmiot types, selected podmioty, selected user groups, individual users
- Permission levels: view, download, edit (for UKNF staff)

### Biblioteka (File Repository)
- UKNF staff can add/modify/delete files and metadata
- Metadata fields: Nazwa pliku, Okres sprawozdawczy, Data aktualizacji wzoru, Załącznik
- Categorization and filtering by type, period, update date
- Version marking: current vs. archival with last update date
- Access control: grant specific users/groups permission to view/download
- File history log showing all modifications
- Search by metadata and content (prepare hooks for full-text search)

---

## Contact Management & Groups

### Adresaci (Recipients) Management
Four recipient types with dedicated selection interfaces:

1. **Wybrane typy podmiotów**: Multi-select dropdown of podmiot types (e.g., "Instytucja Pożyczkowa")
2. **Wybrane podmioty**: Multi-select table/autocomplete from full podmiot list
3. **Wybrani użytkownicy**: Multi-select table/autocomplete from external users
4. **Wybrane grupy kontaktów**: Multi-select from saved contact groups

**Features**:
- Preview selected recipients count before sending
- Save recipient selections as reusable templates

### Grupy Kontaktów (Contact Groups)
- Create/edit/delete contact groups
- Add external users to groups (from system users)
- Add contacts to groups (non-system users with email only)
- Group member management table with add/remove actions
- Display group usage statistics (how many komunikaty/wiadomości sent)

### Kontakty (Contacts)
- Add individuals who are NOT system users
- Fields: Imię, Nazwisko, Email, Telefon, Podmiot (optional association)
- Email-only notification capability (no system login)
- Use for external stakeholders who receive notifications
- Bulk import from CSV

---

## FAQ Management

### Question Submission Interface
- Anonymous and authenticated submission options
- Fields: Tytuł, Treść, Kategoria (dropdown), Etykiety (tag input), Status
- Rich text editor for question body
- Status: Draft, Submitted, Answered, Closed

### FAQ Answer Management (UKNF Staff)
- Pending questions queue with filter by category/status
- WYSIWYG answer composer (`p-editor`)
- Assign categories and tags
- Publish/unpublish controls
- Edit/delete existing Q&A pairs

### FAQ Browse Interface
- Search bar with keyword matching (title, content, tags)
- Category filter sidebar with counts
- Tag cloud for popular tags
- Sort options: Popularność (most viewed), Data dodania (newest first), Oceny (highest rated)
- Rating display (1-5 stars) with `p-rating` component
- User rating submission (one rating per user per answer)
- View counter for each Q&A
- Expandable/collapsible answers using `p-accordion` or `p-panel`
- Related questions suggestions at bottom

### FAQ Analytics (UKNF Staff)
- Most viewed questions dashboard
- Average ratings per category
- Unanswered questions alert

---

## Podmiot Data Management (Kartoteka Podmiotów)

### Podmiot List View (UKNF Staff/Admin)
- Comprehensive table with all podmiot metadata
- Filters: Typ podmiotu, Status podmiotu, Kategoria, Sektor, Podsektor, Podmiot transgraniczny
- Search by: Nazwa podmiotu, Kod UKNF, LEI, NIP, KRS
- Export podmiot list to XLSX/CSV
- Quick actions: View details, Edit, View users, View history

### Podmiot Details View
**Read-only for external users** (their assigned podmioty only)

Display all fields per schema:
- **Identifiers**: ID, Typ podmiotu, Kod UKNF, Nazwa podmiotu, LEI, NIP, KRS, Numer wpisu do rejestru UKNF
- **Address**: Ulica, Numer budynku, Numer lokalu, Kod pocztowy, Miejscowość
- **Contact**: Telefon (international format validation), E-mail
- **Classification**: Status podmiotu, Kategoria podmiotu, Sektor podmiotu, Podsektor podmiotu, Podmiot transgraniczny (checkbox)
- **Timestamps**: Data utworzenia, Data aktualizacji
- Associated users list with roles
- Change history timeline with versioning

### Podmiot Data Update (External Users - Aktualizator danych podmiotu)
- Editable fields for external users: Nazwa podmiotu, Ulica, Kod pocztowy, Miejscowość, Telefon, E-mail
- Form pre-filled with current data
- "Potwierdź dane" action if data is current
- Submit change request creates new sprawa with category "Zmiana danych rejestrowych"
- Periodic alert prompt: "Czy dane Twojego podmiotu są aktualne?" with confirmation dialog

### Change Verification (UKNF Staff)
- Pending data change requests queue
- Side-by-side comparison: current vs. requested data
- Approve/Reject actions
- Version history preservation
- Approval writes to Baza Podmiotów
- Automatic notification to requesting user on approval/rejection

---

## Administrative Module Features (Administrator systemu only)

### User Management Interface
- Combined table for internal and external users with role filter
- User CRUD operations: Create, Edit, Activate/Deactivate, Delete
- Password reset action - manual override for locked accounts
- Role assignment interface with checkbox or multi-select
- Bulk actions: Activate, Deactivate, Assign role
- Search/filter by: Name, Email, Role, Status (Active/Inactive), Podmiot (for external users)

### Password Policy Configuration
Form-based policy editor:
- Minimum length (slider/number input, e.g., 8-32 characters)
- Complexity requirements (checkboxes): Uppercase, Lowercase, Numbers, Special characters
- Password expiration period (days, 0 = never expires)
- Password history length (prevent reuse of last N passwords)
- Failed login attempt limit before lockout
- "Force Password Change" action for selected users
- Real-time password strength indicator in user forms based on current policy
- Preview current policy summary panel

### Role Management Interface
- Role creation form: Role name, Description
- Permission assignment matrix:
  - **Rows**: System features/modules (Sprawozdania, Sprawy, Wiadomości, Biblioteka, Komunikaty, FAQ, Kartoteka, Wnioski, Admin Panel)
  - **Columns**: Permission levels (None, Read, Write, Delete, Manage)
  - Checkboxes or toggles for each permission
- Role assignment to users (multi-select user picker)
- Pre-defined system roles (read-only): Administrator systemu, Pracownik UKNF, Administrator Podmiotu, Pracownik Podmiotu
- Custom role creation for granular permissions
- Role usage analytics: Number of users per role
- Audit log for role changes

---

## Messaging & Communication

### Wiadomości
Email-like interface with:
- Inbox/Sent/Drafts views using `p-table`
- Thread grouping and filtering by podmiot
- Attachment support with multiple files (PDF, DOC/DOCX, XLS/XLSX, CSV/TXT, MP3, ZIP max 100MB total before compression)
- File format validation - reject unexpected formats
- WYSIWYG composer using `p-editor`
- Read/unread indicators with `p-badge`
- Status indicators: "Oczekuje na odpowiedź UKNF", "Oczekuje na odpowiedź Użytkownika", "Zamknięty"
- Integration with other modules (wnioski, sprawy, sprawozdania) for contextual messaging
- Bulk messaging to podmiot groups/types

### Komunikaty (Announcements)
- Priority levels (Niski, Średni, Wysoki) displayed with color-coded `p-tag`
- Category filtering and assignment
- Expiration date display with countdown indicator
- **Read confirmation**: High-priority messages require explicit "Odczytano" button click with `p-dialog` confirmation
- Confirmation captures: date/time, user name (Imię, Nazwisko), podmiot name
- Show statistics: "71/100 podmiotów odczytało" with `p-progressBar`
- Display read timestamp, user name, and podmiot name on confirmation
- WYSIWYG editor for announcement composition
- Recipient definition: single users, user groups, podmiot types, all external users
- Attachment support
- Publication/unpublication controls
- Version history for edits
- Highlight new/unread komunikaty with visual badge on dashboard

---

## Sprawozdania (Reports Module)

### Report Submission Interface
- Excel XLSX template download from Biblioteka before upload
- File upload via `p-fileUpload` with XLSX format validation
- Automated validation workflow with status tracking
- Display validation report attachment after processing
- Show unique identifier after successful transmission ("Przekazane" status)
- Support report corrections (korekta) linked to original report
- Display korekta relationship in report details

### Report Status Dashboard
- Registry views: "Sprawozdania kwartalne", "Sprawozdania roczne", "Aktualne", "Archiwalne"
- Quick filters: "Moje podmioty" (for UKNF staff), status, reporting period
- Archive action for UKNF staff to move reports to archive
- Report calendar/schedule showing upcoming deadlines with reminders
- Missing report indicator - list podmioty that haven't submitted for selected period

### Report Details View
- Metadata: file name, number, reporting period, submitter (Imię, Nazwisko, Email, Telefon)
- Podmiot information
- Validation status with detailed error/success report
- Correction history if applicable
- Download original XLSX and validation report
- "Zakwestionuj" action for UKNF staff with "Opis nieprawidłowości" text field

### Report Validation States
- Visual workflow diagram showing current state
- Status descriptions with icons
- Automatic timeout indicator after 24h
- Error categorization: technical vs. validation rule errors

---

## Sprawy (Administrative Cases Module)

### Case Creation Form
- Category selection: "Zmiana danych rejestrowych", "Zmiana składu osobowego", "Wezwanie do Podmiotu Nadzorowanego", "Uprawnienia do Systemu", "Sprawozdawczość", "Inne"
- Priority selection: Niski, Średni, Wysoki
- Single podmiot association (mandatory)
- Case number auto-generation
- Attachment upload support
- Draft save functionality (status: "Wersja robocza")
- Submit/launch action (status: "Nowa sprawa")

### Case Management Interface
- "Teczka sprawy" (case folder) view with tabs: Details, Documents, Messages, History
- Process stage indicators with timeline
- Messaging thread integrated within case
- Attachment management panel
- Status change actions:
  - "Do uzupełnienia" - UKNF requests additional info
  - "Zakończona" - UKNF closes case
  - "Anulowana" - cancel before user reads (with "wiadomość anulowana" placeholder for user)
- Cancellation notification sent to podmiot
- History log showing all status changes, document additions, message exchanges

### Case List View
- Filterable table with: case number, podmiot, category, priority, status, assigned staff, creation date
- Quick filters for external users: "Moje sprawy", status
- Quick filters for UKNF staff: "Moje podmioty", status, category, priority
- Row actions: View details, Add message, Add document, Change status
- Visual priority indicators

---

## Access Request Workflows (Wnioski o dostęp)

### Registration & Initial Request
- Registration form: Imię, Nazwisko, PESEL (masked: *******1234), Telefon, Email
- Email activation link sent
- Password setup with policy enforcement
- Auto-generation of access request with status "Roboczy"

### Access Request Form (Linia Uprawnień)
- Podmiot selection from Katalog Podmiotów (multi-select for multiple permission lines)
- Permission checkboxes per podmiot: Sprawozdawczość, Sprawy, Administrator podmiotu
- Podmiot email field - assigns email for automatic notifications
- Submit confirmation with email notification
- Status change: Roboczy → Nowy

### Request Review Interface (UKNF/Admin)
- Table view of all requests with filters: "Moje podmioty", "Wymaga działania UKNF", "Obsługiwany przez UKNF"
- Permission line details showing requested permissions per podmiot
- Approve/Reject actions per permission line
- Messaging capability within request context
- Attachment request/upload for verification documents
- Status updates: Nowy → Zaakceptowany/Zablokowany

### Permission Management (Administrator Podmiotu)
- View all users for their podmiot
- Modify permissions: Sprawozdawczość (access/no access), Sprawy (access/no access)
- Block/unblock user access
- Cannot modify other Administrators unless UKNF grants permission

### Admin Blocking Rules
- UKNF can block Administrator Podmiotu
- If other Admins exist for podmiot, they retain approval rights
- If no other Admins, UKNF must approve permission changes
- Blocking Admin doesn't affect already-approved user permissions

---

## Status Labels & Badges

Create a consistent status labeling system using `p-tag`:

### Sprawozdania (Reports)
- Robocze (secondary/gray) - draft state after file added
- Przekazane (info/blue) - validation started with unique ID
- W trakcie (warning/orange) - validation in progress
- Proces walidacji zakończony sukcesem (success/green) - no validation errors
- Błędy z reguł walidacji (danger/red) - validation rule failures
- Błąd techniczny w procesie walidacji (danger/red) - technical processing error
- Błąd - przekroczono czas (danger/red) - 24h timeout exceeded
- Zakwestionowane przez UKNF (danger/red) - manually rejected by UKNF staff

### Wnioski (Access Requests)
- Roboczy (secondary/gray) - not yet submitted
- Nowy (info/blue) - submitted, pending approval
- Zaakceptowany (success/green) - all permission lines approved
- Zablokowany (danger/red) - all permission lines blocked
- Zaktualizowany (warning/orange) - modified, awaiting re-approval

### Sprawy (Cases)
- Wersja robocza (secondary/gray) - draft, not visible to UKNF
- Nowa sprawa (info/blue) - submitted/launched
- W toku (warning/orange) - opened by staff or external user
- Do uzupełnienia (warning/orange) - requires user action/attachment
- Zakończona (success/green) - closed by UKNF
- Anulowana (danger/red) - cancelled before user read

### Wiadomości (Messages)
- Oczekuje na odpowiedź UKNF (info/blue) - external user sent
- Oczekuje na odpowiedź Użytkownika (warning/orange) - UKNF staff replied
- Zamknięty (secondary/gray) - conversation completed

### Komunikaty Priority
- Niski (secondary/gray)
- Średni (info/blue)
- Wysoki (danger/red) - requires read confirmation

**Use severity levels**: `success`, `info`, `warning`, `danger`, `secondary`

