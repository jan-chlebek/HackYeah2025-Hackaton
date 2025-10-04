import { Routes } from '@angular/router';

export const SPRAWOZDANIA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./sprawozdania-list/sprawozdania-list.component').then(m => m.SprawozdaniaListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./sprawozdania-create/sprawozdania-create.component').then(m => m.SprawozdaniaCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./sprawozdania-details/sprawozdania-details.component').then(m => m.SprawozdaniaDetailsComponent)
  }
];
