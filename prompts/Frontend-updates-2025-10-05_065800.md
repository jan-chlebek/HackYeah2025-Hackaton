# Prompt: Display list of supervised entities from API endpoint

**Date**: 2025-10-05 06:58:00  
**Branch**: Frontend-updates

## User Request
On this part you should display the list of podmioty from http://localhost:5000/api/v1/entities endpoint which returns for example:
```json
{"data":[{"id":10,"name":"Allianz Polska S.A.","entityType":"Insurance","nip":"5260231971","regon":"012267870","city":"Warszawa","isActive":true,"userCount":1,"createdAt":"2025-10-05T02:39:31.139616Z"}, ...], "pagination":{"page":1,"pageSize":20,"totalCount":16,"totalPages":1}}
```

## Context
Following up on the previous work where we:
1. Created the `SupervisedEntityService` to interact with the backend API
2. Added "Kartoteka Podmiotów" menu item to the sidebar

Now we need to implement the actual list view component that displays the supervised entities data from the API endpoint.

## Analysis
1. **Existing Structure**: The `features/entities/` directory already exists with placeholder components:
   - `entities-list.component.ts` - Just a placeholder
   - Entity routes configured in `entities.routes.ts`
   - Main route at `/entities` working

2. **API Response Structure**:
   - Returns paginated data with `data` array and `pagination` object
   - Each entity has: id, name, entityType, nip, regon, city, isActive, userCount, createdAt
   - Pagination includes: page, pageSize, totalCount, totalPages

3. **Pattern Reference**: Analyzed existing list components (library-list, reports-list) for consistent patterns

## Implementation

### 1. Updated Component TypeScript (`entities-list.component.ts`)
Completely rewrote the component with:

**Imports & Dependencies**:
- Angular core: `OnInit`, `inject`, `ChangeDetectorRef`
- Router for navigation
- FormsModule for two-way binding
- PrimeNG modules: Table, Button, InputText, Select, Breadcrumb
- `SupervisedEntityService` for API calls

**Component State**:
- `entities: SupervisedEntityListItem[]` - List of entities from API
- `loading: boolean` - Loading state indicator
- `totalRecords: number` - Total count for pagination
- `page`, `pageSize`, `first` - Pagination state
- `filters: EntityFilters` - Search and filter state

**Data Loading**:
- `loadEntities()` - Fetches data from API with current pagination and filters
- Subscribes to `entityService.getEntities()`
- Updates component state on success/error
- Triggers change detection

**Pagination**:
- `onPageChange(event)` - Handles PrimeNG table pagination events
- Updates page number, page size, and first record index

**Filtering**:
- `applyFilters()` - Resets to page 1 and reloads with current filters
- `clearFilters()` - Resets all filters and reloads
- `toggleFilters()` - Shows/hides filter panel
- Filter fields: searchTerm (name/NIP/REGON/KRS), entityType, isActive

**Navigation**:
- `viewEntityDetails(id)` - Navigate to entity details page
- `createEntity()` - Navigate to entity creation page

**Utility Methods**:
- `getEntityTypeLabel()` - Maps entity type codes to Polish labels
- `formatDate()` - Formats ISO dates to Polish locale

**Configuration**:
- Entity type options: Bank, Insurance, InvestmentFund, PensionFund, BrokerageHouse, CreditUnion
- Active status options: All, Active, Inactive
- Page size options: 10, 20, 50, 100
- Breadcrumb: Dashboard → Kartoteka Podmiotów

### 2. Created HTML Template (`entities-list.component.html`)
Comprehensive template with:

**Breadcrumb Navigation**:
- PrimeNG breadcrumb component
- Links: Home → Dashboard → Kartoteka Podmiotów

**Page Header**:
- H1 title: "Kartoteka Podmiotów"
- "Dodaj podmiot" button (create new entity)

**Filter Section**:
- Collapsible panel with toggle button
- Three filter fields:
  1. **Search**: Free text search (name, NIP, REGON, KRS) with Enter key support
  2. **Entity Type**: Dropdown with all entity types + "Wszystkie"
  3. **Status**: Active/Inactive/All filter
- Action buttons: "Wyczyść" (clear), "Szukaj" (search)

