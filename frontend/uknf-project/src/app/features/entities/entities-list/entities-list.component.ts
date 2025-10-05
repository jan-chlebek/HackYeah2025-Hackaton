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
    { label: 'Kartoteka Podmiotów' }
  ];
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  entities: SupervisedEntityListItem[] = [];
  loading = false;
  totalRecords = 0;
  error = false;
  errorMessage = '';
  
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
    this.error = false;
    this.errorMessage = '';
    
    console.log('[EntitiesList] Loading entities with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });
    console.log('[EntitiesList] API URL:', `http://localhost:5000/api/v1/entities`);
    
    this.entityService.getEntities(
      this.page,
      this.pageSize,
      this.filters
    ).subscribe({
      next: (response) => {
        console.log('[EntitiesList] Entities loaded successfully:', response);
        this.entities = response.data || [];
        this.totalRecords = response.pagination?.totalCount || 0;
        this.loading = false;
        this.error = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('[EntitiesList] Error loading entities:', error);
        console.error('[EntitiesList] Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          error: error.error,
          url: error.url
        });
        
        // Set user-friendly error message
        if (error.status === 401) {
          this.errorMessage = 'Nie masz uprawnień do przeglądania kartoteki podmiotów. Zaloguj się ponownie.';
        } else if (error.status === 403) {
          this.errorMessage = 'Dostęp do kartoteki podmiotów jest zabroniony. Skontaktuj się z administratorem.';
        } else if (error.status === 0) {
          this.errorMessage = 'Brak połączenia z serwerem. Sprawdź, czy backend działa na porcie 5000.';
        } else {
          this.errorMessage = `Błąd serwera (${error.status}): ${error.statusText || 'Nieznany błąd'}`;
        }
        
        this.entities = [];
        this.totalRecords = 0;
        this.loading = false;
        this.error = true;
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
