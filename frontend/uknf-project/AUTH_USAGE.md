# Authentication & Authorization Usage Guide

## Overview

The login page now connects to the real backend API (`/api/v1/Auth/login`) and handles user roles and permissions properly.

## API Integration

### Login Endpoint

**Endpoint:** `POST http://localhost:5000/api/v1/Auth/login`

**Request:**
```json
{
  "email": "admin@uknf.gov.pl",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh-token-string",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "email": "admin@uknf.gov.pl",
    "fullName": "Administrator Systemowy",
    "roles": ["Administrator"],
    "permissions": [
      "ViewMessages",
      "SendMessages",
      "ViewFAQs",
      "ManageFAQs",
      "ViewLibrary",
      "ManageLibrary",
      "ViewEntities",
      "ManageEntities",
      "ManageUsers"
    ],
    "supervisedEntityId": null
  }
}
```

## User Roles & Permissions

### Role Hierarchy

| Role | Permissions Count | Description |
|------|-------------------|-------------|
| **Internal User** | 4 | Basic access to view messages, FAQs, library, entities |
| **Supervisor** | 7 | Can manage FAQs, library, entities (no user management) |
| **Administrator** | 9 | Full access including user management |

### Permission List

- `ViewMessages` - View messages
- `SendMessages` - Send messages
- `ViewFAQs` - View FAQs
- `ManageFAQs` - Create/edit/delete FAQs
- `ViewLibrary` - View library files
- `ManageLibrary` - Upload/edit/delete library files
- `ViewEntities` - View supervised entities
- `ManageEntities` - Create/edit/delete entities
- `ManageUsers` - Manage users (admin only)

## Using Auth Service

### Basic Authentication

```typescript
import { inject } from '@angular/core';
import { AuthService } from '@app/services/auth.service';

export class MyComponent {
  private authService = inject(AuthService);

  login() {
    this.authService.login({
      email: 'admin@uknf.gov.pl',
      password: 'Admin123!'
    }).subscribe({
      next: (response) => {
        console.log('Logged in:', response.user);
        // Token is automatically stored in localStorage
      },
      error: (error) => {
        console.error('Login failed:', error);
      }
    });
  }
}
```

### Checking Permissions

#### Method 1: Check Elevated Permissions

```typescript
import { computed, inject } from '@angular/core';
import { AuthService } from '@app/services/auth.service';

export class MyComponent {
  private authService = inject(AuthService);
  
  // Returns true for Supervisor or Administrator
  hasElevatedPermissions = computed(() => 
    this.authService.hasElevatedPermissions()
  );
  
  showAdminFeature() {
    if (this.hasElevatedPermissions()) {
      // Show advanced features
    }
  }
}
```

#### Method 2: Check Specific Permission

```typescript
canManageUsers = computed(() => 
  this.authService.hasPermission('ManageUsers')
);

canManageFAQs = computed(() => 
  this.authService.hasPermission('ManageFAQs')
);
```

#### Method 3: Check Role

```typescript
isAdmin = computed(() => 
  this.authService.hasRole('Administrator')
);

isSupervisor = computed(() => 
  this.authService.hasRole('Supervisor')
);
```

### Template Usage

```html
<!-- Show button only for users with elevated permissions -->
@if (hasElevatedPermissions()) {
  <button (click)="deleteEntity()">Delete</button>
}

<!-- Show admin panel only for administrators -->
@if (authService.hasRole('Administrator')) {
  <app-admin-panel />
}

<!-- Show edit button only if user can manage FAQs -->
@if (authService.hasPermission('ManageFAQs')) {
  <button [routerLink]="['/faq/edit', faq.id]">Edit FAQ</button>
}
```

### Getting Current User

```typescript
const user = this.authService.getCurrentUser();
if (user) {
  console.log('User:', user.fullName);
  console.log('Roles:', user.roles);
  console.log('Permissions:', user.permissions);
}
```

### Logout

```typescript
logout() {
  this.authService.logout();
  this.router.navigate(['/auth/login']);
}
```

## Test Credentials

From the seeded database:

### Administrator (9 permissions)
- Email: `admin@uknf.gov.pl`
- Password: `Admin123!`

### Supervisor (7 permissions)
- Email: `anna.nowak@uknf.gov.pl`
- Password: `Supervisor123!`

### Internal User (4 permissions)
- Email: `jan.kowalski@uknf.gov.pl`
- Password: `User123!`

## Example: Dashboard with Permission-Based Features

```typescript
import { Component, inject, computed } from '@angular/core';
import { AuthService } from '@app/services/auth.service';

@Component({
  selector: 'app-dashboard',
  template: `
    <div class="dashboard">
      <h1>Welcome, {{ currentUser()?.fullName }}</h1>
      
      <div class="user-info">
        <p>Roles: {{ currentUser()?.roles.join(', ') }}</p>
        <p>Permissions: {{ currentUser()?.permissions.length }}</p>
        
        @if (hasElevatedPermissions()) {
          <span class="badge">Elevated Access</span>
        }
      </div>

      <!-- Basic features for all users -->
      <section>
        <button routerLink="/messages">View Messages</button>
        <button routerLink="/faq">View FAQs</button>
      </section>

      <!-- Advanced features for elevated users -->
      @if (hasElevatedPermissions()) {
        <section>
          <h2>Management Tools</h2>
          
          @if (canManageFAQs()) {
            <button routerLink="/faq/manage">Manage FAQs</button>
          }
          
          @if (canManageEntities()) {
            <button routerLink="/entities/manage">Manage Entities</button>
          }
        </section>
      }

      <!-- Admin-only features -->
      @if (isAdmin()) {
        <section>
          <h2>Administration</h2>
          <button routerLink="/admin/users">User Management</button>
          <button routerLink="/admin/reports">System Reports</button>
        </section>
      }
    </div>
  `
})
export class DashboardComponent {
  private authService = inject(AuthService);
  
  currentUser = computed(() => this.authService.getCurrentUser());
  hasElevatedPermissions = computed(() => this.authService.hasElevatedPermissions());
  isAdmin = computed(() => this.authService.hasRole('Administrator'));
  canManageFAQs = computed(() => this.authService.hasPermission('ManageFAQs'));
  canManageEntities = computed(() => this.authService.hasPermission('ManageEntities'));
}
```

## Token Storage

Tokens are automatically stored in `localStorage`:
- `accessToken` - JWT access token (valid for 1 hour)
- `refreshToken` - Refresh token for obtaining new access tokens
- `tokenType` - Token type (usually "Bearer")
- `expiresIn` - Token expiration time in seconds
- `currentUser` - Serialized user information

## Security Notes

- All API calls should include the access token in the `Authorization` header: `Bearer {accessToken}`
- Tokens are stored in localStorage (consider using httpOnly cookies for production)
- The backend validates permissions on every protected endpoint
- Use HTTPS in production to protect tokens in transit

## Next Steps

1. **Add HTTP Interceptor**: Automatically attach tokens to API requests
2. **Add Auth Guard**: Protect routes based on permissions
3. **Token Refresh**: Implement automatic token refresh before expiration
4. **Logout on Expired Token**: Clear session when token expires
