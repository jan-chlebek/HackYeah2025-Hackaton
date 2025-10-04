# Administrative Module - Requirements & Implementation Plan

**Branch:** krzys
**Date:** 2025-10-04
**Status:** Planning Phase

## 📋 Executive Summary

The **Administrative Module (Moduł Administracyjny)** is one of the three core modules of the UKNF Communication Platform. It provides system administrators (UKNF employees) with tools to manage users, configure security policies, manage roles/permissions, and maintain the supervised entities registry.

**Current Backend Status:** 0% implemented for Admin Module
**Priority:** HIGH (required for complete platform functionality)

---

## 🎯 Module Objectives

Per requirements (DETAILS_UKNF_Prompt2Code2.md, lines 127-145):

1. **User Account Management** - Manage internal (UKNF staff) and external (entity representatives) user accounts
2. **Password Policy Management** - Configure password complexity, expiration, history
3. **Role & Permission Management** - Define roles, assign permissions, assign users to roles
4. **Supervised Entity Registry** - Maintain database of supervised financial institutions

---

## 👥 User Roles

### Internal Users (UKNF)
- **System Administrator** - Full system access, manages all users and entities
- **UKNF Employee** - Standard UKNF staff with operational access

### External Users (Supervised Entities)
- **Entity Administrator** - Manages users within their supervised entity
- **Entity Employee** - Standard employee of supervised entity

---

## 🏗️ Required Backend Components

### 1. Entities (Already Exist in Core)

✅ **User.cs** - Already implemented with:
- Id, Email, PasswordHash, FirstName, LastName
- Role (enum: SystemAdmin, UKNFEmployee, EntityAdmin, EntityEmployee)
- SupervisedEntityId (FK)
- IsActive, CreatedAt, UpdatedAt
- Navigation: SupervisedEntity, Reports, Messages

✅ **SupervisedEntity.cs** - Already implemented with:
- Id, Name, EntityType, UknfCode, LEI, NIP, REGON, KRS
- Address fields (Street, BuildingNumber, ApartmentNumber, PostalCode, City, Country)
- Contact fields (Phone, Email, Website)
- IsActive, CreatedAt, UpdatedAt
- Navigation: Users, Reports

