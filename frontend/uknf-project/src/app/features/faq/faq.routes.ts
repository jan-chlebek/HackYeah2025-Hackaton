import { Routes } from '@angular/router';

export const faqRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./faq-list/faq-list.component').then(m => m.FaqListComponent)
  },
  {
    path: 'ask',
    loadComponent: () => import('./faq-ask/faq-ask.component').then(m => m.FaqAskComponent)
  },
  {
    path: 'manage',
    loadComponent: () => import('./faq-manage/faq-manage.component').then(m => m.FaqManageComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./faq-details/faq-details.component').then(m => m.FaqDetailsComponent)
  }
];
