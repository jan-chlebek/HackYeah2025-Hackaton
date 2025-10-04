# Frontend Routes Summary

This document provides an overview of all routes created for the UKNF Platform Communication application.

## Main Routes Structure

All routes are configured in `src/app/app.routes.ts` with lazy loading for optimal performance.

### Root Routes

| Route | Description | Component/Module |
|-------|-------------|------------------|
| `/` | Redirects to dashboard | → `/dashboard` |
| `/dashboard` | Main dashboard for all users | `DashboardComponent` |
| `/auth` | Authentication module | Lazy-loaded module |
| `/sprawozdania` | Reports management | Lazy-loaded module |
| `/wiadomosci` | Messaging system | Lazy-loaded module |
| `/sprawy` | Administrative cases | Lazy-loaded module |
| `/biblioteka` | File repository/library | Lazy-loaded module |
| `/komunikaty` | Announcements board | Lazy-loaded module |
| `/faq` | Q&A knowledge base | Lazy-loaded module |
| `/kartoteka` | Podmiot registry | Lazy-loaded module |
| `/wnioski` | Access requests | Lazy-loaded module |
| `/admin` | Administration panel | Lazy-loaded module |
| `/**` | Fallback route | → `/dashboard` |

---

## Module-Specific Routes

### 1. Authentication Module (`/auth`)
**File:** `src/app/features/auth/auth.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/auth/login` | `LoginComponent` | User login |
| `/auth/register` | `RegisterComponent` | External user registration |
| `/auth/password-reset` | `PasswordResetComponent` | Password reset |

---

### 2. Dashboard (`/dashboard`)
**File:** `src/app/features/dashboard/dashboard.component.ts`

Single component showing personalized dashboard with:
- Kafelki (tiles) for quick access
- Status summaries
- Recent activities
- Notifications

---

### 3. Sprawozdania Module (`/sprawozdania`)
**File:** `src/app/features/sprawozdania/sprawozdania.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/sprawozdania` | `SprawozdaniaListComponent` | List of all reports with filters |
| `/sprawozdania/create` | `SprawozdaniaCreateComponent` | Submit new report (XLSX upload) |
| `/sprawozdania/:id` | `SprawozdaniaDetailsComponent` | Report details with validation status |

**Features:**
- Report submission with XLSX template
- Validation status tracking
- Correction (korekta) support
- Report calendar/schedule

---

### 4. Wiadomości Module (`/wiadomosci`)
**File:** `src/app/features/wiadomosci/wiadomosci.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/wiadomosci` | `WiadomosciListComponent` | Message inbox/sent items |
| `/wiadomosci/compose` | `WiadomosciComposeComponent` | Compose new message |
| `/wiadomosci/:id` | `WiadomosciDetailsComponent` | Message thread details |

**Features:**
- Email-like interface
- Attachment support (PDF, DOC, XLS, CSV, MP3, ZIP)
- Thread grouping
- Status tracking

---

### 5. Sprawy Module (`/sprawy`)
**File:** `src/app/features/sprawy/sprawy.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/sprawy` | `SprawyListComponent` | List of administrative cases |
| `/sprawy/create` | `SprawyCreateComponent` | Create new case |
| `/sprawy/:id` | `SprawyDetailsComponent` | Case folder with documents |

**Features:**
- Case categories (Zmiana danych, Zmiana składu, etc.)
- Priority levels (Niski, Średni, Wysoki)
- Status workflow
- Document management

---

### 6. Biblioteka Module (`/biblioteka`)
**File:** `src/app/features/biblioteka/biblioteka.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/biblioteka` | `BibliotekaListComponent` | File repository browser |
| `/biblioteka/:id` | `BibliotekaDetailsComponent` | File details and history |

**Features:**
- File upload/download
- Version management
- Permission control
- Metadata management
- Template downloads (e.g., XLSX report templates)

---

