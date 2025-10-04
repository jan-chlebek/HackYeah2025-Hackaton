import { Routes } from '@angular/router';

export const KOMUNIKATY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./komunikaty-list/komunikaty-list.component').then(m => m.KomunikatyListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./komunikaty-create/komunikaty-create.component').then(m => m.KomunikatyCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./komunikaty-details/komunikaty-details.component').then(m => m.KomunikatyDetailsComponent)
  }
];
