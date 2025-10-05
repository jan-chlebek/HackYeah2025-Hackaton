# UI Requirements vs Implementation Analysis

**Date**: 2025-10-05
**Purpose**: Gap analysis between UI requirements and current Angular implementation
**Focus**: Frontend UI components only

---

## Executive Summary

### Current Implementation Status
- **Total Components Required**: ~35 screens/components
- **Fully Implemented**: 4 components (11%)
- **Partially Implemented**: 3 components (9%)
- **Stub Only**: 28 components (80%)

### Key Findings
1. ✅ **Basic architecture established**: Routing, layout, services exist
2. ✅ **App shell complete**: Header, menu, accessibility features working
3. 🟡 **Limited component implementation**: Only 4 fully working screens
4. ❌ **No PrimeNG integration**: Despite being specified in requirements
5. ❌ **No API integration**: Services exist but not connected to backend
6. ❌ **No state management**: NgRx Signal Store mentioned but not implemented

---

## 1. UI Requirements from Prototypes

Based on `UI_SCREENS_SUMMARY.md` - 8 documented prototype screens:

### 1.1 Dashboard (Screen 00 & 01)
**Requirements**:
- Statistics cards (Messages, Cases, Documents count)
- Quick action buttons
- Recent activity feed
- Widget-based layout
- Welcome message
- Navigation menu

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Basic layout exists in `dashboard.component.ts` (172 lines)
- ✅ Three tabs: Pulpit, Wnioski, Biblioteka
- ✅ Table with pagination, sorting, filtering
- ❌ No statistics cards
- ❌ No quick action buttons
- ❌ No recent activity feed
- ❌ No widget-based layout
- ❌ Using mock data instead of API

**Gap**: Dashboard has table view but missing dashboard widgets, statistics, and quick actions from prototype

---

### 1.2 Access Request Preview (Screen 02)
**Requirements**:
- Form preview (read-only mode)
- Applicant information section
- Request details section
- Attachments list with download
- Status information
- Action buttons: Approve, Reject, Back, Save as Draft
- Long scrollable page with section separators

**Implementation Status**: ✅ **IMPLEMENTED**
- ✅ Fully implemented in `wnioski-details.component.ts` (140 lines)
- ✅ Podmiot table with data
- ✅ Ownership details table
- ✅ Attachments section with download
- ✅ Action buttons (Approve, Reject, Save Draft, Back)
- ✅ Proper form structure
- ⚠️ Using mock data (not API integrated)

**Gap**: Component complete but needs API integration

---

### 1.3 File Repository - Browse (Screen 03)
**Requirements**:
- Table-based file listing
- Filter panel (left sidebar):
  - Document type
  - Date added (range picker)
  - Status (Active/Archived)
  - Entity dropdown
- File table columns:
  - Checkbox (bulk selection)
  - Filename with icon
  - Type (badge)
  - Date added
  - Size (KB/MB)
  - Added by (user)
  - Actions (view, download, edit, delete icons)
- Sortable columns
- Sticky header
- Pagination
- Bulk actions bar: Download selected, Delete selected, Move to...
- Search bar + "Add file" button

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Implemented in `biblioteka-list.component.ts` (365 lines)
- ✅ Table with sortable columns
- ✅ Pagination
- ✅ Search filtering
- ✅ Mock data (8 files)
- ❌ No left sidebar filters
- ❌ No bulk selection checkboxes
- ❌ No file type badges
- ❌ No file size column
- ❌ No "Added by" column
- ❌ No action icons (view, download, edit, delete)
- ❌ No bulk actions bar
- ❌ Missing "Add file" button

**Gap**: Basic table exists but missing ~60% of required features (filters, bulk actions, action icons)

---

### 1.4 File Repository - Add File (Screen 04)
**Requirements**:
- Modal dialog or dedicated page
- Drag & drop upload area with:
  - Cloud/folder icon
  - "Drag files here or click to select" text
  - "Select files" button
  - Supported formats note
  - Max file size note (50 MB)
- File metadata form:
  - Filename (pre-filled)
  - Document type (dropdown)
  - Description (textarea, optional)
  - Tags (multi-select)
  - Related entity (dropdown)
