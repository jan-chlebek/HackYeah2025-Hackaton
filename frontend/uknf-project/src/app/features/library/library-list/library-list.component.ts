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
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [
    { label: 'Pulpit użytkownika', routerLink: '/dashboard' },
    { label: 'Wnioski o dostęp', routerLink: '/wnioski' },
    { label: 'Biblioteka - repozytorium plików' }
  ];

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
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      this.generateMockData();
      this.loadFiles();
    }, 0);
  }

  loadFiles(): void {
    this.loading = true;
    console.log('Loading files with params:', { page: this.page, pageSize: this.pageSize, filters: this.filters });

    // TODO: Replace with actual API call
    // this.libraryService.getFiles(this.page, this.pageSize, this.filters).subscribe({
    //   next: (response) => {
    //     this.files = response.data || [];
    //     this.totalRecords = response.pagination?.totalCount || 0;
    //     this.loading = false;
    //   },
    //   error: (error) => {
    //     console.error('Error loading files:', error);
    //     this.files = [];
    //     this.totalRecords = 0;
    //     this.loading = false;
    //   }
    // });

    // Mock data for now
    setTimeout(() => {
      this.loading = false;
      this.cdr.markForCheck();
    }, 500);
  }

  generateMockData(): void {
    // Generate mock data matching the screenshot
    this.files = [
      {
        id: 1,
        fileName: 'Plik_w_repozytorium_01.xlsx',
        fileSize: 1024000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Kwartal',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_01.xlsx',
        okresSprawozdawczy: 'Kwartal'
      },
      {
        id: 2,
        fileName: 'Plik_w_repozytorium_02.xlsx',
        fileSize: 2048000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: '',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_02.xlsx'
      },
      {
        id: 3,
        fileName: 'Plik_w_repozytorium_03.xlsx',
        fileSize: 1536000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: '',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_03.xlsx'
      },
      {
        id: 4,
        fileName: 'Plik_w_repozytorium_04.xlsx',
        fileSize: 3072000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Rok',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_04.xlsx',
        okresSprawozdawczy: 'Rok'
      },
      {
        id: 5,
        fileName: 'Plik_w_repozytorium_05.xlsx',
        fileSize: 2560000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Kwartal',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_05.xlsx',
        okresSprawozdawczy: 'Kwartal'
      },
      {
        id: 6,
        fileName: 'Plik_w_repozytorium_06.xlsx',
        fileSize: 1792000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Kwartal',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_06.xlsx',
        okresSprawozdawczy: 'Kwartal'
      },
      {
        id: 7,
        fileName: 'Plik_w_repozytorium_07.xlsx',
        fileSize: 2304000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Kwartal',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_07.xlsx',
        okresSprawozdawczy: 'Kwartal'
      },
      {
        id: 8,
        fileName: 'Plik_w_repozytorium_08.xlsx',
        fileSize: 1280000,
        fileType: 'xlsx',
        uploadDate: '2024-09-14',
        updateDate: '2024-09-14',
        reportingPeriod: 'Rok',
        isArchived: false,
        uploadedBy: 'Jan Kowalski',
        dataAktualizacji: '2024-09-14',
        nazwaPilku: 'Plik_w_repozytorium_08.xlsx',
        okresSprawozdawczy: 'Rok'
      }
    ];
    this.totalRecords = 200; // Mock total
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
    // TODO: Implement actual download
  }

  viewFileDetails(file: LibraryFile): void {
    this.router.navigate(['/biblioteka', file.id]);
  }

  deleteFile(file: LibraryFile): void {
    if (confirm(`Czy na pewno chcesz usunąć plik "${file.fileName}"?`)) {
      console.log('Deleting file:', file);
      // TODO: Implement actual delete
    }
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
