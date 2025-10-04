import { Routes } from '@angular/router';

export const WIADOMOSCI_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./wiadomosci-list/wiadomosci-list.component').then(m => m.WiadomosciListComponent)
  },
  {
    path: 'compose',
    loadComponent: () => import('./wiadomosci-compose/wiadomosci-compose.component').then(m => m.WiadomosciComposeComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./wiadomosci-details/wiadomosci-details.component').then(m => m.WiadomosciDetailsComponent)
  }
];
