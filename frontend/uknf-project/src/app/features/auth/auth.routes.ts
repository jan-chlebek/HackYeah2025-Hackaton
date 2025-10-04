import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'access-requests',
    loadComponent: () => import('./access-requests/access-requests-list.component').then(m => m.AccessRequestsListComponent)
  },
  {
    path: 'access-requests/:id',
    loadComponent: () => import('./access-requests/access-request-details.component').then(m => m.AccessRequestDetailsComponent)
  },
  {
    path: 'select-entity',
    loadComponent: () => import('./select-entity/select-entity.component').then(m => m.SelectEntityComponent)
  }
];
