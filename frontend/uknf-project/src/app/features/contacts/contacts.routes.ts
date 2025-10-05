import { Routes } from '@angular/router';

export const contactsRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./contacts-list/contacts-list.component').then(m => m.ContactsListComponent)
  },
  {
    path: 'groups',
    loadComponent: () => import('./contact-groups/contact-groups.component').then(m => m.ContactGroupsComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./contact-create/contact-create.component').then(m => m.ContactCreateComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./contact-details/contact-details.component').then(m => m.ContactDetailsComponent)
  }
];
