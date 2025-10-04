import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full'
  },
  {
    path: 'users',
    loadComponent: () => import('./admin-users/admin-users.component').then(m => m.AdminUsersComponent)
  },
  {
    path: 'password-policy',
    loadComponent: () => import('./admin-password-policy/admin-password-policy.component').then(m => m.AdminPasswordPolicyComponent)
  },
  {
    path: 'roles',
    loadComponent: () => import('./admin-roles/admin-roles.component').then(m => m.AdminRolesComponent)
  }
];
