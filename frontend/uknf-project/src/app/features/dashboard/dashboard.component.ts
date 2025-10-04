import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Communication {
  id: number;
  publicationDate: string;
  subject: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  // Expose Math for template
  Math = Math;

  // Breadcrumb
  systemName = 'System';
  entityName = 'Instytucja Testowa';

  // Active tab
  activeTab: 'pulpit' | 'wnioski' | 'biblioteka' = 'pulpit';

  // Search/Filter
  searchExpanded = false;

  // Communications data
  communications: Communication[] = [];
  filteredCommunications: Communication[] = [];

  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 200;
  totalPages = 0;
  pageNumbers: number[] = [];
  
  // Sort
  sortColumn: 'publicationDate' | 'subject' = 'publicationDate';
  sortDirection: 'asc' | 'desc' = 'desc';

  ngOnInit() {
    this.generateMockData();
    this.updatePagination();
  }

  generateMockData() {
    // Generate 200 mock communications
    const subjects = [
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.',
      'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.'
    ];

    for (let i = 1; i <= 200; i++) {
      const date = new Date(2025, 1 + (i % 2), 14 + (i % 7));
      this.communications.push({
        id: i,
        publicationDate: date.toISOString().split('T')[0],
        subject: subjects[i % 2]
      });
    }

    this.sortData();
    this.updateFilteredData();
  }

  setActiveTab(tab: 'pulpit' | 'wnioski' | 'biblioteka') {
    this.activeTab = tab;
  }

  toggleSearch() {
    this.searchExpanded = !this.searchExpanded;
  }

  sortData() {
    this.communications.sort((a, b) => {
      let comparison = 0;
      if (this.sortColumn === 'publicationDate') {
        comparison = a.publicationDate.localeCompare(b.publicationDate);
      } else {
        comparison = a.subject.localeCompare(b.subject);
      }
      return this.sortDirection === 'asc' ? comparison : -comparison;
    });
  }

  sortBy(column: 'publicationDate' | 'subject') {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.sortData();
    this.updateFilteredData();
  }

  updateFilteredData() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.filteredCommunications = this.communications.slice(startIndex, endIndex);
  }

  updatePagination() {
    this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
    this.pageNumbers = this.getPageNumbers();
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);
    
    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.pageNumbers = this.getPageNumbers();
      this.updateFilteredData();
    }
  }

  goToFirstPage() {
    this.goToPage(1);
  }

  goToLastPage() {
    this.goToPage(this.totalPages);
  }

  previousPage() {
    this.goToPage(this.currentPage - 1);
  }

  nextPage() {
    this.goToPage(this.currentPage + 1);
  }

  changeItemsPerPage(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.itemsPerPage = parseInt(select.value);
    this.currentPage = 1;
    this.updatePagination();
    this.updateFilteredData();
  }

  preview() {
    console.log('Preview clicked');
    // TODO: Implement preview functionality
  }

  export() {
    console.log('Export clicked');
    // TODO: Implement export functionality
  }
}
