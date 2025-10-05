# Authorization Temporarily Disabled - TODO Tracking

**Date**: 2025-10-04  
**Status**: ⚠️ AUTHORIZATION DISABLED FOR TESTING  
**Branch**: krzys

## ⚠️ CRITICAL: RE-ENABLE BEFORE PRODUCTION

All authorization has been temporarily disabled to allow testing without authentication. This is **NOT SAFE FOR PRODUCTION** and must be re-enabled before deployment.

## Changes Made

### 1. Program.cs - Middleware Disabled
**File**: `backend/UknfCommunicationPlatform.Api/Program.cs`

**Line ~172**: Commented out authentication and authorization middleware:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// app.UseAuthentication();
// app.UseAuthorization();
```

**Impact**: No JWT token validation occurs. All requests are allowed through without authentication.

---

### 2. EntitiesController - Class-Level Authorization
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/EntitiesController.cs`

**Line ~14**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
```

**Impact**: All entity management endpoints are publicly accessible without authentication.

**Affected Endpoints**:
- `GET /api/v1/entities` - List entities
- `GET /api/v1/entities/{id}` - Get entity details
- `POST /api/v1/entities` - Create entity
- `PUT /api/v1/entities/{id}` - Update entity
- `DELETE /api/v1/entities/{id}` - Delete entity
- `GET /api/v1/entities/{id}/users` - List entity users
- `POST /api/v1/entities/{id}/users` - Add user to entity

---

### 3. MessagesController - Class-Level Authorization
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/MessagesController.cs`

**Line ~16**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
```

**Line ~282**: Modified `GetCurrentUserId()` to return default user ID (1 = admin):
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily using hardcoded user ID for testing
var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim))
{
    _logger.LogWarning("Authorization disabled - using default user ID 1");
    return 1; // Default to admin user when auth is disabled
}
```

**Impact**: All message endpoints are publicly accessible. All operations execute as user ID 1 (admin).

**Affected Endpoints**:
- `GET /api/v1/messages` - List messages
- `GET /api/v1/messages/{id}` - Get message details
- `POST /api/v1/messages` - Send message
- `PATCH /api/v1/messages/{id}/read` - Mark as read
- `PATCH /api/v1/messages/read` - Mark multiple as read
- `DELETE /api/v1/messages/{id}` - Delete message
- `GET /api/v1/messages/stats` - Get message statistics
- `GET /api/v1/messages/folders/{folder}` - Get messages by folder
- `POST /api/v1/messages/{id}/move` - Move to folder
- `GET /api/v1/messages/search` - Search messages

---

### 4. UsersController - Class-Level Authorization
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/v1/UsersController.cs`

**Line ~15**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize] // Require authentication for all endpoints
```

**Impact**: All user management endpoints are publicly accessible without authentication.

**Affected Endpoints**:
- `GET /api/v1/users` - List users
- `GET /api/v1/users/{id}` - Get user details
- `POST /api/v1/users` - Create user
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Deactivate user
- `PUT /api/v1/users/{id}/activate` - Activate user
- `GET /api/v1/users/{id}/roles` - Get user roles
- `POST /api/v1/users/{id}/roles` - Assign role
- `DELETE /api/v1/users/{id}/roles/{roleId}` - Remove role

---

### 5. AuthController - Method-Level Authorization
**File**: `backend/UknfCommunicationPlatform.Api/Controllers/AuthController.cs`

#### 5.1 Logout Endpoint
**Line ~89**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
```

**Line ~94**: Modified to handle missing user claims:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily using hardcoded user ID for testing
var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
{
    _logger.LogWarning("Authorization disabled - logout endpoint called without authentication");
    return Ok(new { message = "Logout successful (auth disabled)" });
}
```

**Impact**: Logout endpoint always succeeds, even without authentication.

#### 5.2 Change Password Endpoint
**Line ~121**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
```

**Line ~133**: Modified to use default user ID:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily using hardcoded user ID for testing
var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
{
    _logger.LogWarning("Authorization disabled - using default user ID 1 for password change");
    userId = 1; // Default to admin user when auth is disabled
}
```

**Impact**: Password change operates on user ID 1 (admin) when called without authentication.

#### 5.3 Get Current User (me) Endpoint
**Line ~157**: Commented out [Authorize] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize]
```

**Line ~165**: Modified to return mock admin user data:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily return mock data when auth is disabled
var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
// ... (null-safe claim extraction)

if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
{
    _logger.LogWarning("Authorization disabled - returning mock user data");
    return Ok(new
    {
        userId = 1,
        email = "admin@uknf.gov.pl",
        roles = new[] { "Administrator" },
        permissions = new[] { "users.read", "users.write", "users.delete", "entities.read", 
                             "entities.write", "messages.read", "messages.write", 
                             "reports.read", "reports.write" },
        supervisedEntityId = (long?)null
    });
}
```

