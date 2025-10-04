import { Routes } from '@angular/router';

export const FAQ_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./faq-list/faq-list.component').then(m => m.FaqListComponent)
  },
  {
    path: 'submit',
    loadComponent: () => import('./faq-submit/faq-submit.component').then(m => m.FaqSubmitComponent)
  },
  {
    path: 'manage',
    loadComponent: () => import('./faq-manage/faq-manage.component').then(m => m.FaqManageComponent)
  }
];
