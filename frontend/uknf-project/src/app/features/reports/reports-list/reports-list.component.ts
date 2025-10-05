import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

// PrimeNG Imports
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { BreadcrumbModule } from 'primeng/breadcrumb';

// Services
import { ReportService, Report, ReportFilters, ReportValidationStatus } from '../../../services/report.service';

@Component({
  selector: 'app-reports-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    SelectModule,
    DatePickerModule,
    InputTextModule,
    CheckboxModule,
    BreadcrumbModule
  ],
  templateUrl: './reports-list.component.html',
  styleUrls: ['./reports-list.component.css']
})
export class ReportsListComponent implements OnInit {
  private reportService = inject(ReportService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Breadcrumb
  breadcrumbItems = [
    { label: 'Sprawozdawczość' }
  ];

  // Table data
  reports: Report[] = [];
  loading = false;
  totalRecords = 0;
  first = 0;
  rows = 20;

  // Filters
  showFilters = true;
  filters: ReportFilters = {};
  searchQuery = '';
  
  // Filter options
  statusOptions: { label: string; value: ReportValidationStatus }[] = [
    { label: 'Robocze', value: 'Robocze' },
    { label: 'Przekazane', value: 'Przekazane' },
    { label: 'W trakcie', value: 'W trakcie' },
    { label: 'Zaakceptowane', value: 'Proces walidacji zakończony sukcesem' },
    { label: 'Błędy walidacji', value: 'Błędy z reguł walidacji' },
    { label: 'Błąd techniczny', value: 'Błąd techniczny w procesie walidacji' },
    { label: 'Przekroczono czas', value: 'Błąd - przekroczono czas' }
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

  // Mock entities for filter
  entityOptions = [
    { label: 'Bank Pekao S.A.', value: 1 },
    { label: 'PKO BP', value: 2 },
    { label: 'ING Bank Śląski', value: 3 },
    { label: 'mBank', value: 4 },
    { label: 'Santander Bank Polska', value: 5 }
  ];

  ngOnInit(): void {
    // Replace mock with real backend load; keep mock only as first paint placeholder
    this.generateMockData();
    setTimeout(() => {
      this.loadReports();
    }, 0);
  }

  loadReports(): void {
    this.loading = true;
    const page = Math.floor(this.first / this.rows) + 1;

    // Build a translated filters object for backend (quarter only)
    const backendFilters: ReportFilters = { ...this.filters };
    if (backendFilters.okresSprawozdawczy) {
      backendFilters.okresSprawozdawczy = backendFilters.okresSprawozdawczy.split('_')[0];
    }

    const previous = [...this.reports];
    this.reportService.getReports(page, this.rows, backendFilters).subscribe({
      next: (response) => {
        console.log('Reports loaded successfully:', response);
        // Map backend ReportDto -> Report shape if backend has only minimal fields
        if (response && Array.isArray(response as any)) {
          // Defensive: if backend returns raw array (fallback scenario)
          // @ts-ignore
          const arr = response as any[];
          this.reports = arr.map((r, idx) => ({
            id: r.id ?? r.Id ?? idx + 1,
            nazwaPliku: r.fileName ?? r.FileName ?? 'Plik.xlsx',
            numerSprawozdania: r.reportNumber ?? r.ReportNumber,
            podmiotNazwa: r.entityName ?? r.EntityName ?? '-',
            podmiotId: r.entityId ?? 0,
            okresSprawozdawczy: r.reportingPeriod ?? r.ReportingPeriod ?? '-',
            dataZlozenia: r.submittedAt ?? new Date().toISOString(),
            statusWalidacji: 'Robocze',
            uzytkownikNazwisko: undefined,
            uzytkownikImie: undefined,
            uzytkownikEmail: undefined,
            uzytkownikTelefon: undefined,
            czyKorekta: false,
            wielkoscPliku: 0
          }));
          this.totalRecords = this.reports.length;
        } else if ((response as any).data) {
          const resp: any = response;
          this.reports = (resp.data || []).map((r: any, idx: number) => ({
            id: r.id ?? r.Id ?? idx + 1,
            nazwaPliku: r.fileName ?? r.FileName ?? 'Plik.xlsx',
            numerSprawozdania: r.reportNumber ?? r.ReportNumber,
            podmiotNazwa: r.entityName ?? r.EntityName ?? '-',
            podmiotId: r.entityId ?? 0,
            okresSprawozdawczy: r.reportingPeriod ?? r.ReportingPeriod ?? '-',
            dataZlozenia: r.submittedAt ?? new Date().toISOString(),
            statusWalidacji: 'Robocze',
            uzytkownikNazwisko: undefined,
            uzytkownikImie: undefined,
            uzytkownikEmail: undefined,
            uzytkownikTelefon: undefined,
            czyKorekta: false,
            wielkoscPliku: 0
          }));
          this.totalRecords = resp.pagination?.totalCount || this.reports.length;
        }
        // Keep previous mock if backend returned empty quickly to avoid flicker
        if (this.reports.length === 0 && previous.length > 0) {
            this.reports = previous;
            this.totalRecords = previous.length;
        }
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading reports:', error);
        // Preserve previous data instead of clearing to prevent disappearance
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    this.loadReports();
  }

  applyFilters(): void {
    this.first = 0; // Reset to first page
    this.filters.szukaj = this.searchQuery || undefined;
    this.loadReports();
  }

  clearFilters(): void {
    this.filters = {};
    this.searchQuery = '';
    this.first = 0;
    this.loadReports();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  viewReportDetails(report: Report): void {
    this.router.navigate(['/reports', report.id]);
  }

  goToSubmitReport(): void {
    this.router.navigate(['/reports/submit']);
  }

  downloadValidationReport(report: Report): void {
    if (!report.raportWalidacjiUrl) {
      console.warn('No validation report available for this report');
      return;
    }

    this.reportService.downloadValidationReport(report.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Raport_walidacji_${report.numerSprawozdania || report.id}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading validation report:', error);
      }
    });
  }

  getStatusBadgeClass(status: ReportValidationStatus): string {
    return this.reportService.getStatusBadgeConfig(status).class;
  }

  getStatusBadgeLabel(status: ReportValidationStatus): string {
    return this.reportService.getStatusBadgeConfig(status).label;
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }

  formatDate(dateString: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('pl-PL', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  // Mock data generator
  generateMockData(): void {
    const statuses: ReportValidationStatus[] = [
      'Robocze',
      'Przekazane',
      'W trakcie',
      'Proces walidacji zakończony sukcesem',
      'Błędy z reguł walidacji',
      'Błąd techniczny w procesie walidacji',
      'Błąd - przekroczono czas'
    ];

    const entities = ['Bank Pekao S.A.', 'PKO BP', 'ING Bank Śląski', 'mBank', 'Santander Bank Polska'];
    const periods = ['Q1_2025', 'Q2_2025', 'Q3_2024', 'Q4_2024'];
    const surnames = ['Kowalski', 'Nowak', 'Wiśniewski', 'Wójcik', 'Kowalczyk'];
    const names = ['Jan', 'Anna', 'Piotr', 'Maria', 'Tomasz'];

    this.reports = Array.from({ length: 15 }, (_, i) => {
      const entityName = entities[i % entities.length];
      const period = periods[i % periods.length];
      const surname = surnames[i % surnames.length];
      const name = names[i % names.length];
      const status = statuses[i % statuses.length];
      const hasValidationReport = ['Proces walidacji zakończony sukcesem', 'Błędy z reguł walidacji'].includes(status);

      return {
        id: i + 1,
        nazwaPliku: `RIP${100000 + i}_${period}.xlsx`,
        numerSprawozdania: `RIP${100000 + i}`,
        podmiotNazwa: entityName,
        podmiotId: (i % entities.length) + 1,
        okresSprawozdawczy: period,
        dataZlozenia: new Date(2025, 9 - (i % 4), 15 - i, 10 + (i % 12), 30).toISOString(),
        statusWalidacji: status,
        uzytkownikNazwisko: surname,
        uzytkownikImie: name,
        uzytkownikEmail: `${name.toLowerCase()}.${surname.toLowerCase()}@${entityName.toLowerCase().replace(/\s+/g, '')}.pl`,
        uzytkownikTelefon: `+48 ${500 + i} ${100 + i} ${200 + i}`,
        czyKorekta: i % 5 === 0,
        wielkoscPliku: 150000 + (i * 25000),
        raportWalidacjiUrl: hasValidationReport ? `/api/v1/reports/${i + 1}/validation-report` : undefined,
        dataWalidacji: hasValidationReport ? new Date(2025, 9 - (i % 4), 16 - i, 14 + (i % 8), 0).toISOString() : undefined,
        opisBledow: status === 'Błędy z reguł walidacji' ? 'Wykryto błędy w komórkach B12, C45, D78' : undefined
      };
    });

    this.totalRecords = this.reports.length;
    this.loading = false;
    this.cdr.markForCheck();
  }
}