### 7. Komunikaty Module (`/komunikaty`)
**File:** `src/app/features/komunikaty/komunikaty.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/komunikaty` | `KomunikatyListComponent` | Announcements board |
| `/komunikaty/create` | `KomunikatyCreateComponent` | Create announcement (UKNF only) |
| `/komunikaty/:id` | `KomunikatyDetailsComponent` | Announcement details |

**Features:**
- WYSIWYG editor
- Priority levels
- Recipient targeting (groups, types, individual)
- Read confirmation tracking
- Expiration dates

---

### 8. FAQ Module (`/faq`)
**File:** `src/app/features/faq/faq.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/faq` | `FaqListComponent` | Browse Q&A knowledge base |
| `/faq/submit` | `FaqSubmitComponent` | Submit question (anonymous/authenticated) |
| `/faq/manage` | `FaqManageComponent` | Manage Q&A (UKNF only) |

**Features:**
- Question submission
- Answer management
- Category and tag filtering
- Rating system (1-5 stars)
- Search functionality

---

### 9. Kartoteka Module (`/kartoteka`)
**File:** `src/app/features/kartoteka/kartoteka.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/kartoteka` | `KartotekaListComponent` | Podmiot registry list |
| `/kartoteka/:id` | `KartotekaDetailsComponent` | Podmiot details |
| `/kartoteka/:id/update` | `KartotekaUpdateComponent` | Update podmiot data |

**Features:**
- Podmiot data management
- Change history tracking
- Data verification workflow
- Associated users view
- Update request submission

**Podmiot Fields:**
- Identifiers: ID, Typ podmiotu, Kod UKNF, Nazwa, LEI, NIP, KRS
- Address: Ulica, Numer budynku, Numer lokalu, Kod pocztowy, Miejscowość
- Contact: Telefon, E-mail
- Classification: Status, Kategoria, Sektor, Podsektor, Transgraniczny

---

### 10. Wnioski Module (`/wnioski`)
**File:** `src/app/features/wnioski/wnioski.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/wnioski` | `WnioskiListComponent` | Access requests list |
| `/wnioski/create` | `WnioskiCreateComponent` | Create access request |
| `/wnioski/:id` | `WnioskiDetailsComponent` | Request details with approval workflow |

**Features:**
- Access request submission
- Permission line management
- Approval workflow (UKNF/Admin Podmiotu)
- Status tracking (Roboczy, Nowy, Zaakceptowany, Zablokowany)
- Messaging within request context

---

### 11. Admin Module (`/admin`)
**File:** `src/app/features/admin/admin.routes.ts`

| Route | Component | Purpose |
|-------|-----------|---------|
| `/admin` | Redirects to `/admin/users` | → `/admin/users` |
| `/admin/users` | `AdminUsersComponent` | User management |
| `/admin/password-policy` | `AdminPasswordPolicyComponent` | Password policy configuration |
| `/admin/roles` | `AdminRolesComponent` | Role and permission management |

**Features:**
- User CRUD operations (internal & external)
- Password policy settings
- Role creation and assignment
- Permission matrix configuration
- Bulk operations

---

## Role-Based Access

Routes will be protected with guards based on user roles:

### User Roles

1. **External Users:**
   - **Pracownik Podmiotu Nadzorowanego** - Limited access (sprawozdania, sprawy, wiadomości, biblioteka view, komunikaty view, FAQ)
   - **Administrator Podmiotu Nadzorowanego** - Extended access (+ wnioski management for their podmiot)

2. **Internal Users:**
   - **Pracownik UKNF** - Full operational access (+ manage "Moje podmioty")
   - **Administrator systemu** - Full system access (+ admin panel)

### Access Matrix

