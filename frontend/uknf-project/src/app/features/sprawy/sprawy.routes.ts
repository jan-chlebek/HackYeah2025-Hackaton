import { Routes } from '@angular/router';

export const SPRAWY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./sprawy-list/sprawy-list.component').then(m => m.SprawyListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./sprawy-create/sprawy-create.component').then(m => m.SprawyCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./sprawy-details/sprawy-details.component').then(m => m.SprawyDetailsComponent)
  }
];