- Additional options:
  - "Share with supervised entities" checkbox
  - "Require read confirmation" checkbox
  - Expiry date (date picker, optional)
- File preview after selection:
  - Filename with icon
  - File size
  - Progress bar during upload
  - Remove button (X)
- Action buttons: Cancel, Add (disabled until valid)
- Validation messages

**Implementation Status**: ❌ **NOT IMPLEMENTED**
- ❌ No upload component exists
- ❌ No drag & drop functionality
- ❌ No file metadata form
- ❌ No chunked upload (required for large files)

**Gap**: 100% missing - needs full implementation

---

### 1.5 Messages - List View (Screen 05)
**Requirements**:
- Split view: message list (40%) + preview pane (60%)
- Filter tabs: All, Unread (with badge), Starred, Sent, Archive
- Message list items showing:
  - Checkbox for selection
  - Star icon (toggle favorite)
  - Sender name (bold if unread)
  - Subject line (bold if unread)
  - Preview text (gray)
  - Timestamp (right aligned)
  - Attachment icon (if applicable)
  - Unread indicator (blue dot/bold)
- Visual states: unread, read, selected, hover
- Pagination at bottom
- Message preview panel:
  - From, To, Subject, Date/Time
  - Attachment indicator
  - Action buttons: Reply, Reply All, Forward, Delete, More dropdown
  - Full message body
  - Attachments section (if present)
  - Quick reply box (collapsed by default)

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Implemented in `wiadomosci-list.component.ts` (316 lines)
- ✅ Table with pagination
- ✅ Sorting functionality
- ✅ Search and column filtering
- ✅ Priority and status filters
- ❌ No split view (list + preview)
- ❌ No filter tabs (All, Unread, Starred, etc.)
- ❌ No star/favorite functionality
- ❌ No unread indicators
- ❌ No attachment icons
- ❌ No message preview pane
- ❌ Table-only view instead of inbox-style layout

**Gap**: Has data management but wrong UI pattern (table instead of inbox split view)

---

### 1.6 Messages - Filtered View (Screen 06)
**Requirements**:
- Three-column layout: filters (~20%) + list (~30%) + preview (~50%)
- Advanced filter panel with sections:
  - Status: Unread, Read, Starred checkboxes
  - Date: Last 7/30 days radio, Date range picker
  - Sender: Autocomplete/dropdown with recent senders
  - Message Type: System, Notifications, From entities, Internal checkboxes
  - Attachments: With/Without checkboxes
  - Priority: High, Normal, Low checkboxes
- Filter actions: "Apply filters" button, "Clear filters" link
- Active filters shown as removable chips/tags above list
- Collapsible filter panel (toggle button)
- Filter count indicator on toggle
- Result count: "Found X messages"

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Column filters implemented in `wiadomosci-list.component.ts`
- ✅ Date range filtering (from/to)
- ✅ Status and priority filters
- ✅ Checkbox filters: mojePodmioty, wymaganaOdpowiedzUKNF
- ❌ No three-column layout
- ❌ No filter sidebar panel
- ❌ Filters embedded in table headers (not left panel)
- ❌ No filter chips/tags
- ❌ No collapsible filter panel
- ❌ No result count display

**Gap**: Filter logic exists but UI presentation differs from requirement (inline vs. sidebar)

---

### 1.7 Messages - Details View (Screen 07)
**Requirements**:
- Full-page message display with thread view
- Breadcrumb: Messages > [Subject]
- Back button to message list
- Primary message section:
  - Avatar/icon, Sender name (bold), email, org badge, timestamp
  - Action bar: Reply, Reply All, Forward, Star, More dropdown
  - Message metadata: To (expandable), CC, Subject, Date
  - Full HTML content with formatting
  - Quoted text collapsed with "Show quoted text"
  - Attachments section with cards:
    - Large file icons by type
    - Filename, file size
    - Download + Preview buttons
    - "Download all as ZIP" option
    - Virus scan status indicator
- Thread view below:
  - Previous messages collapsed (headers only)
  - Click to expand
  - Visual threading (indent/connectors)
  - Chronological order
