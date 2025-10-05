import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DialogModule } from 'primeng/dialog';
import { MenuItem } from 'primeng/api';
import { ContactService, ContactListItem, ContactFilters } from '../../../services/contact.service';
import { SupervisedEntityService, SupervisedEntityListItem } from '../../../services/supervised-entity.service';

@Component({
  selector: 'app-contacts-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    BreadcrumbModule,
    DialogModule
  ],
  templateUrl: './contacts-list.component.html',
  styleUrls: ['./contacts-list.component.css']
})
export class ContactsListComponent implements OnInit {
  private contactService = inject(ContactService);
  private entityService = inject(SupervisedEntityService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Adresaci' }
  ];
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  contacts: ContactListItem[] = [];
  loading = false;
  totalRecords = 0;
  
  // Pagination
  page = 1;
  pageSize = 20;
  first = 0;
  pageSizeOptions = [10, 20, 50, 100];

  // Filters
  showFilters = true;
  filters: ContactFilters = {
    searchTerm: '',
    supervisedEntityId: undefined,
    isActive: undefined,
    isPrimary: undefined
  };

  // Entity options for filter
  entityOptions: { label: string; value: number }[] = [];
  
  // Active status options
  activeStatusOptions = [
    { label: 'Wszystkie', value: undefined },
    { label: 'Aktywne', value: true },
    { label: 'Nieaktywne', value: false }
  ];

  // Primary contact options
  primaryOptions = [
    { label: 'Wszystkie', value: undefined },
    { label: 'Główne', value: true },
    { label: 'Dodatkowe', value: false }
  ];

  // Delete confirmation dialog
  showDeleteDialog = false;
  contactToDelete: ContactListItem | null = null;

  ngOnInit(): void {
    this.loadEntities();
    this.loadContacts();
  }

  loadEntities(): void {
    this.entityService.getEntities(1, 1000).subscribe({
      next: (response) => {
        this.entityOptions = [
          { label: 'Wszystkie podmioty', value: undefined as any },
          ...response.data.map(entity => ({
            label: entity.name,
            value: entity.id
          }))
        ];
      },
      error: (error) => {
        console.error('Error loading entities:', error);
      }
    });
  }

  loadContacts(): void {
    this.loading = true;
    console.log('Loading contacts with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });
    
    this.contactService.getContacts(
      this.page,
      this.pageSize,
      this.filters
    ).subscribe({
      next: (response) => {
        console.log('Contacts loaded successfully:', response);
        this.contacts = response.data;
        this.totalRecords = response.pagination.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading contacts:', error);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.first = event.first;
    this.loadContacts();
  }

  applyFilters(): void {
    this.page = 1;
    this.first = 0;
    this.loadContacts();
  }

  clearFilters(): void {
    this.filters = {
      searchTerm: '',
      supervisedEntityId: undefined,
      isActive: undefined,
      isPrimary: undefined
    };
    this.page = 1;
    this.first = 0;
    this.loadContacts();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  viewContactDetails(contactId: number): void {
    this.router.navigate(['/contacts', contactId]);
  }

  createContact(): void {
    this.router.navigate(['/contacts/create']);
  }

  editContact(contactId: number): void {
    this.router.navigate(['/contacts', contactId]);
  }

  confirmDelete(contact: ContactListItem): void {
    this.contactToDelete = contact;
    this.showDeleteDialog = true;
  }

  deleteContact(): void {
    if (this.contactToDelete) {
      this.contactService.deleteContact(this.contactToDelete.id).subscribe({
        next: () => {
          console.log('Contact deleted successfully');
          this.showDeleteDialog = false;
          this.contactToDelete = null;
          this.loadContacts();
        },
        error: (error) => {
          console.error('Error deleting contact:', error);
          this.showDeleteDialog = false;
        }
      });
    }
  }

  cancelDelete(): void {
    this.showDeleteDialog = false;
    this.contactToDelete = null;
  }

  manageGroups(): void {
    this.router.navigate(['/contacts/groups']);
  }
}
