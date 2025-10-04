import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'sprawozdania',
    loadChildren: () => import('./features/sprawozdania/sprawozdania.routes').then(m => m.SPRAWOZDANIA_ROUTES)
  },
  {
    path: 'wiadomosci',
    loadChildren: () => import('./features/wiadomosci/wiadomosci.routes').then(m => m.WIADOMOSCI_ROUTES)
  },
  {
    path: 'sprawy',
    loadChildren: () => import('./features/sprawy/sprawy.routes').then(m => m.SPRAWY_ROUTES)
  },
  {
    path: 'biblioteka',
    loadChildren: () => import('./features/biblioteka/biblioteka.routes').then(m => m.BIBLIOTEKA_ROUTES)
  },
  {
    path: 'komunikaty',
    loadChildren: () => import('./features/komunikaty/komunikaty.routes').then(m => m.KOMUNIKATY_ROUTES)
  },
  {
    path: 'faq',
    loadChildren: () => import('./features/faq/faq.routes').then(m => m.FAQ_ROUTES)
  },
  {
    path: 'kartoteka',
    loadChildren: () => import('./features/kartoteka/kartoteka.routes').then(m => m.KARTOTEKA_ROUTES)
  },
  {
    path: 'wnioski',
    loadChildren: () => import('./features/wnioski/wnioski.routes').then(m => m.WNIOSKI_ROUTES)
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
