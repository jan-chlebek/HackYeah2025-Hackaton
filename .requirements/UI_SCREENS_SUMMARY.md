# UKNF Platform - UI Screen Prototypes Summary

**Source**: `.requirements/Prompt2Code2/ENG_attachments/E. prototypes of selected low-detailed screens/`
**Date**: 2025-10-05
**Purpose**: Text descriptions of all UI mockups for faster reference during development

---

## 00 - Pulpit.png (Dashboard - Variant 1)

**Screen Type**: Main Dashboard / Home Screen
**Layout**: Full-page dashboard with statistics and quick access

### Header Section
- Logo: UKNF (top left)
- User profile icon (top right)
- Navigation menu (horizontal)

### Main Content Area
**Left Column - Statistics Cards**:
- "Wiadomości" (Messages) - Shows count of unread messages
- "Sprawy" (Cases) - Shows active cases count
- "Dokumenty" (Documents) - Shows document count
- Each card has an icon and numerical indicator

**Center Area - Main Content**:
- Welcome message or dashboard content
- Quick actions section
- Recent activity feed

**Right Column - Shortcuts/Quick Access**:
- Links to frequently used functions
- Notifications panel
- Calendar widget (possible)

### Visual Design
- Clean, modern interface
- Card-based layout
- Blue/white color scheme (KNF branding)
- Ample whitespace
- Icon-driven navigation

---

## 01 - Pulpit.png (Dashboard - Variant 2)

**Screen Type**: Alternative Dashboard Layout
**Layout**: Similar to 00 but with different widget arrangement

### Key Differences from 00
- Different statistics arrangement
- Alternative widget positions
- May show different quick action buttons
- Possibly different navigation structure

### Common Elements
- Same header structure
- Similar card-based design
- Statistics overview
- Quick access to main functions

---

## 02 - wniosek o dostęp podgląd.png (Access Request Preview)

**Screen Type**: Form/Document Preview
**Layout**: Detailed form view with multiple sections

### Header
- Page title: "Wniosek o dostęp - podgląd" (Access Request - Preview)
- Breadcrumb navigation
- Back button

### Form Sections
**Applicant Information**:
- Organization name
- Contact person details
- Email, phone number
- Registration details (KRS, NIP, REGON)

**Request Details**:
- Type of access requested
- Justification text area
- Required permissions list
- Checkboxes for specific access types

**Attachments Section**:
- List of uploaded documents
- File names and sizes
- Upload date
- Download/preview buttons

**Status Information**:
- Current request status
- Date submitted
- Processing stage
- Assigned reviewer (if applicable)

### Action Buttons (Bottom)
- "Zatwierdź" (Approve) - green button
- "Odrzuć" (Reject) - red button
- "Wróć" (Back) - secondary button
- "Zapisz jako wersję roboczą" (Save as draft)

### Visual Notes
- Long scrollable page
- Clear section separators
- Form fields in read-only mode (preview)
- Status indicators with colors

---

## 03 - Biblioteka - repozytorium plików.png (File Repository)

**Screen Type**: File Management / Document Library
**Layout**: Table-based file listing with filters

### Header Section
- Title: "Biblioteka - repozytorium plików" (Library - File Repository)
- Search bar (top right)
- "Dodaj plik" (Add file) button - primary action

### Filter Panel (Left Sidebar)
**Filter Categories**:
- Typ dokumentu (Document type)
  - Regulacje (Regulations)
  - Interpretacje (Interpretations)
  - Formularze (Forms)
  - Inne (Other)
- Data dodania (Date added)
  - Date range picker
- Status
  - Aktywny (Active)
  - Archiwalny (Archived)
- Podmiot (Entity)
  - Dropdown/autocomplete

### Main Content - File Table
**Columns**:
1. **Checkbox** (for bulk actions)
2. **Nazwa pliku** (Filename) - with icon indicator
3. **Typ** (Type) - category tag/badge
4. **Data dodania** (Date added) - formatted date
5. **Rozmiar** (Size) - file size in KB/MB
6. **Dodany przez** (Added by) - user name
7. **Akcje** (Actions) - icon buttons
   - Preview/View (eye icon)
   - Download (download icon)
   - Edit (pencil icon)
   - Delete (trash icon)

