import { Routes } from '@angular/router';

export const routes: Routes = [
  // Landing / Home
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  // Authentication & Authorization Module
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.authRoutes)
  },

  // Dashboard (Main)
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },

  // Communication Module - Messages
  {
    path: 'messages',
    loadChildren: () => import('./features/messages/messages.routes').then(m => m.messagesRoutes)
  },

  // Communication Module - Reports (Sprawozdania)
  {
    path: 'reports',
    loadChildren: () => import('./features/reports/reports.routes').then(m => m.reportsRoutes)
  },

  // Communication Module - Library (Biblioteka)
  {
    path: 'library',
    loadChildren: () => import('./features/library/library.routes').then(m => m.libraryRoutes)
  },

  // Communication Module - Cases (Sprawy)
  {
    path: 'cases',
    loadChildren: () => import('./features/cases/cases.routes').then(m => m.casesRoutes)
  },

  // Communication Module - Announcements (Komunikaty/Tablica Ogłoszeń)
  {
    path: 'announcements',
    loadChildren: () => import('./features/announcements/announcements.routes').then(m => m.announcementsRoutes)
  },

  // Communication Module - Contacts (Kartoteka/Adresaci)
  {
    path: 'contacts',
    loadChildren: () => import('./features/contacts/contacts.routes').then(m => m.contactsRoutes)
  },

  // Communication Module - FAQ
  {
    path: 'faq',
    loadChildren: () => import('./features/faq/faq.routes').then(m => m.faqRoutes)
  },

  // Communication Module - Entity Registry (Kartoteka Podmiotów)
  {
    path: 'entities',
    loadChildren: () => import('./features/entities/entities.routes').then(m => m.entitiesRoutes)
  },

  // Admin Module
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes)
  },

  // 404 Not Found
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
