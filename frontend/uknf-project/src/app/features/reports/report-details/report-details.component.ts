import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

// PrimeNG Imports
import { ButtonModule } from 'primeng/button';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { TimelineModule } from 'primeng/timeline';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';

// Services
import { ReportService, Report, ReportStatusHistory, ReportValidationStatus } from '../../../services/report.service';

@Component({
  selector: 'app-report-details',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    BreadcrumbModule,
    TimelineModule,
    CardModule,
    TagModule
  ],
  templateUrl: './report-details.component.html',
  styleUrls: ['./report-details.component.css']
})
export class ReportDetailsComponent implements OnInit {
  private reportService = inject(ReportService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Data
  report: Report | null = null;
  statusHistory: ReportStatusHistory[] = [];
  loading = true;
  error = '';

  // Breadcrumb
  breadcrumbItems = [

    { label: 'Moduł komunikacyjny' },
    { label: 'Sprawozdawczość', routerLink: '/reports' },
    { label: 'Szczegóły sprawozdania' }
  ];

  ngOnInit(): void {
    // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError in zoneless mode
    setTimeout(() => {
      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.loadReportDetails(parseInt(id, 10));
      } else {
        this.error = 'Brak identyfikatora sprawozdania';
        this.loading = false;
      }
    }, 0);
  }

  loadReportDetails(id: number): void {
    this.loading = true;
    
    // Load report details
    this.reportService.getReportDetails(id).subscribe({
      next: (report) => {
        this.report = report;
        this.loading = false;
        this.cdr.markForCheck();
        
        // Load status history
        this.loadStatusHistory(id);
      },
      error: (error) => {
        console.error('Error loading report details:', error);
        this.error = 'Nie udało się załadować szczegółów sprawozdania';
        this.loading = false;
        this.cdr.markForCheck();
        
        // Generate mock data for demo
        this.generateMockReport(id);
      }
    });
  }

  loadStatusHistory(reportId: number): void {
    this.reportService.getReportStatusHistory(reportId).subscribe({
      next: (history) => {
        this.statusHistory = history;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Error loading status history:', error);
        // Generate mock history for demo
        this.generateMockHistory();
      }
    });
  }

  downloadValidationReport(): void {
    if (!this.report || !this.report.raportWalidacjiUrl) {
      return;
    }

    this.reportService.downloadValidationReport(this.report.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Raport_walidacji_${this.report?.numerSprawozdania || this.report?.id}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading validation report:', error);
      }
    });
  }

  goToCorrection(): void {
    this.router.navigate(['/reports', this.report?.id, 'corrections']);
  }

  goBack(): void {
    this.router.navigate(['/reports']);
  }

  getStatusBadgeClass(status: string): string {
    return this.reportService.getStatusBadgeConfig(status as any).class;
  }

  getStatusBadgeLabel(status: string): string {
    return this.reportService.getStatusBadgeConfig(status as any).label;
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
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  // Mock data generators for demo
  generateMockReport(id: number): void {
    this.report = {
      id: id,
      nazwaPliku: `RIP${100000 + id}_Q1_2025.xlsx`,
      numerSprawozdania: `RIP${100000 + id}`,
      podmiotNazwa: 'Bank Pekao S.A.',
      podmiotId: 1,
      okresSprawozdawczy: 'Q1_2025',
      dataZlozenia: new Date(2025, 9, 4, 10, 30).toISOString(),
      statusWalidacji: 'Błędy z reguł walidacji',
      uzytkownikNazwisko: 'Kowalski',
      uzytkownikImie: 'Jan',
      uzytkownikEmail: 'jan.kowalski@pekao.pl',
      uzytkownikTelefon: '+48 500 100 200',
      czyKorekta: false,
      wielkoscPliku: 245000,
      raportWalidacjiUrl: `/api/v1/reports/${id}/validation-report`,
      dataWalidacji: new Date(2025, 9, 4, 14, 15).toISOString(),
      opisBledow: 'Wykryto błędy w komórkach B12, C45, D78 - nieprawidłowy format danych'
    };
    
    this.loading = false;
    this.error = '';
    this.generateMockHistory();
    this.cdr.markForCheck();
  }

  generateMockHistory(): void {
    if (!this.report) return;

    this.statusHistory = [
      {
        id: 1,
        raportId: this.report.id,
        status: 'Robocze' as ReportValidationStatus,
        dataZmiany: new Date(2025, 9, 4, 10, 30).toISOString(),
        uzytkownikNazwisko: 'Kowalski',
        uwagi: 'Sprawozdanie zostało dodane do systemu'
      },
      {
        id: 2,
        raportId: this.report.id,
        status: 'Przekazane' as ReportValidationStatus,
        dataZmiany: new Date(2025, 9, 4, 10, 31).toISOString(),
        uzytkownikNazwisko: 'System',
        uwagi: 'Nadano unikalny identyfikator RIP' + (100000 + this.report.id)
      },
      {
        id: 3,
        raportId: this.report.id,
        status: 'W trakcie' as ReportValidationStatus,
        dataZmiany: new Date(2025, 9, 4, 10, 32).toISOString(),
        uzytkownikNazwisko: 'System',
        uwagi: 'Rozpoczęto proces walidacji'
      },
      {
        id: 4,
        raportId: this.report.id,
        status: 'Błędy z reguł walidacji' as ReportValidationStatus,
        dataZmiany: new Date(2025, 9, 4, 14, 15).toISOString(),
        uzytkownikNazwisko: 'System',
        uwagi: 'Wykryto błędy walidacji: komórki B12, C45, D78'
      }
    ].reverse(); // Show newest first

    this.cdr.markForCheck();
  }
}