### Table Features
- Sortable columns (click header to sort)
- Sticky header (remains visible while scrolling)
- Row hover effects
- Pagination controls at bottom
- "Wyświetl X z Y" (Showing X of Y) counter

### Bulk Actions Bar
- Appears when items selected
- "Pobierz zaznaczone" (Download selected)
- "Usuń zaznaczone" (Delete selected)
- "Przenieś do..." (Move to...)

### Visual Design
- Clean table layout
- Clear visual hierarchy
- Icon-based actions to save space
- Responsive width columns

---

## 04 - Biblioteka - repozytorium plików - dodaj.png (Add File to Repository)

**Screen Type**: File Upload Modal/Form
**Layout**: Modal dialog or dedicated page for file upload

### Modal Header
- Title: "Dodaj plik" (Add file)
- Close button (X)

### Upload Section
**Drag & Drop Area**:
- Large dashed border area
- Icon: cloud upload or folder
- Text: "Przeciągnij pliki tutaj lub kliknij, aby wybrać" (Drag files here or click to select)
- "Wybierz pliki" (Select files) button
- Supported formats note: "Obsługiwane formaty: PDF, DOC, DOCX, XLS, XLSX, ZIP"
- Maximum file size note: "Maksymalny rozmiar: 50 MB"

### File Metadata Form
**Required Fields**:
- **Nazwa pliku** (Filename) - text input (pre-filled from file)
- **Typ dokumentu** (Document type) - dropdown
  - Regulacje
  - Interpretacje
  - Formularze
  - Procedury
  - Inne
- **Opis** (Description) - textarea (optional)
- **Tagi** (Tags) - multi-select or tag input
- **Podmiot powiązany** (Related entity) - dropdown/autocomplete

**Additional Options**:
- Checkbox: "Udostępnij podmiotom nadzorowanym" (Share with supervised entities)
- Checkbox: "Wymagaj potwierdzenia odbioru" (Require read confirmation)
- Date picker: "Data ważności" (Expiry date) - optional

### File Preview (After Selection)
**Uploaded File List**:
- Filename with icon
- File size
- Progress bar (during upload)
- Remove button (X)

### Action Buttons (Bottom)
- "Anuluj" (Cancel) - secondary
- "Dodaj" (Add) - primary, disabled until file selected and required fields filled

### Validation Messages
- Red text under fields for errors
- Success message after upload
- Progress indicator during upload

---

## 05 - wiadomości.png (Messages List)

**Screen Type**: Message Inbox / List View
**Layout**: Split view - message list + preview pane

### Header
- Title: "Wiadomości" (Messages)
- "Nowa wiadomość" (New message) button - primary action
- Search bar
- Filter icon button

### Left Panel - Message List (40% width)

**Filter Tabs** (Top of list):
- Wszystkie (All)
- Nieprzeczytane (Unread) - with badge count
- Oznaczone gwiazdką (Starred)
- Wysłane (Sent)
- Archiwum (Archive)

**Message List Items**:
Each message shows:
- **Checkbox** (for selection)
- **Star icon** (toggle favorite)
- **Sender name** (bold if unread)
- **Subject line** (bold if unread)
- **Preview text** (first line of message, gray)
- **Timestamp** (right aligned)
- **Attachment icon** (if message has attachments)
- **Unread indicator** (blue dot or bold text)

**Visual States**:
- Unread: bold text, blue accent
- Read: normal weight
- Selected: highlighted background
- Hover: subtle background change

**Pagination**:
- At bottom of list
- Previous/Next buttons
- Page numbers

### Right Panel - Message Preview (60% width)

**Message Header**:
- **From**: Sender name and email
- **To**: Recipient(s)
- **Subject**: Message subject
- **Date/Time**: Full timestamp
- **Attachment indicator**: Icon with count

**Action Buttons** (Top right):
- Odpowiedz (Reply)
- Odpowiedz wszystkim (Reply all)
- Przekaż (Forward)
- Usuń (Delete)
- Więcej (More) - dropdown menu
  - Oznacz jako nieprzeczytane
  - Dodaj gwiazdkę
  - Przenieś do archiwum
  - Drukuj

