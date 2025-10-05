import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { SupervisedEntityService, SupervisedEntityListItem, EntityFilters } from '../../../services/supervised-entity.service';

@Component({
  selector: 'app-entities-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    BreadcrumbModule
  ],
  templateUrl: './entities-list.component.html',
  styleUrls: ['./entities-list.component.css']
})
export class EntitiesListComponent implements OnInit {
  private entityService = inject(SupervisedEntityService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Pulpit użytkownika', routerLink: '/dashboard' },
    { label: 'Kartoteka Podmiotów' }
  ];
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  entities: SupervisedEntityListItem[] = [];
  loading = false;
  totalRecords = 0;
  
  // Pagination
  page = 1;
  pageSize = 20;
  first = 0;
  pageSizeOptions = [10, 20, 50, 100];

  // Filters
  showFilters = true;
  filters: EntityFilters = {
    searchTerm: '',
    entityType: undefined,
    isActive: undefined
  };

  // Entity type options
  entityTypeOptions = [
    { label: 'Wszystkie', value: undefined },
    { label: 'Bank', value: 'Bank' },
    { label: 'Ubezpieczenia', value: 'Insurance' },
    { label: 'Fundusz Inwestycyjny', value: 'InvestmentFund' },
    { label: 'Fundusz Emerytalny', value: 'PensionFund' },
    { label: 'Dom Maklerski', value: 'BrokerageHouse' },
    { label: 'SKOK', value: 'CreditUnion' }
  ];

  // Active status options
  activeStatusOptions = [
    { label: 'Wszystkie', value: undefined },
    { label: 'Aktywne', value: true },
    { label: 'Nieaktywne', value: false }
  ];

  ngOnInit(): void {
    this.loadEntities();
  }

  loadEntities(): void {
    this.loading = true;
    console.log('Loading entities with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });
    
    this.entityService.getEntities(
      this.page,
      this.pageSize,
      this.filters
    ).subscribe({
      next: (response) => {
        console.log('Entities loaded successfully:', response);
        this.entities = response.data;
        this.totalRecords = response.pagination.totalCount;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading entities:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          error: error.error
        });
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.first = event.first;
    this.loadEntities();
  }

  applyFilters(): void {
    this.page = 1;
    this.first = 0;
    this.loadEntities();
  }

  clearFilters(): void {
    this.filters = {
      searchTerm: '',
      entityType: undefined,
      isActive: undefined
    };
    this.page = 1;
    this.first = 0;
    this.loadEntities();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  viewEntityDetails(entityId: number): void {
    this.router.navigate(['/entities', entityId]);
  }

  createEntity(): void {
    this.router.navigate(['/entities/create']);
  }

  getEntityTypeLabel(entityType: string): string {
    const option = this.entityTypeOptions.find(opt => opt.value === entityType);
    return option ? option.label : entityType;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pl-PL', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
  }
}
