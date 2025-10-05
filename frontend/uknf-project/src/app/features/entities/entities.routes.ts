import { Routes } from '@angular/router';

export const entitiesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./entities-list/entities-list.component').then(m => m.EntitiesListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./entity-create/entity-create.component').then(m => m.EntityCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./entity-details/entity-details.component').then(m => m.EntityDetailsComponent)
  },
  {
    path: ':id/update',
    loadComponent: () => import('./entity-update/entity-update.component').then(m => m.EntityUpdateComponent)
  }
];
