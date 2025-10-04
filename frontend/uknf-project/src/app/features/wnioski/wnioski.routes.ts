import { Routes } from '@angular/router';

export const WNIOSKI_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./wnioski-list/wnioski-list.component').then(m => m.WnioskiListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./wnioski-create/wnioski-create.component').then(m => m.WnioskiCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./wnioski-details/wnioski-details.component').then(m => m.WnioskiDetailsComponent)
  }
];