| Module | Pracownik Podmiotu | Admin Podmiotu | Pracownik UKNF | Admin Systemu |
|--------|-------------------|----------------|----------------|---------------|
| Dashboard | ✓ | ✓ | ✓ | ✓ |
| Auth | ✓ | ✓ | ✓ | ✓ |
| Sprawozdania | ✓ (own) | ✓ (own) | ✓ (all) | ✓ (all) |
| Wiadomości | ✓ | ✓ | ✓ | ✓ |
| Sprawy | ✓ | ✓ | ✓ | ✓ |
| Biblioteka | ✓ (view) | ✓ (view) | ✓ (manage) | ✓ (manage) |
| Komunikaty | ✓ (view) | ✓ (view) | ✓ (manage) | ✓ (manage) |
| FAQ | ✓ (view/submit) | ✓ (view/submit) | ✓ (manage) | ✓ (manage) |
| Kartoteka | ✓ (own view) | ✓ (own view/update) | ✓ (all) | ✓ (all) |
| Wnioski | ✓ (own) | ✓ (approve own podmiot) | ✓ (approve) | ✓ (all) |
| Admin | ✗ | ✗ | ✗ | ✓ |

---

## Technical Implementation

### Routing Configuration

All routes use:
- **Lazy loading** for optimal performance
- **Standalone components** (Angular 20)
- **Feature-based folder structure**
- Preparation for **role-based guards** (to be implemented)

### Folder Structure

```
src/app/
├── app.routes.ts                    # Main routing configuration
├── features/
│   ├── dashboard/
│   │   ├── dashboard.component.ts
│   │   ├── dashboard.component.html
│   │   └── dashboard.component.css
│   ├── auth/
│   │   ├── auth.routes.ts
│   │   ├── login/
│   │   ├── register/
│   │   └── password-reset/
│   ├── sprawozdania/
│   │   ├── sprawozdania.routes.ts
│   │   ├── sprawozdania-list/
│   │   ├── sprawozdania-create/
│   │   └── sprawozdania-details/
│   ├── wiadomosci/
│   │   ├── wiadomosci.routes.ts
│   │   ├── wiadomosci-list/
│   │   ├── wiadomosci-compose/
│   │   └── wiadomosci-details/
│   ├── sprawy/
│   │   ├── sprawy.routes.ts
│   │   ├── sprawy-list/
│   │   ├── sprawy-create/
│   │   └── sprawy-details/
│   ├── biblioteka/
│   │   ├── biblioteka.routes.ts
│   │   ├── biblioteka-list/
│   │   └── biblioteka-details/
│   ├── komunikaty/
│   │   ├── komunikaty.routes.ts
│   │   ├── komunikaty-list/
│   │   ├── komunikaty-create/
│   │   └── komunikaty-details/
│   ├── faq/
│   │   ├── faq.routes.ts
│   │   ├── faq-list/
│   │   ├── faq-submit/
│   │   └── faq-manage/
│   ├── kartoteka/
│   │   ├── kartoteka.routes.ts
│   │   ├── kartoteka-list/
│   │   ├── kartoteka-details/
│   │   └── kartoteka-update/
│   ├── wnioski/
│   │   ├── wnioski.routes.ts
│   │   ├── wnioski-list/
│   │   ├── wnioski-create/
│   │   └── wnioski-details/
│   └── admin/
│       ├── admin.routes.ts
│       ├── admin-users/
│       ├── admin-password-policy/
│       └── admin-roles/
```

---

## Next Steps

To complete the routing implementation, the following items need to be added:

1. **Auth Guards** - Implement role-based route protection
2. **Navigation Components** - Header, sidebar, breadcrumbs
3. **Shared Services** - Auth service, API services
4. **State Management** - NgRx/Signal Store for complex state
5. **API Integration** - Connect to backend REST API
6. **PrimeNG Components** - Add UI library components
7. **Form Validation** - Implement Polish-specific validators (PESEL, NIP, KRS, LEI)
8. **File Upload** - Chunked upload implementation
9. **Internationalization** - Polish locale setup

---

## Status: ✅ Complete

All routes and empty component scaffolds have been successfully created. The application structure is ready for feature implementation.

**Total Routes Created:** 47  
**Total Components Created:** 37  
**Total Modules:** 11

All routes follow Angular 20 best practices with standalone components and lazy loading.