- Reply compose box at bottom:
  - Quick reply: expands compose area
  - Rich text editor with formatting toolbar
  - Attachment button
  - Send + Cancel buttons
  - Full compose: To/CC/BCC, Subject, templates, signature, send options
- Optional right sidebar:
  - Message metadata (related case, tags, folder, message ID)
  - Related messages

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Implemented in `wiadomosci-details.component.ts` (113 lines)
- ✅ Basic message display structure
- ✅ Priority, status fields
- ✅ User/UKNF message sections
- ✅ User attachments list with download
- ✅ UKNF attachments with selection
- ✅ Response textarea
- ✅ Action buttons: Cancel, Save & Send
- ❌ No breadcrumb navigation
- ❌ No avatar/icon display
- ❌ No action bar (Reply, Forward, Star, etc.)
- ❌ No message threading
- ❌ No rich text editor (plain textarea only)
- ❌ No attachment preview
- ❌ No "Download all as ZIP"
- ❌ No virus scan status
- ❌ No quoted text collapse
- ❌ No related messages sidebar

**Gap**: Basic form exists but missing ~50% of features (threading, rich editor, previews, metadata sidebar)

---

### 1.8 Common UI Patterns (All Screens)
**Requirements**:
- Top navigation bar: logo (left), user menu (right)
- Breadcrumb navigation for deep pages
- Side navigation menu (collapsible)
- Color scheme: Primary blue (KNF branding), white/light gray backgrounds, green/red/yellow accents
- Typography: Sans-serif, clear hierarchy, responsive sizes
- Primary buttons: Solid blue with white text
- Secondary buttons: Outlined or gray
- Icon buttons for common actions
- Hover states on all interactive elements
- Tables: sticky headers, sortable columns, row selection, action icons, pagination
- Forms: labels above fields, required indicators (*), inline validation, placeholders, logical grouping
- Accessibility: high contrast, keyboard navigation, screen reader compatibility, focus indicators, ARIA labels

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Top navigation implemented in `app.component.ts`
- ✅ User menu with session timer
- ✅ Side navigation menu (collapsible)
- ✅ Accessibility controls: font size, dark mode, high contrast
- ✅ Basic table patterns (sorting, pagination) in some components
- ❌ No breadcrumb navigation
- ❌ Inconsistent color scheme (not using KNF blue systematically)
- ❌ No PrimeNG components (using plain HTML/CSS)
- ❌ No icon buttons library integrated
- ❌ No form validation framework
- ❌ ARIA labels missing in many places
- ❌ Keyboard navigation not fully implemented

**Gap**: App shell is good but missing systematic UI component library integration

---

## 2. Additional Requirements from Functional Specs

From `DETAILS_UKNF_Prompt2Code2.md` - Functional requirements beyond prototypes:

### 2.1 Communication Module

#### 2.1.1 Report Submission (Sprawozdania)
**Requirements**:
- Accept reports from supervised entities
- Provide feedback/acknowledgment
- Link reports to cases

**Implementation Status**: ❌ **STUB ONLY**
- Route exists: `/sprawozdania`
- Component exists but empty: `sprawozdania-list.component.ts`
- No UI, no logic, no API integration

---

#### 2.1.2 Messaging (Wiadomości)
**Requirements**: (Covered in Screen 05, 06, 07 above)
- Bi-directional communication
- Attachment support
- Internal/external user communication

**Implementation Status**: 🟡 **PARTIAL** (as analyzed above)

---

#### 2.1.3 File Repository (Biblioteka)
**Requirements**: (Covered in Screen 03, 04 above)
- Local file library
- Chunked uploads for large files
- Metadata tagging
- Search and filtering
- Virus scanning (integration point)

**Implementation Status**: 🟡 **PARTIAL** (as analyzed above)
- Missing: chunked upload, virus scanning, comprehensive metadata

---

#### 2.1.4 Case Management (Sprawy)
**Requirements**:
- Create and manage cases related to supervised entities
- Link messages/documents to cases
- Case status tracking
- Assignment to staff members

**Implementation Status**: ❌ **STUB ONLY**
- Routes exist: `/sprawy`, `/sprawy/create/:id`, `/sprawy/:id`
- Components exist but empty:
  - `sprawy-list.component.ts`
  - `sprawy-create.component.ts`
  - `sprawy-details.component.ts`
