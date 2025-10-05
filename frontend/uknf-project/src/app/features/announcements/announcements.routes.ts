import { Routes } from '@angular/router';

export const announcementsRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./announcements-list/announcements-list.component').then(m => m.AnnouncementsListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./announcement-create/announcement-create.component').then(m => m.AnnouncementCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./announcement-details/announcement-details.component').then(m => m.AnnouncementDetailsComponent)
  }
];