**Impact**: `/auth/me` returns mock admin user data when called without authentication.

#### 5.4 Get Lock Status Endpoint (Admin Only)
**Line ~205**: Commented out [Authorize(Roles = "Administrator")] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize(Roles = "Administrator")]
```

**Impact**: Any user can check if accounts are locked.

#### 5.5 Unlock Account Endpoint (Admin Only)
**Line ~222**: Commented out [Authorize(Roles = "Administrator")] attribute:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily disabled for testing
// [Authorize(Roles = "Administrator")]
```

**Line ~236**: Modified logging to handle missing admin ID:
```csharp
// TODO: RE-ENABLE AUTHORIZATION - Temporarily log without admin ID
_logger.LogInformation("Account unlocked for user {UserId} by admin (auth disabled)", userId);
```

**Impact**: Any user can unlock accounts.

---

## Security Implications

⚠️ **CRITICAL SECURITY ISSUES WITH AUTHORIZATION DISABLED**:

1. **No Authentication Required**: Anyone can access all API endpoints without providing credentials
2. **No Authorization Checks**: Role-based and permission-based access control is bypassed
3. **Data Exposure**: Sensitive user data, messages, and entity information is publicly accessible
4. **Administrative Functions**: Anyone can create/delete users, modify entities, unlock accounts
5. **Message Access**: All users can read all messages (privacy violation)
6. **Audit Trail Gaps**: Actions are logged as "auth disabled" or user ID 1, making it impossible to track who did what

## Re-enabling Authorization Checklist

To re-enable authorization, perform the following steps:

### Step 1: Program.cs
- [ ] Uncomment `app.UseAuthentication();`
- [ ] Uncomment `app.UseAuthorization();`

### Step 2: Controllers
- [ ] **EntitiesController**: Uncomment `[Authorize]` at line ~14
- [ ] **MessagesController**: Uncomment `[Authorize]` at line ~16
- [ ] **MessagesController**: Remove fallback logic in `GetCurrentUserId()` method (line ~282)
- [ ] **UsersController**: Uncomment `[Authorize]` at line ~15
- [ ] **AuthController - Logout**: Uncomment `[Authorize]` at line ~89
- [ ] **AuthController - Logout**: Remove fallback return statement (line ~94-98)
- [ ] **AuthController - Change Password**: Uncomment `[Authorize]` at line ~121
- [ ] **AuthController - Change Password**: Remove fallback user ID assignment (line ~133-137)
- [ ] **AuthController - Get Current User**: Uncomment `[Authorize]` at line ~157
- [ ] **AuthController - Get Current User**: Remove mock data return (line ~165-178)
- [ ] **AuthController - Get Lock Status**: Uncomment `[Authorize(Roles = "Administrator")]` at line ~205
- [ ] **AuthController - Unlock Account**: Uncomment `[Authorize(Roles = "Administrator")]` at line ~222
- [ ] **AuthController - Unlock Account**: Restore original logging with admin ID (line ~236)

### Step 3: Testing
- [ ] Run all backend tests to ensure authorization works correctly
- [ ] Test login/logout flow with valid credentials
- [ ] Verify role-based access control (admin vs. regular users)
- [ ] Verify permission-based access control
- [ ] Test API endpoints with and without JWT tokens
- [ ] Verify 401 Unauthorized responses for unauthenticated requests
- [ ] Verify 403 Forbidden responses for unauthorized requests

### Step 4: Code Review
- [ ] Search for all TODO comments: `TODO: RE-ENABLE AUTHORIZATION`
- [ ] Verify no hardcoded user IDs remain in production code
- [ ] Review all logging statements to ensure they properly capture user context
- [ ] Remove this tracking document

## Search Command to Find All TODOs

```bash
grep -r "TODO: RE-ENABLE AUTHORIZATION" backend/
```

Expected output:
- `Program.cs` (2 TODOs - authentication & authorization middleware)
- `EntitiesController.cs` (1 TODO - class-level authorize)
- `MessagesController.cs` (2 TODOs - class-level authorize + GetCurrentUserId method)
- `UsersController.cs` (1 TODO - class-level authorize)
- `AuthController.cs` (11 TODOs - logout, change password, me endpoint, lock status, unlock account)

**Total: 17 TODO comments to address**

## Timeline

- **Disabled**: 2025-10-04 (for development/testing)
- **Re-enable By**: ⚠️ BEFORE ANY PRODUCTION DEPLOYMENT ⚠️
- **Review Date**: TBD

## Notes

This document should be deleted once authorization is fully re-enabled and tested.