- No UI, no logic, no API integration

---

#### 2.1.5 Announcements (Komunikaty)
**Requirements**:
- Bulletin board functionality
- Target all users or specific groups
- Read confirmation tracking
- Create, edit, delete announcements

**Implementation Status**: ❌ **STUB ONLY**
- Routes exist: `/komunikaty`, `/komunikaty/create`, `/komunikaty/:id`
- Components exist but empty:
  - `komunikaty-list.component.ts`
  - `komunikaty-create.component.ts`
  - `komunikaty-details.component.ts`
- No UI, no logic, no API integration

---

#### 2.1.6 Contacts & Groups
**Requirements**:
- Manage recipients (addressees)
- Contact groups
- Individual contacts

**Implementation Status**: ❌ **NOT STARTED**
- No routes defined
- No components created
- Mentioned in requirements but not scaffolded

---

#### 2.1.7 FAQ Management
**Requirements**:
- Question and answer database
- Users can ask questions
- Browse existing Q&A
- FAQ categorization

**Implementation Status**: ❌ **STUB ONLY**
- Routes exist: `/faq`, `/faq/create`, `/faq/:id`
- Components exist but empty:
  - `faq-list.component.ts`
  - `faq-create.component.ts`
  - `faq-details.component.ts`
- No UI, no logic, no API integration

---

#### 2.1.8 Entity Registry (Kartoteka/Podmioty)
**Requirements**:
- Maintain database of supervised entities
- Update entity data
- View entity details
- Link to cases/messages

**Implementation Status**: ❌ **STUB ONLY**
- Routes exist: `/kartoteka`, `/kartoteka/:id`, `/kartoteka/update/:id`
- Components exist but empty:
  - `kartoteka-list.component.ts`
  - `kartoteka-details.component.ts`
  - `kartoteka-update.component.ts`
- No UI, no logic, no API integration

---

### 2.2 Authentication & Authorization Module

#### 2.2.1 User Authentication
**Requirements**:
- OAuth2/OIDC with JWT
- Login screen
- Register screen
- Password reset
- Session management
- Multi-factor authentication (optional)

**Implementation Status**: ❌ **STUB ONLY**
- Routes exist: `/auth/login`, `/auth/register`, `/auth/password-reset`
- Components exist but empty:
  - `login.component.ts` (12 lines)
  - `register.component.ts` (12 lines)
  - `password-reset.component.ts` (12 lines)
- No forms, no validation, no API integration
- Session timer exists in app.component but no actual auth flow

---

#### 2.2.2 Authorization & Roles
**Requirements**:
- Role-based access control (RBAC)
- Permissions per feature
- User role assignment
- Route guards based on roles

**Implementation Status**: ❌ **NOT IMPLEMENTED**
- No route guards defined
- No role checking logic
- Admin routes exist but unprotected
- Auth service exists but not integrated

---

### 2.3 Administration Module

#### 2.3.1 User Management
**Requirements**:
- List users
- Create/edit/delete users
- Assign roles
- Activate/deactivate accounts

**Implementation Status**: ❌ **STUB ONLY**
- Route exists: `/admin/users`
- Component exists but empty: `admin-users.component.ts` (12 lines)
- No UI, no logic

---

#### 2.3.2 Role Management
**Requirements**:
- Define roles
- Assign permissions to roles
- Edit role properties

**Implementation Status**: ❌ **STUB ONLY**
- Route exists: `/admin/roles`
- Component exists but empty: `admin-roles.component.ts` (12 lines)
- No UI, no logic

---

#### 2.3.3 Password Policy
**Requirements**:
- Configure password complexity rules
- Password expiration settings
- Lockout policies

**Implementation Status**: ❌ **STUB ONLY**
- Route exists: `/admin/password-policy`
- Component exists but empty: `admin-password-policy.component.ts` (12 lines)
- No UI, no logic

---

### 2.4 Cross-Cutting UI Requirements

