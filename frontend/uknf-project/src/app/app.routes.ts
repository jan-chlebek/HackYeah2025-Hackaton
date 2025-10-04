import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'sprawozdania',
    loadChildren: () => import('./features/sprawozdania/sprawozdania.routes').then(m => m.SPRAWOZDANIA_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'wiadomosci',
    loadChildren: () => import('./features/wiadomosci/wiadomosci.routes').then(m => m.WIADOMOSCI_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'sprawy',
    loadChildren: () => import('./features/sprawy/sprawy.routes').then(m => m.SPRAWY_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'biblioteka',
    loadChildren: () => import('./features/biblioteka/biblioteka.routes').then(m => m.BIBLIOTEKA_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'komunikaty',
    loadChildren: () => import('./features/komunikaty/komunikaty.routes').then(m => m.KOMUNIKATY_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'faq',
    loadChildren: () => import('./features/faq/faq.routes').then(m => m.FAQ_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'kartoteka',
    loadChildren: () => import('./features/kartoteka/kartoteka.routes').then(m => m.KARTOTEKA_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'wnioski',
    loadChildren: () => import('./features/wnioski/wnioski.routes').then(m => m.WNIOSKI_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES),
    canActivate: [roleGuard(['Administrator systemu'])]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
