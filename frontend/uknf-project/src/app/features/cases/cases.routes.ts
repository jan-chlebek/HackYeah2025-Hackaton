import { Routes } from '@angular/router';

export const casesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./cases-list/cases-list.component').then(m => m.CasesListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./case-create/case-create.component').then(m => m.CaseCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./case-details/case-details.component').then(m => m.CaseDetailsComponent)
  }
];