#### 2.4.1 Accessibility (WCAG 2.2)
**Requirements**:
- High contrast mode
- Font size controls
- Keyboard navigation
- Screen reader support
- ARIA labels
- Focus management

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Font size controls implemented (app.component)
- ✅ Dark mode toggle
- ✅ High contrast mode toggle
- ⚠️ Manual CSS classes (body.font-medium, body.dark-mode)
- ❌ ARIA labels incomplete
- ❌ Keyboard navigation not systematically implemented
- ❌ No focus trap in modals
- ❌ No skip-to-content links

---

#### 2.4.2 Responsive Design
**Requirements**:
- Mobile-friendly layouts
- Tablet optimization
- Desktop-first approach
- Breakpoints for different screen sizes

**Implementation Status**: ❌ **NOT VERIFIED**
- Components built but responsive behavior not tested
- No media queries observed in component CSS
- Tailwind CSS available but not extensively used
- PrimeNG components (responsive by default) not integrated

---

#### 2.4.3 Performance
**Requirements**:
- Lazy loading of routes
- Caching strategy
- Pagination for large lists
- Virtual scrolling for long tables

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Lazy loading configured in routes
- ✅ Pagination implemented in several list components
- ❌ No caching strategy (HTTP interceptor not configured)
- ❌ No virtual scrolling
- ❌ No service worker for offline support

---

#### 2.4.4 Error Handling & User Feedback
**Requirements**:
- Toast notifications for actions
- Error messages with guidance
- Loading indicators
- Confirmation dialogs for destructive actions
- Validation messages

**Implementation Status**: ❌ **MINIMAL**
- ⚠️ Some components have `isLoading` flags
- ⚠️ Some components have `errorMessage` strings
- ❌ No toast notification service integrated
- ❌ No confirmation dialog service
- ❌ No global error interceptor
- ❌ No loading spinner component

---

#### 2.4.5 Export Functionality
**Requirements**:
- Export tables to Excel
- Export to PDF
- Print views

**Implementation Status**: ❌ **NOT IMPLEMENTED**
- No export functionality in any component
- No print stylesheets

---

#### 2.4.6 Search & Filtering
**Requirements**:
- Global search across entities
- Advanced filters per module
- Saved filters
- Quick filters

**Implementation Status**: 🟡 **PARTIAL**
- ✅ Search implemented in: wiadomosci, biblioteka, dashboard
- ✅ Column-level filters in some tables
- ❌ No global search
- ❌ No saved filters
- ❌ No filter presets

---

## 3. Component Inventory Summary

### 3.1 Fully Implemented (4 components)
1. **App Component** (`app.component.ts`) - Layout shell with accessibility
2. **Dashboard Component** (`dashboard.component.ts`) - Basic table view with tabs
3. **Wnioski Details Component** (`wnioski-details.component.ts`) - Access request preview
4. **Wiadomosci List Component** (`wiadomosci-list.component.ts`) - Messages table with filters

---

### 3.2 Partially Implemented (3 components)
1. **Biblioteka List Component** (`biblioteka-list.component.ts`) - File table (missing filters, bulk actions)
2. **Wiadomosci Details Component** (`wiadomosci-details.component.ts`) - Message form (missing threading, rich editor)
3. **Wnioski List Component** (`wnioski-list.component.ts`) - Access requests table (basic structure)

---

### 3.3 Stub Components (28 components)

#### Authentication (3)
- `login.component.ts`
- `register.component.ts`
- `password-reset.component.ts`

#### Reports (1)
- `sprawozdania-list.component.ts`

#### Cases (3)
- `sprawy-list.component.ts`
- `sprawy-create.component.ts`
- `sprawy-details.component.ts`

#### File Library (2)
- `biblioteka-details.component.ts`
- (Missing: `biblioteka-upload.component.ts`)

#### Messages (1)
- `wiadomosci-compose.component.ts`

#### Access Requests (1)
- `wnioski-create.component.ts`

#### Announcements (3)
- `komunikaty-list.component.ts`
- `komunikaty-create.component.ts`
- `komunikaty-details.component.ts`

#### FAQ (3)
- `faq-list.component.ts`
- `faq-create.component.ts`
- `faq-details.component.ts`

