import { Routes } from '@angular/router';

export const KARTOTEKA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./kartoteka-list/kartoteka-list.component').then(m => m.KartotekaListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./kartoteka-details/kartoteka-details.component').then(m => m.KartotekaDetailsComponent)
  },
  {
    path: ':id/update',
    loadComponent: () => import('./kartoteka-update/kartoteka-update.component').then(m => m.KartotekaUpdateComponent)
  }
];