❌ **NEW: Role.cs** - Need to create:
```csharp
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } // e.g., "SystemAdmin", "EntityAdmin"
    public string Description { get; set; }
    public bool IsSystemRole { get; set; } // Cannot be deleted
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

❌ **NEW: Permission.cs** - Need to create:
```csharp
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } // e.g., "users.create", "reports.view"
    public string Resource { get; set; } // e.g., "users", "reports"
    public string Action { get; set; } // e.g., "create", "read", "update", "delete"
    public string Description { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; }
}
```

❌ **NEW: RolePermission.cs** - Junction table:
```csharp
public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public Role Role { get; set; }
    public Permission Permission { get; set; }
}
```

❌ **NEW: UserRole.cs** - Junction table (many-to-many):
```csharp
public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
```

❌ **NEW: PasswordPolicy.cs** - Configuration entity:
```csharp
public class PasswordPolicy
{
    public int Id { get; set; }
    public int MinLength { get; set; } // e.g., 12
    public bool RequireUppercase { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequireSpecialChar { get; set; }
    public int ExpirationDays { get; set; } // 0 = never expires
    public int HistoryCount { get; set; } // Prevent reuse of last N passwords
    public int MaxFailedAttempts { get; set; } // Lock account after N failures
    public int LockoutDurationMinutes { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UpdatedByUserId { get; set; }
}
```

❌ **NEW: AuditLog.cs** - Track admin actions:
```csharp
public class AuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; } // Null for system actions
    public string Action { get; set; } // e.g., "UserCreated", "PasswordReset"
    public string Resource { get; set; } // e.g., "User", "Role"
    public int? ResourceId { get; set; }
    public string Details { get; set; } // JSON with change details
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }

    public User User { get; set; }
}
```

### 2. DTOs to Create

**User Management:**
- `CreateUserRequest` - Email, FirstName, LastName, RoleIds[], SupervisedEntityId (optional)
- `UpdateUserRequest` - FirstName, LastName, IsActive, RoleIds[]
- `UserResponse` - Full user details with roles and entity name
- `UserListItemResponse` - Simplified for table view
- `SetPasswordRequest` - UserId, NewPassword, RequireChangeOnLogin
- `ResetPasswordRequest` - UserId

**Entity Management:**
- `CreateEntityRequest` - All entity fields
- `UpdateEntityRequest` - Editable fields
- `EntityResponse` - Full entity details with user count
- `EntityListItemResponse` - Simplified for table view

**Role Management:**
- `CreateRoleRequest` - Name, Description, PermissionIds[]
- `UpdateRoleRequest` - Description, PermissionIds[]
- `RoleResponse` - Role with permissions list
- `AssignRoleRequest` - UserIds[], RoleId

**Permission Management:**
- `PermissionResponse` - Id, Name, Resource, Action, Description

**Password Policy:**
- `PasswordPolicyResponse` - All policy settings
- `UpdatePasswordPolicyRequest` - Policy configuration

**Audit:**
- `AuditLogResponse` - Log entry with user details
- `AuditLogFilterRequest` - UserId, Action, Resource, DateFrom, DateTo

### 3. Controllers to Create

#### ❌ **UsersController** (Priority: CRITICAL)
```
GET    /api/v1/users                    - List users (with filtering, pagination)
GET    /api/v1/users/{id}               - Get user details
POST   /api/v1/users                    - Create user account
PUT    /api/v1/users/{id}               - Update user account
DELETE /api/v1/users/{id}               - Delete user account (soft delete)
POST   /api/v1/users/{id}/set-password  - Set password for user
POST   /api/v1/users/{id}/reset-password - Reset password (generate temporary)
POST   /api/v1/users/{id}/activate      - Activate user account
POST   /api/v1/users/{id}/deactivate    - Deactivate user account
POST   /api/v1/users/{id}/unlock        - Unlock locked account
```

#### ❌ **EntitiesController** (Priority: HIGH)
```
GET    /api/v1/entities                 - List supervised entities (with filtering, pagination)
GET    /api/v1/entities/{id}            - Get entity details with users
POST   /api/v1/entities                 - Create supervised entity
PUT    /api/v1/entities/{id}            - Update entity details
DELETE /api/v1/entities/{id}            - Delete entity (soft delete)
GET    /api/v1/entities/{id}/users      - List users assigned to entity
POST   /api/v1/entities/import          - Bulk import from CSV (test data)
```

#### ❌ **RolesController** (Priority: HIGH)
```
GET    /api/v1/roles                    - List all roles
GET    /api/v1/roles/{id}               - Get role with permissions
POST   /api/v1/roles                    - Create new role
PUT    /api/v1/roles/{id}               - Update role (name, description, permissions)
DELETE /api/v1/roles/{id}               - Delete role (only non-system roles)
POST   /api/v1/roles/{id}/assign        - Assign role to users
POST   /api/v1/roles/{id}/revoke        - Revoke role from users
GET    /api/v1/roles/{id}/users         - List users with this role
```

#### ❌ **PermissionsController** (Priority: MEDIUM)
```
GET    /api/v1/permissions              - List all available permissions
GET    /api/v1/permissions/by-resource  - Group permissions by resource
```

#### ❌ **PasswordPolicyController** (Priority: MEDIUM)
```
GET    /api/v1/password-policy          - Get current password policy
PUT    /api/v1/password-policy          - Update password policy
POST   /api/v1/password-policy/force-change - Force password change for user(s)
```

#### ❌ **AuditController** (Priority: LOW)
```
GET    /api/v1/audit                    - List audit logs (with filtering, pagination)
GET    /api/v1/audit/{id}               - Get specific audit log entry
GET    /api/v1/audit/export             - Export audit logs (CSV/Excel)
```

### 4. Services to Create

- **UserManagementService** - User CRUD, password operations, account status
- **EntityManagementService** - Entity CRUD, entity import from CSV
- **RoleManagementService** - Role CRUD, permission assignment, user-role mapping
- **PasswordPolicyService** - Policy validation, password strength checking, history tracking
- **AuditService** - Log creation, query with filters

### 5. Authorization Requirements

All admin endpoints must require authentication and proper authorization:

```csharp
[Authorize(Roles = "SystemAdmin")] // Most admin operations
[Authorize(Roles = "SystemAdmin,EntityAdmin")] // Some operations for entity admins
```

**Authorization Matrix:**

| Operation | SystemAdmin | UKNFEmployee | EntityAdmin | EntityEmployee |
|-----------|-------------|--------------|-------------|----------------|
| Manage all users | ✅ | ❌ | ❌ | ❌ |
| Manage entity users | ✅ | ❌ | ✅ (own entity) | ❌ |
| Manage entities | ✅ | ❌ | ❌ | ❌ |
| Manage roles | ✅ | ❌ | ❌ | ❌ |
| View audit logs | ✅ | ✅ (limited) | ❌ | ❌ |
| Configure password policy | ✅ | ❌ | ❌ | ❌ |

---

## 📊 Database Schema Changes

### New Tables Required:
1. `roles` - Role definitions
2. `permissions` - Permission definitions
3. `role_permissions` - Many-to-many junction
4. `user_roles` - Many-to-many junction (User ↔ Role)
5. `password_policies` - Single-row config table
6. `password_history` - Track password changes for history validation
7. `audit_logs` - Comprehensive audit trail

### Modified Tables:
- `users` - Add fields:
  - `last_password_change_at`
  - `failed_login_attempts`
  - `locked_until`
  - `require_password_change`

### Seed Data Required:
- Default roles: SystemAdmin, UKNFEmployee, EntityAdmin, EntityEmployee
- Default permissions for all resources/actions
- Default password policy
- Initial system admin user

---

## 🔐 Non-Functional Requirements

### Security (Critical):
- ✅ **Password hashing:** BCrypt with salt (min work factor 12)
- ✅ **Password validation:** Enforce current policy rules
- ✅ **Password history:** Prevent reuse of last N passwords (stored as hashes)
- ✅ **Account lockout:** Auto-lock after failed attempts, time-based unlock
- ✅ **Audit logging:** Log ALL administrative actions with IP, timestamp, details
- ✅ **Input validation:** Strict validation on all user inputs
- ✅ **HTTPS only:** All admin endpoints must use HTTPS
- ✅ **CSRF protection:** Anti-forgery tokens

### Performance:
- Pagination on all list endpoints (max 100 items per page)
- Indexes on: `users.email`, `users.is_active`, `entities.nip`, `entities.regon`, `audit_logs.timestamp`
- Caching for password policy (rarely changes)

### Usability:
- Clear error messages for validation failures
- Consistent response format (problem+json for errors)
- Comprehensive Swagger documentation

---

## 🎯 Implementation Priority

### Sprint 1: Core User & Entity Management (CRITICAL)
**Goal:** Enable admin to manage users and entities

1. ✅ Create new entities: Role, Permission, RolePermission, UserRole, PasswordPolicy, AuditLog
2. ✅ Update ApplicationDbContext with new DbSets and configurations
3. ✅ Generate and apply EF Core migration
4. ✅ Create seed data (roles, permissions, default policy)
5. ✅ Implement UsersController (all endpoints)
6. ✅ Implement EntitiesController (all endpoints)
7. ✅ Create DTOs for users and entities
8. ✅ Implement UserManagementService
9. ✅ Implement EntityManagementService
10. ✅ Add password hashing (BCrypt.Net-Next)
11. ✅ Basic authorization attributes

**Deliverable:** Admins can create/edit/delete users and entities

### Sprint 2: Roles, Permissions & Password Policy (HIGH)
**Goal:** Enable role-based access control and password security

1. ✅ Implement RolesController (all endpoints)
2. ✅ Implement PermissionsController (read-only)
3. ✅ Implement PasswordPolicyController
4. ✅ Create DTOs for roles, permissions, password policy
5. ✅ Implement RoleManagementService
6. ✅ Implement PasswordPolicyService (validation, history checking)
7. ✅ Add password history tracking
8. ✅ Add account lockout logic
9. ✅ Enforce password policy on password changes

**Deliverable:** Full RBAC system, secure password management

### Sprint 3: Audit Logging & CSV Import (MEDIUM)
**Goal:** Complete audit trail and bulk data import

1. ✅ Implement AuditController (all endpoints)
2. ✅ Implement AuditService with filtering
3. ✅ Add audit logging to all admin operations
4. ✅ Implement CSV import for entities (use test data file)
5. ✅ Add audit log export (CSV)
6. ✅ Add IP address tracking

**Deliverable:** Complete audit system, bulk entity import

---

## 🧪 Testing Requirements

### Unit Tests:
- PasswordPolicyService validation logic
- Password hashing and verification
- Role permission checking
- User account lockout logic

### Integration Tests:
- UsersController CRUD operations
- EntitiesController CRUD operations
- RolesController permission assignment
- Password policy enforcement
- Audit log creation

### Manual Test Scenarios:
1. Create user → assign role → verify permissions
2. Update password policy → attempt weak password → verify rejection
3. Multiple failed logins → verify account lock → unlock account
4. Import CSV with entity data → verify creation
5. Admin creates entity admin → entity admin manages own users
6. View audit logs → filter by action → verify details

---

## 📦 Required NuGet Packages

Already have:
- ✅ Microsoft.EntityFrameworkCore
- ✅ Npgsql.EntityFrameworkCore.PostgreSQL

Need to add:
- ❌ **BCrypt.Net-Next** - Password hashing
- ❌ **CsvHelper** - CSV import/export (optional, for bulk entity import)

---

## 🎨 Frontend Components (Out of Scope for Backend Analysis)

The administrative module will need Angular components for:
- User management table with CRUD
- Entity management table with CRUD
- Role management with permission assignment
- Password policy configuration panel
- Audit log viewer with filtering
- CSV upload for entity import

---

## 📝 API Documentation Requirements

All endpoints must be documented in Swagger with:
- Summary and description
- Request/response examples
- Authorization requirements
- Possible error responses (400, 401, 403, 404, 500)

---

## ✅ Success Criteria

The Administrative Module is complete when:

1. ✅ System admins can create, edit, and delete user accounts
2. ✅ System admins can create, edit, and delete supervised entities
3. ✅ System admins can create custom roles with specific permissions
4. ✅ System admins can assign/revoke roles to/from users
5. ✅ Entity admins can manage users within their own entity
6. ✅ Password policy is configurable and enforced
7. ✅ Failed login attempts trigger account lockout
8. ✅ Password history prevents reuse
9. ✅ All administrative actions are logged in audit trail
10. ✅ Audit logs can be filtered and exported
11. ✅ Bulk entity import works from CSV
12. ✅ All endpoints have Swagger documentation
13. ✅ Authorization is enforced correctly for all roles

---

## 🚀 Next Steps

**Immediate Action:** Start with Sprint 1 - Core User & Entity Management

**First Implementation Task:**
1. Create new entity classes (Role, Permission, etc.)
2. Update ApplicationDbContext
3. Create and apply migration
4. Implement UsersController with basic CRUD

**Estimated Effort:**
- Sprint 1: 1-2 days
- Sprint 2: 1 day
- Sprint 3: 0.5 day
- **Total: 2.5-3.5 days**

---

**Question for User:** Shall I start implementing Sprint 1 (Core User & Entity Management)? This includes creating the entities, migration, UsersController, and EntitiesController with full CRUD operations.
