import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full'
  },
  {
    path: 'users',
    loadComponent: () => import('./users/users-list.component').then(m => m.UsersListComponent)
  },
  {
    path: 'users/create',
    loadComponent: () => import('./users/user-create.component').then(m => m.UserCreateComponent)
  },
  {
    path: 'users/:id',
    loadComponent: () => import('./users/user-details.component').then(m => m.UserDetailsComponent)
  },
  {
    path: 'password-policy',
    loadComponent: () => import('./password-policy/password-policy.component').then(m => m.PasswordPolicyComponent)
  },
  {
    path: 'roles',
    loadComponent: () => import('./roles/roles-list.component').then(m => m.RolesListComponent)
  },
  {
    path: 'roles/create',
    loadComponent: () => import('./roles/role-create.component').then(m => m.RoleCreateComponent)
  },
  {
    path: 'roles/:id',
    loadComponent: () => import('./roles/role-details.component').then(m => m.RoleDetailsComponent)
  }
];
