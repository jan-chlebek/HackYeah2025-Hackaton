# Quick Verification Steps

## Step 1: Restart Angular Dev Server

```bash
# Stop current server (Ctrl+C)
# Then restart:
cd frontend/uknf-project
npm start
```

Wait for: `✔ Compiled successfully`

## Step 2: Open Browser Fresh

1. Open **new incognito window** (Ctrl+Shift+N)
2. Go to: http://localhost:4200
3. Open DevTools (F12) - **Console tab**

## Step 3: Login as External User

```
Email: kontakt@pkobp.pl
Password: External123!
```

## Step 4: Check Console Output

You should see logs like:
```
[AuthService] Loading user from storage: null
[AuthService] Login successful: { roles: ["ExternalUser"], permissions: [] }
[AuthService] Signal updated
[AuthService] Computing hasElevatedPermissions: { permissionCount: 0 }
[AuthService] hasElevatedPermissions result: { result: false }
[Sidebar] Computing visible menu items, hasElevatedPermissions: false
[Sidebar] Item "Wnioski o dostęp" requires elevated perms: false
[Sidebar] Item "Kartoteka Podmiotów" requires elevated perms: false
[Sidebar] Item "Sprawy" requires elevated perms: false
[Sidebar] Item "Adresaci" requires elevated perms: false
[Sidebar] Visible items count: 5 / 9
```

## Step 5: Verify Menu

Count the menu items in sidebar. Should see EXACTLY **5 items**:
- ✅ Wiadomości
- ✅ Komunikaty  
- ✅ Biblioteka - repozytorium plików
- ✅ Sprawozdawczość
- ✅ Moje pytania

Should NOT see:
- ❌ Wnioski o dostęp
- ❌ Kartoteka Podmiotów
- ❌ Sprawy
- ❌ Adresaci

## Troubleshooting

### If you see all 9 items:

**Check 1: Is code compiled?**
```
Look in terminal where npm start is running
Should see: "✔ Compiled successfully"
If not, restart: npm start
```

**Check 2: Is browser cached?**
```
Press: Ctrl+Shift+R (hard refresh)
Or: Open incognito window
```

**Check 3: Check console logs**
```
If NO logs appear → Code not loaded, refresh browser
If logs show "result: true" → Share the logs with me
If logs show "result: false" but menu has 9 items → Browser cache issue
```

**Check 4: Clear everything**
```javascript
// In browser console:
localStorage.clear();
sessionStorage.clear();
location.reload();
```

## Quick Test: Compare Users

### Test 1: External User (should see 5 items)
- Email: `kontakt@pkobp.pl`
- Password: `External123!`
- Console should show: `hasElevatedPermissions: false`
- Menu should show: **5 items**

### Test 2: Administrator (should see 9 items)  
- Logout
- Email: `admin@uknf.gov.pl`
- Password: `Admin123!`
- Console should show: `hasElevatedPermissions: true`
- Menu should show: **9 items**

If admin sees 9 items but external still sees 9, then the signal isn't updating properly.

## If Still Not Working

Run this in browser console after logging in as external user:

```javascript
// Check what the service thinks
const authService = document.querySelector('app-root')?.__ngContext__?.[8]?.get(AuthService);
console.log('Current user:', authService?.currentUserSignal());
console.log('Has elevated:', authService?.hasElevatedPermissionsSignal());
```

Or simply share a screenshot of:
1. The browser console logs
2. The sidebar menu (how many items you see)