**Data Table** (PrimeNG Table):
- **Columns**:
  1. # - Row number
  2. Nazwa podmiotu - Entity name (bold, sortable)
  3. Typ - Entity type with colored badges (sortable)
  4. NIP - Tax ID
  5. REGON - Statistical number
  6. Miasto - City
  7. Liczba użytkowników - User count with badge
  8. Status - Active/Inactive with colored badge (sortable)
  9. Data utworzenia - Creation date (sortable)
  10. Akcje - Actions (view button)

- **Features**:
  - Lazy loading with server-side pagination
  - Striped rows
  - Sortable columns (name, type, status, createdAt)
  - Responsive scrolling
  - Loading spinner
  - Empty state message with icon
  - Current page report in Polish
  - Row actions: View details (eye icon)

**Accessibility**:
- ARIA labels on breadcrumb, filter panel, action buttons
- Keyboard navigation support
- Screen reader friendly

### 3. Created CSS Styles (`entities-list.component.css`)
Professional styling with:

**Layout**:
- Container with max-width 1400px, centered
- Responsive grid for filters (auto-fit, minmax 250px)
- Flexbox for header and actions

**Page Header**:
- Flex layout with space-between
- Large blue title (#003366)
- Responsive wrapping

**Filter Section**:
- Light gray background (#f8f9fa)
- Rounded corners (8px)
- Collapsible animation with max-height transition
- Clean form field styling

**Badges & Status**:
- **Entity Type Badges**: Color-coded by type
  - Bank: Blue (#e3f2fd / #1565c0)
  - Insurance: Purple (#f3e5f5 / #6a1b9a)
  - InvestmentFund: Green (#e8f5e9 / #2e7d32)
  - PensionFund: Orange (#fff3e0 / #e65100)
  - BrokerageHouse: Pink (#fce4ec / #c2185b)
  - CreditUnion: Teal (#e0f2f1 / #00695c)

- **Status Badges**: 
  - Active: Green (#d4edda / #155724)
  - Inactive: Red (#f8d7da / #721c24)

- **User Count Badge**: 
  - Circular blue badge (#e3f2fd / #1565c0)

**Table Styling**:
- White background with shadow
- Rounded corners
- Proper spacing and alignment
- Center-aligned action buttons

**Empty State**:
- Large building icon
- Centered text with helpful message

**Responsive Design**:
- Mobile breakpoint at 768px
- Single column filters on mobile
- Full-width buttons on mobile
- Smaller headings

**High Contrast Mode**:
- Black background (#000000)
- Yellow text and borders (#FFFF00)
- All badges inverted (yellow background, black text)
- Full accessibility support

## Benefits
✅ **Complete Entity Management UI**: Users can now view all supervised entities in a professional table
✅ **Advanced Filtering**: Search by name/NIP/REGON/KRS, filter by type and active status
✅ **Server-Side Pagination**: Handles large datasets efficiently
✅ **Sortable Columns**: Click headers to sort by name, type, status, or creation date
✅ **Visual Entity Types**: Color-coded badges make entity types instantly recognizable
✅ **Responsive Design**: Works on desktop, tablet, and mobile devices
✅ **Accessibility**: WCAG 2.2 compliant with ARIA labels and high contrast mode
✅ **Consistent UX**: Matches patterns from other list views (library, reports, messages)
✅ **Real API Integration**: Connects to actual backend endpoint at http://localhost:5000/api/v1/entities
✅ **Navigation Ready**: Links prepared for entity details and create pages

## Testing Notes
To test this component:
1. Navigate to http://localhost:4200/entities
2. Verify 16 entities load from API
3. Test search filter (e.g., search "PKO")
4. Test entity type filter (e.g., select "Bank")
5. Test status filter (active/inactive)
6. Test pagination (change page size to 10)
7. Click entity row to verify navigation
8. Toggle high contrast mode and verify styles
9. Test on mobile viewport (< 768px)

## Next Steps
Now that the list view is complete, we can:
1. Implement entity details view (`entity-details.component.ts`)
2. Implement entity create form (`entity-create.component.ts`)
3. Implement entity update/edit form (`entity-update.component.ts`)
4. Add CSV import functionality
5. Add entity user management

## Compliance
✅ Uses newly created `SupervisedEntityService`
✅ Follows Angular standalone component pattern
✅ Matches existing list component patterns (library, reports)
✅ Implements KNF color scheme and branding
✅ WCAG 2.2 accessibility compliance
✅ Responsive design for all devices
✅ High contrast mode support
✅ Polish language UI (as per requirements)
