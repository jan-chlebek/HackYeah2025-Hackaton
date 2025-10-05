import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DialogModule } from 'primeng/dialog';
import { LibraryService, LibraryFile, LibraryFilters } from '../../../services/library.service';
import { AuthService } from '../../../services/auth.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-library-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    DatePickerModule,
    CheckboxModule,
    BreadcrumbModule,
    DialogModule
  ],
  templateUrl: './library-list.component.html',
  styleUrls: ['./library-list.component.css']
})
export class LibraryListComponent implements OnInit {
  private libraryService = inject(LibraryService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [];

  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  // Table data
  files: LibraryFile[] = [];
  totalRecords = 0;
  loading = false;

  // Pagination
  page = 1;
  pageSize = 10;
  pageSizeOptions = [10, 25, 50, 100];

  // Filters
  showFilters = false;
  filters: LibraryFilters = {};

  // Selected file for upload/edit modal
  selectedFile: LibraryFile | null = null;
  showAddFileDialog = false;

  // File upload
  uploadFileName: string = '';
  uploadFileObject: File | null = null;
  uploadReportingPeriod: string = '';
  uploadReportingPeriodDate: Date | null = null;
  uploadIsArchived: boolean = false;
  
  // Entity recipients
  entityRecipientOptions = [
    { label: 'Nazwa podmiotu', value: 'nazwa_podmiotu' },
    { label: 'Adresaci grupa pierwsza', value: 'grupa_pierwsza' },
    { label: 'Adresaci grupa druga', value: 'grupa_druga' },
    { label: 'Adresaci grupa trzecia', value: 'grupa_trzecia' }
  ];
  selectedRecipient: string = '';

  ngOnInit(): void {
    // Build breadcrumb based on permissions
    const items: MenuItem[] = [];
    
    if (this.authService.hasElevatedPermissions()) {
      items.push({ label: 'Wnioski o dostęp', routerLink: '/wnioski' });
    }
    
    items.push({ label: 'Biblioteka - repozytorium plików' });
    this.breadcrumbItems = items;
    
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      this.loadFiles();
    }, 0);
  }

  loadFiles(): void {
    this.loading = true;
    console.log('Loading files with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });

    this.libraryService.getFiles(this.page, this.pageSize, this.filters).subscribe({
      next: (response) => {
        // Map API response to component format
        this.files = (response.data || []).map(file => ({
          ...file,
          dataAktualizacji: file.uploadedAt,
          nazwaPilku: file.fileName,
          okresSprawozdawczy: file.category || '',
          uploadedBy: file.uploadedByName || file.uploadedByEmail || ''
        }));
        this.totalRecords = response.pagination?.totalCount || 0;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading files:', error);
        this.files = [];
        this.totalRecords = 0;
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }



  onPageChange(event: any): void {
    this.page = event.page + 1;
    this.pageSize = event.rows;
    this.loadFiles();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  applyFilters(): void {
    this.page = 1;
    this.loadFiles();
  }

  clearFilters(): void {
    this.filters = {};
    this.page = 1;
    this.loadFiles();
  }

  openAddFileDialog(): void {
    this.selectedFile = null;
    this.uploadFileName = '';
    this.uploadFileObject = null;
    this.uploadReportingPeriod = '';
    this.uploadReportingPeriodDate = null;
    this.uploadIsArchived = false;
    this.selectedRecipient = '';
    this.showAddFileDialog = true;
  }

  closeAddFileDialog(): void {
    this.showAddFileDialog = false;
    this.selectedFile = null;
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.uploadFileObject = file;
      this.uploadFileName = file.name;
    }
  }

  uploadFile(): void {
    if (this.uploadFileObject) {
      console.log('Uploading file:', {
        file: this.uploadFileObject,
        fileName: this.uploadFileName,
        reportingPeriod: this.uploadReportingPeriod,
        isArchived: this.uploadIsArchived,
        recipient: this.selectedRecipient
      });
      
      // TODO: Implement actual upload
      // const metadata = {
      //   fileName: this.uploadFileName,
      //   reportingPeriod: this.uploadReportingPeriod,
      //   isArchived: this.uploadIsArchived,
      //   recipients: this.selectedRecipients
      // };
      // this.libraryService.uploadFile(this.uploadFileObject, metadata).subscribe({
      //   next: (file) => {
      //     console.log('File uploaded:', file);
      //     this.loadFiles();
      //     this.closeAddFileDialog();
      //   },
      //   error: (error) => {
      //     console.error('Error uploading file:', error);
      //   }
      // });
      
      this.closeAddFileDialog();
    }
  }

  downloadFile(file: LibraryFile): void {
    console.log('Downloading file:', file);
    this.libraryService.downloadFile(file.id).subscribe({
      next: (blob) => {
        // Create a temporary URL for the blob
        const url = window.URL.createObjectURL(blob);
        
        // Create a temporary anchor element to trigger download
        const a = document.createElement('a');
        a.href = url;
        a.download = file.fileName || `file_${file.id}`;
        document.body.appendChild(a);
        a.click();
        
        // Clean up
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading file:', error);
        alert('Nie udało się pobrać pliku. Spróbuj ponownie.');
      }
    });
  }

  formatDate(date: string | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pl-PL');
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }
}