#### Entity Registry (3)
- `kartoteka-list.component.ts`
- `kartoteka-details.component.ts`
- `kartoteka-update.component.ts`

#### Administration (3)
- `admin-users.component.ts`
- `admin-roles.component.ts`
- `admin-password-policy.component.ts`

---

### 3.4 Missing Components (Not Created)
1. Contacts & Groups management (list, create, edit)
2. Biblioteka upload modal/page
3. Global search component
4. Notification center
5. User profile/settings page
6. Help/documentation viewer
7. System settings page

---

## 4. Technology Stack Gaps

### 4.1 Required but Not Integrated

#### PrimeNG Components
**Requirement**: Use PrimeNG as primary UI library
**Status**: ❌ **NOT USED**
- PrimeNG installed in package.json but not imported
- No PrimeNG components in any template
- Missing components that should use PrimeNG:
  - p-table (instead of HTML tables)
  - p-button
  - p-dialog
  - p-dropdown
  - p-calendar
  - p-fileUpload
  - p-editor (rich text)
  - p-toast
  - p-confirmDialog
  - p-tree (for threading)
  - p-card
  - p-tag
  - p-badge
  - p-toolbar
  - p-splitButton
  - p-multiSelect
  - p-chip (for filter tags)

**Impact**: High - components built with raw HTML/CSS lack polish, accessibility, and consistency

---

#### State Management
**Requirement**: NgRx Signal Store or similar
**Status**: ❌ **NOT IMPLEMENTED**
- No global state management
- Component-level state only
- No shared state between components
- No caching of API responses

**Impact**: Medium - harder to share data, no offline support, repeated API calls

---

#### Rich Text Editor
**Requirement**: PrimeNG Editor or Quill
**Status**: ❌ **NOT IMPLEMENTED**
- Messages use plain textarea
- No formatting toolbar
- No HTML content support

**Impact**: High - messages cannot have formatted content (bold, lists, links, etc.)

---

#### File Upload
**Requirement**: Chunked upload for large files (50MB+)
**Status**: ❌ **NOT IMPLEMENTED**
- No upload component
- No drag & drop
- No chunking logic
- No progress tracking

**Impact**: High - cannot upload files as required

---

#### Form Validation
**Requirement**: Reactive forms with validation
**Status**: ❌ **MINIMAL**
- FormsModule used (template-driven) instead of ReactiveFormsModule
- No validation framework
- No error display patterns
- No custom validators

**Impact**: Medium - poor user experience, data quality issues

---

#### HTTP Interceptors
**Requirement**: Auth token injection, error handling, loading indicators
**Status**: ❌ **NOT CONFIGURED**
- No interceptors defined
- Manual error handling in each service
- No global loading state

**Impact**: Medium - code duplication, inconsistent error handling

---

#### Route Guards
**Requirement**: Protect routes based on authentication and roles
**Status**: ❌ **NOT IMPLEMENTED**
- No guards defined
- All routes publicly accessible
- No role-based access control

**Impact**: Critical - security issue, no authorization

---

### 4.2 Integrated but Underutilized

#### Tailwind CSS
**Status**: Installed but barely used
- Most components use custom CSS
- Tailwind utility classes rarely seen
- Could speed up styling significantly

---

#### Angular Signals
**Status**: Available (Angular 20) but not used
- Components use traditional RxJS patterns
- Could simplify state management
- Would improve performance

---

## 5. Priority Recommendations

### 5.1 Critical (Must Have for Demo)

#### P1.1 Authentication Flow
**Why**: Cannot demo without login capability
**Effort**: 2-3 days
**Components**:
- Implement login.component (form, validation, API call)
- Implement auth service (token storage, refresh)
- Add HTTP interceptor (token injection)
- Add route guards (auth check)
- Implement logout

---

#### P1.2 PrimeNG Integration
**Why**: UI looks unprofessional without component library
**Effort**: 3-5 days (refactor existing components)
**Components to Refactor**:
1. All tables → p-table
2. All buttons → p-button
3. All forms → p-dropdown, p-calendar, p-inputText
4. Add p-toast for notifications
5. Add p-confirmDialog for delete confirmations

---

