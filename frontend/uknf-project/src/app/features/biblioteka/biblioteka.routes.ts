import { Routes } from '@angular/router';

export const BIBLIOTEKA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./biblioteka-list/biblioteka-list.component').then(m => m.BibliotekaListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./biblioteka-details/biblioteka-details.component').then(m => m.BibliotekaDetailsComponent)
  }
];
