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
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Pulpit użytkownika', routerLink: '/dashboard' },
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

  ngOnInit(): void {
    setTimeout(() => {
      this.loadAnnouncements();
    }, 0);
  }

  loadAnnouncements(): void {
    this.loading = true;
    const page = Math.floor(this.first / this.pageSize) + 1;
    console.log('Loading announcements with params:', { page, pageSize: this.pageSize, first: this.first, filters: this.filters });
    
    this.announcementService.getAnnouncements(page, this.pageSize, this.filters).subscribe({
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
