import { Routes } from '@angular/router';

export const libraryRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./library-list/library-list.component').then(m => m.LibraryListComponent)
  },
  {
    path: 'upload',
    loadComponent: () => import('./file-upload/file-upload.component').then(m => m.FileUploadComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./file-details/file-details.component').then(m => m.FileDetailsComponent)
  }
];