#### P1.3 File Upload
**Why**: Core requirement, high visibility feature
**Effort**: 2-3 days
**Deliverables**:
- Create biblioteka-upload.component
- Implement drag & drop with p-fileUpload
- Add chunked upload logic
- Add progress tracking
- Add metadata form
- Integrate with backend API

---

#### P1.4 Message Compose & Reply
**Why**: Communication module is main feature
**Effort**: 2-3 days
**Deliverables**:
- Implement wiadomosci-compose.component
- Integrate p-editor (rich text)
- Add attachment upload
- Add recipient selector (p-multiSelect)
- Wire up send functionality
- Add reply/forward in wiadomosci-details

---

### 5.2 High Priority (Important for Demo Quality)

#### P2.1 Case Management
**Why**: Part of core communication workflow
**Effort**: 3-4 days
**Deliverables**:
- Implement sprawy-list (table with filters)
- Implement sprawy-create (form)
- Implement sprawy-details (case view with linked messages/docs)
- Wire up API integration

---

#### P2.2 Announcements
**Why**: Required feature, relatively simple
**Effort**: 2-3 days
**Deliverables**:
- Implement komunikaty-list (table)
- Implement komunikaty-create (form with target groups)
- Implement komunikaty-details (view with read confirmations)
- Wire up API integration

---

#### P2.3 Dashboard Widgets
**Why**: First screen users see
**Effort**: 1-2 days
**Deliverables**:
- Replace table view with widget layout
- Add statistics cards (messages, cases, documents count)
- Add quick action buttons
- Add recent activity feed
- Fetch real data from API

---

#### P2.4 Entity Registry
**Why**: Needed for linking to messages/cases
**Effort**: 2-3 days
**Deliverables**:
- Implement kartoteka-list (table)
- Implement kartoteka-details (entity view)
- Implement kartoteka-update (edit form)
- Wire up API integration

---

### 5.3 Medium Priority (Nice to Have)

#### P3.1 FAQ Module
**Effort**: 2 days
**Deliverables**:
- Implement faq-list
- Implement faq-create
- Implement faq-details

---

#### P3.2 Reports Module
**Effort**: 2-3 days
**Deliverables**:
- Implement sprawozdania-list
- Implement sprawozdania-details
- Add report submission form

---

#### P3.3 Admin Module
**Effort**: 3-4 days
**Deliverables**:
- Implement admin-users (table with CRUD)
- Implement admin-roles (role editor)
- Implement admin-password-policy (settings form)

---

#### P3.4 Enhanced Filtering
**Effort**: 2-3 days
**Deliverables**:
- Add filter sidebar for messages (as per Screen 06)
- Add filter chips/tags
- Add saved filters
- Add quick filter presets

---

#### P3.5 Export Functionality
**Effort**: 1-2 days
**Deliverables**:
- Add Excel export (using xlsx library)
- Add PDF export (using jsPDF or print stylesheets)
- Add print button to tables

---

### 5.4 Low Priority (Can Defer)

#### P4.1 Contacts & Groups
**Effort**: 2-3 days
**Reason**: Can use entity registry as workaround for demo

---

#### P4.2 Global Search
**Effort**: 2-3 days
**Reason**: Module-level search sufficient for demo

---

#### P4.3 Notifications Center
**Effort**: 1-2 days
**Reason**: Toast messages sufficient for demo

---

#### P4.4 User Profile/Settings
**Effort**: 1 day
**Reason**: Admin can edit users for demo

---

## 6. Estimated Effort Summary

| Priority | Total Effort | Components |
|----------|-------------|------------|
| **Critical (P1)** | 9-14 days | Auth, PrimeNG refactor, File upload, Message compose |
| **High (P2)** | 8-12 days | Cases, Announcements, Dashboard, Entity registry |
| **Medium (P3)** | 9-14 days | FAQ, Reports, Admin, Filters, Export |
| **Low (P4)** | 6-9 days | Contacts, Global search, Notifications, Profile |
| **TOTAL** | **32-49 days** | **Full implementation** |

**Note**: Times assume 1 developer. With parallel work (2-3 developers), critical path could be reduced to 2-3 weeks.

---

## 7. Quick Wins (Maximize Impact/Effort Ratio)

