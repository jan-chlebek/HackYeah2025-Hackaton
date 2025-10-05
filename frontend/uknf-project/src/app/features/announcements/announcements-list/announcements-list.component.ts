import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TagModule } from 'primeng/tag';
import { MenuItem } from 'primeng/api';
import { AnnouncementService, AnnouncementListItem, AnnouncementFilters } from '../../../services/announcement.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-announcements-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    BreadcrumbModule,
    TagModule
  ],
  templateUrl: './announcements-list.component.html',
  styleUrls: ['./announcements-list.component.css']
})
export class AnnouncementsListComponent implements OnInit {
  private announcementService = inject(AnnouncementService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Komunikaty' }
  ];
  
  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  announcements: AnnouncementListItem[] = [];
  totalRecords = 0;
  loading = false;
  
  // Pagination
  first = 0; // First record index for PrimeNG
  pageSize = 10;
  pageSizeOptions = [10, 25, 50, 100];

  // Filters
  showFilters = false;
  filters: AnnouncementFilters = {};
  searchTerm = '';

  // Sorting
  sortField: string = 'createdAt';
  sortOrder: number = -1; // -1 for descending, 1 for ascending

  // Check if user is internal (UKNF staff) - NOT external user
  get isInternalUser(): boolean {
    const user = this.authService.getCurrentUser();
    console.log('[AnnouncementsList] Checking isInternalUser:', {
      user,
      roles: user?.roles,
      email: user?.email
    });
    
    if (!user || !user.roles || user.roles.length === 0) {
      console.log('[AnnouncementsList] No user or no roles found, returning false');
      return false;
    }
    
    // Check if user has ExternalUser role - if yes, they are NOT internal
    const hasExternalRole = user.roles.some((r: string) => r.toLowerCase() === 'externaluser');
    if (hasExternalRole) {
      console.log('[AnnouncementsList] User has ExternalUser role, returning false');
      return false;
    }
    
    // Check if user has internal roles
    const internalRoles = ['administrator', 'internaluser', 'supervisor'];
    const isInternal = user.roles.some((r: string) => internalRoles.includes(r.toLowerCase()));
    console.log('[AnnouncementsList] isInternalUser result:', isInternal);
    return isInternal;
  }

  ngOnInit(): void {
    setTimeout(() => {
      this.loadAnnouncements();
    }, 0);
  }

  loadAnnouncements(): void {
    this.loading = true;
    const page = Math.floor(this.first / this.pageSize) + 1;
    
    // Add sorting to filters
    const filtersWithSort = {
      ...this.filters,
      sortBy: this.sortField,
      sortDirection: this.sortOrder === 1 ? 'asc' as const : 'desc' as const
    };
    
    console.log('Loading announcements with params:', { page, pageSize: this.pageSize, first: this.first, filters: filtersWithSort });
    
    this.announcementService.getAnnouncements(page, this.pageSize, filtersWithSort).subscribe({
      next: (response) => {
        console.log('Announcements loaded successfully:', response);
        this.announcements = response.items || [];
        this.totalRecords = response.totalItems || 0;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading announcements:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        this.announcements = [];
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
    this.loadAnnouncements();
  }

  onSort(event: any): void {
    console.log('Sort event:', event);
    this.sortField = event.field;
    this.sortOrder = event.order;
    this.first = 0; // Reset to first page when sorting
    this.loadAnnouncements();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  applyFilters(): void {
    // Apply search term to filters
    if (this.searchTerm) {
      this.filters.searchTerm = this.searchTerm;
    } else {
      delete this.filters.searchTerm;
    }
    
    this.first = 0; // Reset to first page
    this.loadAnnouncements();
  }

  clearFilters(): void {
    this.filters = {};
    this.searchTerm = '';
    this.first = 0; // Reset to first page
    this.loadAnnouncements();
  }

  viewAnnouncementDetails(announcement: AnnouncementListItem): void {
    this.router.navigate(['/announcements', announcement.id]);
  }

  createAnnouncement(): void {
    this.router.navigate(['/announcements/create']);
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

  getReadStatusSeverity(isRead: boolean): 'success' | 'info' {
    return isRead ? 'success' : 'info';
  }

  getReadStatusLabel(isRead: boolean): string {
    return isRead ? 'Przeczytane' : 'Nieprzeczytane';
  }

  stripHtmlTags(html: string): string {
    if (!html) return '';
    const tmp = document.createElement('div');
    tmp.innerHTML = html;
    return tmp.textContent || tmp.innerText || '';
  }
}
