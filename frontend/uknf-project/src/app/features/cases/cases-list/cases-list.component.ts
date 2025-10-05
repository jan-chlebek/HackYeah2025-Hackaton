import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TagModule } from 'primeng/tag';
import { MenuItem } from 'primeng/api';
import { CaseService, CaseListItem, CaseFilters, CaseStatus, CaseType } from '../../../services/case.service';

@Component({
  selector: 'app-cases-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    BreadcrumbModule,
    TagModule
  ],
  templateUrl: './cases-list.component.html',
  styleUrls: ['./cases-list.component.css']
})
export class CasesListComponent implements OnInit {
  private caseService = inject(CaseService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Sprawy' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  cases: CaseListItem[] = [];
  totalRecords = 0;
  loading = false;
  
  // Pagination
  first = 0;
  pageSize = 10;
  pageSizeOptions = [10, 25, 50, 100];

  // Filters
  showFilters = false;
  filters: CaseFilters = {};
  searchTerm = '';

  // Filter options
  statusOptions = [
    { label: 'Wszystkie', value: null },
    { label: 'Roboczy', value: CaseStatus.Working },
    { label: 'Nowy', value: CaseStatus.New },
    { label: 'W trakcie', value: CaseStatus.InProgress },
    { label: 'Oczekuje na odpowiedź', value: CaseStatus.AwaitingUserResponse },
    { label: 'Zaakceptowany', value: CaseStatus.Accepted },
    { label: 'Zablokowany', value: CaseStatus.Blocked },
    { label: 'Rozwiązany', value: CaseStatus.Resolved },
    { label: 'Zamknięty', value: CaseStatus.Closed }
  ];

  typeOptions = [
    { label: 'Wszystkie', value: null },
    { label: 'Wniosek o dostęp', value: CaseType.AccessRequest },
    { label: 'Postępowanie', value: CaseType.Investigation },
    { label: 'Audyt', value: CaseType.Audit },
    { label: 'Zgodność', value: CaseType.Compliance },
    { label: 'Ogólny', value: CaseType.General }
  ];

  ngOnInit(): void {
    setTimeout(() => {
      this.loadCases();
    }, 0);
  }

  loadCases(): void {
    this.loading = true;
    const page = Math.floor(this.first / this.pageSize) + 1;
    console.log('Loading cases with params:', { page, pageSize: this.pageSize, first: this.first, filters: this.filters });
    
    this.caseService.getCases(page, this.pageSize, this.filters).subscribe({
      next: (response) => {
        console.log('Cases loaded successfully:', response);
        this.cases = response.items || [];
        this.totalRecords = response.totalItems || 0;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading cases:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        this.cases = [];
        this.totalRecords = 0;
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onPageChange(event: any): void {
    console.log('Page change event:', event);
    this.first = event.first;
    this.pageSize = event.rows;
    this.loadCases();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  applyFilters(): void {
    if (this.searchTerm) {
      this.filters.searchTerm = this.searchTerm;
    } else {
      delete this.filters.searchTerm;
    }
    
    this.first = 0;
    this.loadCases();
  }

  clearFilters(): void {
    this.filters = {};
    this.searchTerm = '';
    this.first = 0;
    this.loadCases();
  }

  viewCaseDetails(caseItem: CaseListItem): void {
    this.router.navigate(['/cases', caseItem.id]);
  }

  createCase(): void {
    this.router.navigate(['/cases/create']);
  }

  formatDate(date: string | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('pl-PL', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusSeverity(status: CaseStatus): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case CaseStatus.Accepted:
        return 'success';
      case CaseStatus.New:
      case CaseStatus.InProgress:
        return 'info';
      case CaseStatus.AwaitingUserResponse:
      case CaseStatus.Updated:
        return 'warn';
      case CaseStatus.Blocked:
        return 'danger';
      case CaseStatus.Working:
      case CaseStatus.Resolved:
      case CaseStatus.Closed:
      default:
        return 'secondary';
    }
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 1: return 'Bardzo wysoki';
      case 2: return 'Wysoki';
      case 3: return 'Średni';
      case 4: return 'Niski';
      case 5: return 'Bardzo niski';
      default: return 'Nieznany';
    }
  }

  getPrioritySeverity(priority: number): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (priority) {
      case 1: return 'danger';
      case 2: return 'warn';
      case 3: return 'info';
      case 4:
      case 5:
      default: return 'secondary';
    }
  }
}