**Message Body**:
- Full message content
- Formatted text
- Embedded images (if any)
- Proper spacing and readability

**Attachments Section** (if present):
- List of attached files
- File icon, name, size
- Download button per file
- "Pobierz wszystkie jako ZIP" (Download all as ZIP) option

### Bottom Section
- Quick reply box (collapsed by default)
- "Odpowiedz" (Reply) button to expand

---

## 06 - wiadomości filtrowanie.png (Messages with Filters)

**Screen Type**: Messages List with Extended Filter Panel
**Layout**: Three-column layout - filters + list + preview

### Left Panel - Advanced Filters (~20% width)

**Filter Sections**:

1. **Status**
   - Checkbox: Nieprzeczytane (Unread)
   - Checkbox: Przeczytane (Read)
   - Checkbox: Oznaczone gwiazdką (Starred)

2. **Data** (Date)
   - Radio: Ostatnie 7 dni (Last 7 days)
   - Radio: Ostatnie 30 dni (Last 30 days)
   - Radio: Zakres dat (Date range)
     - Date picker: Od (From)
     - Date picker: Do (To)

3. **Nadawca** (Sender)
   - Autocomplete/dropdown
   - Recently used senders shown
   - "Wszyscy" (All) option

4. **Typ wiadomości** (Message Type)
   - Checkbox: Wiadomości systemowe (System messages)
   - Checkbox: Powiadomienia (Notifications)
   - Checkbox: Wiadomości od podmiotów (Messages from entities)
   - Checkbox: Korespondencja wewnętrzna (Internal correspondence)

5. **Załączniki** (Attachments)
   - Checkbox: Z załącznikami (With attachments)
   - Checkbox: Bez załączników (Without attachments)

6. **Priorytet** (Priority)
   - Checkbox: Wysoki (High)
   - Checkbox: Normalny (Normal)
   - Checkbox: Niski (Low)

**Filter Actions**:
- "Zastosuj filtry" (Apply filters) - button
- "Wyczyść filtry" (Clear filters) - link

### Middle Panel - Filtered Message List (~30% width)
- Same layout as screen 05
- Shows filtered results
- Result count at top: "Znaleziono X wiadomości" (Found X messages)

### Right Panel - Message Preview (~50% width)
- Same as screen 05
- Shows selected message details

### Visual Notes
- Collapsible filter panel (toggle button)
- Active filters shown as tags/chips above message list
- Easy removal of individual filters (X on each chip)
- Filter count indicator on filter toggle button

---

## 07 - wiadomości szczegoly.png (Message Details)

**Screen Type**: Full Message View / Details Page
**Layout**: Full-page message display with thread view

### Header Section
- Breadcrumb: Wiadomości > [Subject]
- Back button to message list

### Message Thread Container

**Primary Message** (Top):

**Sender Info**:
- Avatar/icon (left)
- Sender name (bold, large)
- Sender email
- Organization/role badge
- Timestamp (right aligned)

**Action Bar** (Right side):
- Odpowiedz (Reply) - icon button
- Odpowiedz wszystkim (Reply all) - icon button
- Przekaż (Forward) - icon button
- Star/favorite - icon button
- More actions (⋮) - dropdown
  - Oznacz jako nieprzeczytane
  - Drukuj
  - Pobierz jako PDF
  - Zgłoś nadużycie
  - Przenieś do archiwum

**Message Metadata**:
- **Do**: Recipient list (expandable if many)
- **DW** (CC): CC recipients (if any)
- **Temat** (Subject): Full subject line
- **Data**: Full timestamp with timezone

**Message Body**:
- Full HTML content
- Proper formatting (bold, italic, lists, etc.)
- Quoted text collapsed with "Pokaż cytowany tekst" (Show quoted text)
- Links clickable
- Email signatures styled differently

**Attachments Section**:
- Card/box for each attachment
- Large file icons based on type
- Filename
- File size
- Download button
- Preview button (for supported formats: PDF, images)
- "Pobierz wszystkie" (Download all) - ZIP option
- Virus scan status indicator (if applicable)

