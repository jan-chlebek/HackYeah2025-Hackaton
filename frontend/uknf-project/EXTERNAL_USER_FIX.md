# Permission System Fix - External User Issue

## Problem

External users (like `kontakt@pkobp.pl`) were seeing all menu components including restricted ones (Kartoteka Podmiotów, Sprawy, Adresaci, Wnioski o dostęp).

## Root Cause

The `hasElevatedPermissions()` method in `AuthService` was reading from `localStorage`, which doesn't trigger Angular's change detection. The `computed()` signals in components weren't updating when the user logged in because they were calling a method that read static data.

## Solution

Refactored `AuthService` to use Angular signals for reactive state management:

### 1. Added Signal-Based State
```typescript
// Signal for current user (reactive)
private currentUserSignal = signal<UserInfoDto | null>(this.loadUserFromStorage());
public currentUser = this.currentUserSignal.asReadonly();

// Computed signal for permission check (auto-updates)
public hasElevatedPermissionsSignal = computed(() => {
  const user = this.currentUserSignal();
  if (!user) return false;

  const elevatedRoles = ['Administrator', 'Supervisor'];
  const hasElevatedRole = user.roles.some(role => elevatedRoles.includes(role));
  const hasMoreThanInternalUser = user.permissions.length > 4;

  return hasElevatedRole || hasMoreThanInternalUser;
});
```

### 2. Updated Login/Logout
```typescript
// Login - update both BehaviorSubject and Signal
this.currentUserSubject.next(response.user);
this.currentUserSignal.set(response.user); // New

// Logout - clear both
this.currentUserSubject.next(null);
this.currentUserSignal.set(null); // New
```

### 3. Updated Components
```typescript
// Sidebar Component - use signal instead of method
visibleMenuItems = computed(() => {
  const hasElevatedPermissions = this.authService.hasElevatedPermissionsSignal(); // Changed
  // ... filter logic
});

// Dashboard Component - same change
tabs = computed(() => {
  const hasElevatedPermissions = this.authService.hasElevatedPermissionsSignal(); // Changed
  // ... filter logic
});
```

## User Roles & Permissions

| Role | Permissions | Has Elevated Access |
|------|------------|---------------------|
| **ExternalUser** | 0 | ❌ No |
| **InternalUser** | 4 | ❌ No |
| **Supervisor** | 7 | ✅ Yes |
| **Administrator** | 9 | ✅ Yes |

## Testing

### External User (Should NOT see restricted features)
```
Email: kontakt@pkobp.pl
Password: External123!
Expected: Only sees Wiadomości, Komunikaty, Biblioteka, Sprawozdawczość, Moje pytania
```

### Internal User (Should NOT see restricted features)
```
Email: jan.kowalski@uknf.gov.pl
Password: User123!
Expected: Same as external user (basic features only)
```

### Supervisor (Should see ALL features)
```
Email: anna.nowak@uknf.gov.pl
Password: Supervisor123!
Expected: Sees all menu items including restricted ones
```

### Administrator (Should see ALL features)
```
Email: admin@uknf.gov.pl
Password: Admin123!
Expected: Sees all menu items including restricted ones
```

## How Signals Fix the Issue

**Before (Broken):**
1. User logs in → data saved to `localStorage`
2. Component calls `authService.hasElevatedPermissions()`
3. Method reads from `localStorage` (static, no reactivity)
4. Computed signal doesn't know data changed
5. UI doesn't update ❌

**After (Fixed):**
1. User logs in → data saved to signal AND `localStorage`
2. Component uses `authService.hasElevatedPermissionsSignal()`
3. Signal automatically propagates changes
4. Computed signal in component auto-updates
5. UI updates reactively ✅

## Files Modified

1. `src/app/services/auth.service.ts`
   - Added `currentUserSignal` signal
   - Added `hasElevatedPermissionsSignal` computed signal
   - Updated `login()` to update signal
   - Updated `logout()` to clear signal

2. `src/app/shared/layout/sidebar/sidebar.component.ts`
   - Changed `hasElevatedPermissions()` → `hasElevatedPermissionsSignal()`

3. `src/app/features/dashboard/dashboard.component.ts`
   - Changed `hasElevatedPermissions()` → `hasElevatedPermissionsSignal()`

## API Note

The method `hasElevatedPermissions()` still exists for programmatic checks (e.g., in services or guards), but UI components should use `hasElevatedPermissionsSignal()` for reactive updates.
