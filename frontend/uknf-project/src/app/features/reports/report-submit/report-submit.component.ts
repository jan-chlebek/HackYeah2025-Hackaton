import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

// PrimeNG Imports
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ProgressBarModule } from 'primeng/progressbar';
import { MessageModule } from 'primeng/message';

// Services
import { ReportService, ReportSubmission } from '../../../services/report.service';

@Component({
  selector: 'app-report-submit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    SelectModule,
    CheckboxModule,
    TextareaModule,
    BreadcrumbModule,
    ProgressBarModule,
    MessageModule
  ],
  templateUrl: './report-submit.component.html',
  styleUrls: ['./report-submit.component.css']
})
export class ReportSubmitComponent implements OnInit {
  private reportService = inject(ReportService);
  public router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems = [
    { label: 'Strona główna', routerLink: '/dashboard' },
    { label: 'Moduł komunikacyjny' },
    { label: 'Sprawozdawczość', routerLink: '/reports' },
    { label: 'Dodaj sprawozdanie' }
  ];

  // Form data
  selectedFile: File | null = null;
  selectedEntity: number | null = null;
  selectedPeriod: string = '';
  isCorrection = false;
  remarks = '';

  // UI state
  uploading = false;
  uploadProgress = 0;
  validationError = '';
  uploadSuccess = false;
  uploadedReportId: number | null = null;

  // Options
  entityOptions = [
    { label: 'Bank Pekao S.A.', value: 1 },
    { label: 'PKO BP', value: 2 },
    { label: 'ING Bank Śląski', value: 3 },
    { label: 'mBank', value: 4 },
    { label: 'Santander Bank Polska', value: 5 }
  ];

  reportingPeriodOptions = [
    { label: 'Q1 2025', value: 'Q1_2025' },
    { label: 'Q2 2025', value: 'Q2_2025' },
    { label: 'Q3 2025', value: 'Q3_2025' },
    { label: 'Q4 2025', value: 'Q4_2025' },
    { label: 'Q1 2024', value: 'Q1_2024' },
    { label: 'Q2 2024', value: 'Q2_2024' },
    { label: 'Q3 2024', value: 'Q3_2024' },
    { label: 'Q4 2024', value: 'Q4_2024' }
  ];

  ngOnInit(): void {
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      // Initialize component
      this.cdr.markForCheck();
    }, 0);
  }

  onFileSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    this.validationError = '';

    // Validate file type
    const validExcelTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
      'application/vnd.ms-excel' // .xls
    ];

    const validExcelExtensions = ['.xlsx', '.xls'];
    const fileExtension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));

    if (!validExcelTypes.includes(file.type) && !validExcelExtensions.includes(fileExtension)) {
      this.validationError = 'Nieprawidłowy format pliku. Dozwolone są tylko pliki Excel (.xlsx, .xls)';
      this.selectedFile = null;
      input.value = '';
      return;
    }

    // Validate file size (max 50MB)
    const maxSizeBytes = 50 * 1024 * 1024; // 50MB
    if (file.size > maxSizeBytes) {
      this.validationError = 'Plik jest zbyt duży. Maksymalny rozmiar to 50MB';
      this.selectedFile = null;
      input.value = '';
      return;
    }

    this.selectedFile = file;
    console.log('File selected:', file.name, file.type, file.size);
  }

  removeFile(): void {
    this.selectedFile = null;
    this.validationError = '';
    // Clear file input
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  triggerFileInput(): void {
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }

  canSubmit(): boolean {
    return !!this.selectedFile && 
           !!this.selectedEntity && 
           !!this.selectedPeriod && 
           !this.uploading &&
           !this.validationError;
  }

  submitReport(): void {
    if (!this.canSubmit() || !this.selectedFile || !this.selectedEntity || !this.selectedPeriod) {
      return;
    }

    this.uploading = true;
    this.uploadProgress = 0;
    this.validationError = '';

    const submission: ReportSubmission = {
      plik: this.selectedFile,
      okresSprawozdawczy: this.selectedPeriod,
      podmiotId: this.selectedEntity,
      czyKorekta: this.isCorrection,
      uwagi: this.remarks || undefined
    };

    // Simulate upload progress
    const progressInterval = setInterval(() => {
      if (this.uploadProgress < 90) {
        this.uploadProgress += 10;
        this.cdr.markForCheck();
      }
    }, 200);

    try {
      this.reportService.uploadReport(submission).subscribe({
        next: (report) => {
          clearInterval(progressInterval);
          this.uploadProgress = 100;
          this.uploading = false;
          this.uploadSuccess = true;
          this.uploadedReportId = report.id;
          console.log('Report uploaded successfully:', report);
          this.cdr.markForCheck();

          // Redirect to report details after 2 seconds
          setTimeout(() => {
            this.router.navigate(['/reports', report.id]);
          }, 2000);
        },
        error: (error) => {
          clearInterval(progressInterval);
          this.uploadProgress = 0;
          this.uploading = false;
          this.validationError = error.message || 'Błąd podczas przesyłania sprawozdania';
          console.error('Error uploading report:', error);
          this.cdr.markForCheck();
        }
      });
    } catch (error: any) {
      clearInterval(progressInterval);
      this.uploadProgress = 0;
      this.uploading = false;
      this.validationError = error.message || 'Błąd walidacji pliku';
      this.cdr.markForCheck();
    }
  }

  cancel(): void {
    this.router.navigate(['/reports']);
  }

  resetForm(): void {
    this.selectedFile = null;
    this.selectedEntity = null;
    this.selectedPeriod = '';
    this.isCorrection = false;
    this.remarks = '';
    this.validationError = '';
    this.uploadSuccess = false;
    this.uploadedReportId = null;
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }
}
