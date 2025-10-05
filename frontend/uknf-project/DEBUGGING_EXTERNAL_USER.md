# External User Menu Visibility - Debugging Guide

## Issue
External user `kontakt@pkobp.pl` is seeing all menu items including restricted ones (Kartoteka Podmiotów, Sprawy, Adresaci, Wnioski o dostęp).

## Expected Behavior

### External User Profile
```json
{
  "roles": ["ExternalUser"],
  "permissions": [],
  "supervisedEntityId": 1
}
```

**Should see:**
- ✅ Wiadomości
- ✅ Komunikaty
- ✅ Biblioteka - repozytorium plików
- ✅ Sprawozdawczość
- ✅ Moje pytania

**Should NOT see:**
- ❌ Wnioski o dostęp
- ❌ Kartoteka Podmiotów
- ❌ Sprawy
- ❌ Adresaci

## Debugging Steps

### 1. Check Browser Console Logs

After logging in with `kontakt@pkobp.pl` / `External123!`, you should see:

```
[AuthService] Login successful: {
  id: 11,
  email: "kontakt@pkobp.pl",
  fullName: "Przedstawiciel PKO Bank Polski S.A.",
  roles: ["ExternalUser"],
  permissions: [],
  supervisedEntityId: 1
}

[AuthService] Signal updated, new value: {...}

[AuthService] Computing hasElevatedPermissions: {
  user: {...},
  roles: ["ExternalUser"],
  permissions: [],
  permissionCount: 0
}

[AuthService] hasElevatedPermissions result: {
  hasElevatedRole: false,
  hasMoreThanInternalUser: false,
  result: false
}

[Sidebar] Computing visible menu items, hasElevatedPermissions: false

[Sidebar] Item "Wnioski o dostęp" requires elevated perms: false
[Sidebar] Item "Kartoteka Podmiotów" requires elevated perms: false
[Sidebar] Item "Sprawy" requires elevated perms: false
[Sidebar] Item "Adresaci" requires elevated perms: false

[Sidebar] Visible items count: 5 / 9
```

### 2. Check localStorage

Open DevTools → Application → Local Storage → http://localhost:4200

You should see:
```
accessToken: "eyJhbGci..."
refreshToken: "BRKPbVv..."
currentUser: {"id":11,"email":"kontakt@pkobp.pl","fullName":"Przedstawiciel PKO Bank Polski S.A.","roles":["ExternalUser"],"permissions":[],"supervisedEntityId":1}
```

### 3. Verify Signal Logic

The logic in `hasElevatedPermissionsSignal`:

```typescript
// For ExternalUser:
const elevatedRoles = ['Administrator', 'Supervisor'];
const hasElevatedRole = ['ExternalUser'].some(role => 
  ['Administrator', 'Supervisor'].includes(role)
); 
// Result: false ✓

const hasMoreThanInternalUser = [].length > 4;
// Result: 0 > 4 = false ✓

return false || false = false ✓
```

### 4. Check If Old Code Exists

Search for any usage of the OLD method:
```bash
# This should NOT exist in sidebar or dashboard:
this.authService.hasElevatedPermissions()
```

Should use:
```typescript
this.authService.hasElevatedPermissionsSignal()
```

### 5. Clear Browser Cache

If the issue persists:
1. Open DevTools (F12)
2. Right-click Refresh button → "Empty Cache and Hard Reload"
3. Or: Clear all site data via Application → Clear storage

### 6. Check Network Tab

In Network tab, verify the login response:
```json
POST /api/v1/Auth/login

Response:
{
  "accessToken": "...",
  "user": {
    "roles": ["ExternalUser"],
    "permissions": []
  }
}
```

## Testing Different Users

### External User (0 permissions)
```
Email: kontakt@pkobp.pl
Password: External123!
Expected Menu Items: 5 (NO restricted items)
```

### Internal User (4 permissions)
```
Email: jan.kowalski@uknf.gov.pl
Password: User123!
Expected Menu Items: 5 (NO restricted items)
```

### Supervisor (7 permissions)
```
Email: anna.nowak@uknf.gov.pl
Password: Supervisor123!
Expected Menu Items: 9 (ALL items including restricted)
```

### Administrator (9 permissions)
```
Email: admin@uknf.gov.pl
Password: Admin123!
Expected Menu Items: 9 (ALL items including restricted)
```

## Common Issues

### Issue: Menu shows all items after login
**Cause:** Old cached code or localStorage from previous session  
**Fix:** Clear browser cache and localStorage, then login again

### Issue: Console shows no logs
**Cause:** Application not recompiled with new logging code  
**Fix:** Stop dev server, run `npm start` again

### Issue: Signal not updating
**Cause:** Using method `hasElevatedPermissions()` instead of signal  
**Fix:** Use `hasElevatedPermissionsSignal()` in components

### Issue: Role name mismatch
**Backend returns:** `"ExternalUser"` (no space)  
**Frontend checks:** `['Administrator', 'Supervisor']`  
**Result:** External users correctly excluded ✓

## Files to Check

1. `src/app/services/auth.service.ts` - Signal definition and login logic
2. `src/app/shared/layout/sidebar/sidebar.component.ts` - Menu filtering
3. `src/app/features/dashboard/dashboard.component.ts` - Tab filtering

## Quick Fix Commands

```bash
# Clear node_modules and reinstall
cd frontend/uknf-project
rm -rf node_modules
npm install

# Restart dev server
npm start

# Clear browser storage in DevTools console
localStorage.clear()
sessionStorage.clear()
location.reload()
```

## Expected Console Output (External User)

When everything works correctly:

```
✓ [AuthService] Login successful
✓ [AuthService] Signal updated
✓ [AuthService] hasElevatedPermissions result: { result: false }
✓ [Sidebar] hasElevatedPermissions: false
✓ [Sidebar] Visible items count: 5 / 9
```

The menu should show exactly **5 items** and hide **4 restricted items**.