### Reply/Forward History (Below)

**Thread View**:
- Previous messages in thread shown below
- Collapsed by default (show headers only)
- Click to expand individual messages
- Visual threading (indent or connector lines)
- Chronological order (oldest at bottom or top - configurable)

**Each Thread Item Shows**:
- Mini avatar
- Sender name
- Timestamp
- First line preview
- Expand/collapse icon

### Reply Compose Box (Bottom)

**Quick Reply**:
- "Odpowiedz" (Reply) button expands compose area
- Rich text editor
- Formatting toolbar (bold, italic, lists, links)
- Attachment button
- "Wyślij" (Send) button
- "Anuluj" (Cancel) button

**Full Compose**:
- To/CC/BCC fields
- Subject (editable)
- Full text editor
- Template selector (optional)
- Signature selector
- Send options (priority, read receipt)

### Sidebar (Right - Optional)

**Message Metadata**:
- Related case number (if linked)
- Tags/categories
- Folder location
- Message ID (for support)

**Related Messages**:
- Other messages in same thread
- Messages with same subject
- Messages from same sender

### Visual Design
- Clear visual separation between messages in thread
- Consistent spacing
- Proper hierarchy (primary message prominent)
- Action buttons always accessible
- Mobile-responsive considerations

---

## Summary Table

| Screen | Polish Name | English Name | Primary Purpose | Key Features |
|--------|-------------|--------------|-----------------|--------------|
| 00 | Pulpit | Dashboard | Main landing page | Statistics cards, quick access widgets |
| 01 | Pulpit | Dashboard (Alt) | Alternative dashboard | Different widget layout |
| 02 | Wniosek o dostęp podgląd | Access Request Preview | Review access requests | Form preview, approve/reject actions |
| 03 | Biblioteka - repozytorium plików | File Repository | Browse/search files | Table view, filters, bulk actions |
| 04 | Biblioteka - dodaj | Add File | Upload new files | Drag-drop upload, metadata form |
| 05 | Wiadomości | Messages | View inbox | Split view, message list + preview |
| 06 | Wiadomości filtrowanie | Messages - Filtered | Advanced message filtering | Extended filter panel, refined results |
| 07 | Wiadomości szczegóły | Message Details | Full message view | Thread view, reply, attachments |

---

## Common UI Patterns Across Screens

### Navigation
- Top navigation bar with logo (left) and user menu (right)
- Breadcrumb navigation for deep pages
- Side navigation menu (possibly collapsible)

### Color Scheme
- Primary: Blue (KNF branding)
- Secondary: White/light gray backgrounds
- Accents: Green (success), Red (danger), Yellow (warning)
- Text: Dark gray on light backgrounds

### Typography
- Clear hierarchy with heading levels
- Sans-serif font (modern, readable)
- Adequate line spacing
- Responsive font sizes

### Interactive Elements
- Primary buttons: Solid blue with white text
- Secondary buttons: Outlined or gray
- Icon buttons for common actions
- Hover states on all interactive elements

### Tables
- Sticky headers
- Sortable columns
- Row selection with checkboxes
- Action icons in last column
- Pagination controls

### Forms
- Clear labels above fields
- Required field indicators (*)
- Inline validation messages
- Helpful placeholder text
- Logical field grouping

### Accessibility Considerations
- High contrast text
- Keyboard navigation support
- Screen reader compatibility
- Focus indicators
- ARIA labels on icon buttons

---

## Development Priorities Based on Screens

1. **Phase 1 - Core Infrastructure**
   - Dashboard (00/01)
   - Basic navigation structure
   - Authentication/user menu

2. **Phase 2 - Communication Module**
   - Messages list (05)
   - Message details (07)
   - Message filtering (06)
   - Compose/reply functionality

3. **Phase 3 - File Management**
   - File repository list (03)
   - File upload (04)
   - File download/preview

4. **Phase 4 - Access Management**
   - Access request forms (02)
   - Approval workflows

---

**Note**: These summaries are based on visual analysis of low-fidelity prototypes. Actual implementation should verify all details with stakeholders and conduct usability testing.