### Quick Win #1: Dashboard Statistics Cards
**Effort**: 4 hours
**Impact**: High (first impression)
**Why**: Simple HTML/CSS, API already exists

---

### Quick Win #2: PrimeNG Buttons
**Effort**: 2 hours
**Impact**: Medium (immediate visual improvement)
**Why**: Search & replace, minimal logic change

---

### Quick Win #3: Toast Notifications
**Effort**: 3 hours
**Impact**: High (better UX across all features)
**Why**: Single service, use in all components

---

### Quick Win #4: Breadcrumbs
**Effort**: 2 hours
**Impact**: Medium (better navigation)
**Why**: Simple routing-based component

---

### Quick Win #5: Login Form
**Effort**: 4 hours
**Impact**: Critical (enables auth flow)
**Why**: Simple form, API already exists

---

## 8. Risks & Constraints

### Technical Debt
- **Heavy refactoring needed**: Most components use plain HTML/CSS instead of PrimeNG
- **No tests**: Adding tests retroactively is harder
- **Inconsistent patterns**: Some components use different approaches

### API Dependencies
- **Backend must be ready**: Frontend work blocked if APIs not available
- **Mock data everywhere**: Need to replace with real API calls
- **No error handling**: Need robust HTTP interceptor before going live

### Time Pressure
- **48-hour hackathon assumed**: Cannot complete all features
- **Must prioritize**: Focus on 3-4 main workflows for demo
- **Polish vs. functionality**: Need balance between working features and visual quality

### Accessibility Compliance
- **WCAG 2.2 partially met**: Need systematic audit
- **Keyboard navigation incomplete**: Manual testing required
- **Screen reader testing**: Not done yet

---

## 9. Recommended Demo Scope

Given time constraints, focus on these user journeys for demo:

### Journey 1: Login & Dashboard (5 min)
1. User logs in
2. Sees dashboard with statistics
3. Clicks quick action to view messages

### Journey 2: View & Reply to Message (5 min)
1. Browse message list (inbox view)
2. Filter messages (unread only)
3. Open message details
4. View attachments
5. Compose reply with rich text
6. Send reply

### Journey 3: Upload File to Library (3 min)
1. Navigate to file library
2. Click "Add file"
3. Drag & drop file
4. Fill metadata form
5. Upload (show progress)
6. Verify file appears in list

### Journey 4: Create Announcement (3 min)
1. Navigate to announcements
2. Click "Create announcement"
3. Fill form (title, content, target groups)
4. Publish
5. Verify announcement visible

### Journey 5: Admin - Manage User (2 min)
1. Navigate to admin users
2. View user list
3. Edit user roles
4. Save changes

**Total Demo Time**: ~20 minutes
**Components Needed**: ~15 (focused implementation)

---

## 10. Conclusion

### Current State
- **Architecture**: ✅ Solid foundation (routing, services, layout)
- **Implementation**: ❌ Only 11% complete (4 of 35 components)
- **UI Quality**: ❌ Below expectations (no PrimeNG, inconsistent styling)
- **API Integration**: ❌ Minimal (mostly mock data)

### Recommended Action Plan

**Phase 1 - Critical Foundation (Week 1)**
1. Implement authentication flow
2. Add HTTP interceptors
3. Integrate PrimeNG in core components
4. Add toast notifications

**Phase 2 - Core Features (Week 2)**
1. Complete message compose/reply
2. Implement file upload
3. Complete dashboard widgets
4. Implement announcements

**Phase 3 - Supporting Features (Week 3)**
1. Implement case management
2. Implement entity registry
3. Add admin user management
4. Polish and testing

**Phase 4 - Demo Prep (Days before demo)**
1. End-to-end testing
2. Mock data cleanup
3. Visual polish
4. Presentation deck

### Success Criteria
- ✅ All 5 demo journeys work end-to-end
- ✅ Visual quality matches KNF branding
- ✅ No console errors during demo
- ✅ Accessible (keyboard navigation works)
- ✅ Responsive (works on tablet/desktop)

---

**Document Version**: 1.0
**Last Updated**: 2025-10-05
**Next Review**: After Sprint 2 completion
