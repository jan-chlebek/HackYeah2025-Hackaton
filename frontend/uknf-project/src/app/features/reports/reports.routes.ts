import { Routes } from '@angular/router';

export const reportsRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./reports-list/reports-list.component').then(m => m.ReportsListComponent)
  },
  {
    path: 'submit',
    loadComponent: () => import('./report-submit/report-submit.component').then(m => m.ReportSubmitComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./report-details/report-details.component').then(m => m.ReportDetailsComponent)
  },
  {
    path: ':id/corrections',
    loadComponent: () => import('./report-corrections/report-corrections.component').then(m => m.ReportCorrectionsComponent)
  }
];
