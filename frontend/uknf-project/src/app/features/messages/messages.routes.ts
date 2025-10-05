import { Routes } from '@angular/router';

export const messagesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./messages-list/messages-list.component').then(m => m.MessagesListComponent)
  },
  {
    path: 'compose',
    loadComponent: () => import('./message-compose/message-compose.component').then(m => m.MessageComposeComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./message-details/message-details.component').then(m => m.MessageDetailsComponent)
  }
];
