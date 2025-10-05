# Permission-Based UI Visibility

## Overview

The application now implements role-based UI visibility. Certain features are only accessible to users with elevated permissions (Supervisors and Administrators), while basic features are available to all authenticated users including External Users (Internal Users).

## Permission Levels

### External User / Internal User (4 permissions)
- **Basic access** to core features
- **Can view:** Messages, Announcements, Library, Reports, FAQs
- **Cannot access:** Kartoteka Podmiotów, Sprawy, Adresaci, Wnioski o dostęp

### Supervisor (7 permissions)
- **Elevated access** to management features
- **Can view:** All features including restricted sections
- **Can manage:** FAQs, Library, Entities

### Administrator (9 permissions)
- **Full access** to all features
- **Can manage:** Users, all entities, all features

## Restricted Features

The following features are **hidden** for users without elevated permissions:

### 1. Sidebar Navigation
- **Wnioski o dostęp** (`/wnioski`)
- **Kartoteka Podmiotów** (`/entities`)
- **Sprawy** (`/cases`)
- **Adresaci** (`/contacts`)

### 2. Dashboard Tabs
- **Wnioski o dostęp** tab

### 3. Breadcrumb Navigation
Restricted breadcrumb links are automatically filtered based on permissions in:
- Messages list page
- Library list page
- Contact groups page

## Implementation

### Sidebar Component

The sidebar dynamically filters menu items based on user permissions:

```typescript
// Menu items with permission requirements
private allMenuItems: MenuItem[] = [
  { label: 'Wiadomości', icon: 'pi pi-envelope', route: '/messages' },
  { label: 'Komunikaty', icon: 'pi pi-megaphone', route: '/announcements' },
  { label: 'Biblioteka', icon: 'pi pi-folder-open', route: '/library' },
  { label: 'Wnioski o dostęp', icon: 'pi pi-file', route: '/wnioski', 
    requiresElevatedPermissions: true },
  { label: 'Sprawozdawczość', icon: 'pi pi-chart-line', route: '/reports' },
  { label: 'Moje pytania', icon: 'pi pi-question-circle', route: '/faq' },
  { label: 'Kartoteka Podmiotów', icon: 'pi pi-building', route: '/entities', 
    requiresElevatedPermissions: true },
  { label: 'Sprawy', icon: 'pi pi-clipboard', route: '/cases', 
    requiresElevatedPermissions: true },
  { label: 'Adresaci', icon: 'pi pi-users', route: '/contacts', 
    requiresElevatedPermissions: true }
];

// Computed property filters based on permissions
visibleMenuItems = computed(() => {
  const hasElevatedPermissions = this.authService.hasElevatedPermissions();
  return this.allMenuItems.filter(item => 
    !item.requiresElevatedPermissions || hasElevatedPermissions
  );
});
```

### Dashboard Component

Tabs are filtered based on permissions:

```typescript
private allTabs = [
  { label: 'Pulpit użytkownika', requiresElevatedPermissions: false },
  { label: 'Wnioski o dostęp', requiresElevatedPermissions: true },
  { label: 'Biblioteka - repozytorium plików', requiresElevatedPermissions: false }
];

tabs = computed(() => {
  const hasElevatedPermissions = this.authService.hasElevatedPermissions();
  return this.allTabs.filter(tab => 
    !tab.requiresElevatedPermissions || hasElevatedPermissions
  );
});
```

### Breadcrumb Components

Breadcrumbs are built dynamically in `ngOnInit`:

```typescript
ngOnInit(): void {
  // Build breadcrumb based on permissions
  const items: MenuItem[] = [];
  
  if (this.authService.hasElevatedPermissions()) {
    items.push({ label: 'Wnioski o dostęp', routerLink: '/wnioski' });
  }
  
  items.push({ label: 'Current Page' });
  this.breadcrumbItems = items;
  
  // ... rest of initialization
}
```

## Testing

### Test as External User (Internal User)
Login with:
- **Email:** `jan.kowalski@uknf.gov.pl`
- **Password:** `User123!`

**Expected behavior:**
- Sidebar shows only: Wiadomości, Komunikaty, Biblioteka, Sprawozdawczość, Moje pytania
- Dashboard shows only 2 tabs (no "Wnioski o dostęp")
- Breadcrumbs don't show restricted links

### Test as Supervisor
Login with:
- **Email:** `anna.nowak@uknf.gov.pl`
- **Password:** `Supervisor123!`

**Expected behavior:**
- Sidebar shows all menu items
- Dashboard shows all 3 tabs
- Breadcrumbs show all links including restricted ones

### Test as Administrator
Login with:
- **Email:** `admin@uknf.gov.pl`
- **Password:** `Admin123!`

**Expected behavior:**
- Sidebar shows all menu items
- Dashboard shows all 3 tabs
- Full access to all features

## Auth Service Methods

### `hasElevatedPermissions(): boolean`
Returns `true` if user is Supervisor or Administrator (more than 4 permissions).

```typescript
const hasAccess = this.authService.hasElevatedPermissions();
```

### `hasPermission(permission: string): boolean`
Check for a specific permission.

```typescript
const canManageUsers = this.authService.hasPermission('ManageUsers');
```

### `hasRole(role: string): boolean`
Check for a specific role.

```typescript
const isAdmin = this.authService.hasRole('Administrator');
```

## Adding New Restricted Features

To restrict a new feature:

### 1. In Sidebar
Add `requiresElevatedPermissions: true` to the menu item:

```typescript
{ 
  label: 'New Feature', 
  icon: 'pi pi-new', 
  route: '/new-feature',
  requiresElevatedPermissions: true 
}
```

### 2. In Templates
Use `@if` directive with permission check:

```html
@if (authService.hasElevatedPermissions()) {
  <button>Restricted Action</button>
}
```

### 3. In Components
Use computed signals:

```typescript
showAdvancedFeatures = computed(() => 
  this.authService.hasElevatedPermissions()
);
```

## Security Notes

⚠️ **Important:** UI visibility is **not** a security measure. Always enforce permissions on the backend API. These UI restrictions only improve user experience by hiding irrelevant features.

The backend API validates permissions on every endpoint, so even if a user manually navigates to a restricted route, they will receive a 403 Forbidden response.

## Files Modified

### Components
- `src/app/shared/layout/sidebar/sidebar.component.ts`
- `src/app/features/dashboard/dashboard.component.ts`
- `src/app/features/messages/messages-list/messages-list.component.ts`
- `src/app/features/library/library-list/library-list.component.ts`
- `src/app/features/contacts/contact-groups/contact-groups.component.ts`

### Services
- `src/app/services/auth.service.ts` (already updated with permission methods)

## Future Enhancements

1. **Route Guards:** Implement Angular route guards to prevent navigation to restricted routes
2. **Interceptor:** Add HTTP interceptor to automatically attach auth token
3. **Token Refresh:** Implement automatic token refresh before expiration
4. **Error Handling:** Show user-friendly messages when API returns 403 Forbidden
