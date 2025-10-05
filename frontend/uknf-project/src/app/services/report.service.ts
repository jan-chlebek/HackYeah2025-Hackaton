import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface Report {
  id: number;
  nazwaPliku: string;
  numerSprawozdania?: string;
  podmiotNazwa: string;
  podmiotId: number;
  okresSprawozdawczy: string;
  dataZlozenia: string;
  statusWalidacji: ReportValidationStatus;
  uzytkownikNazwisko?: string;
  uzytkownikImie?: string;
  uzytkownikEmail?: string;
  uzytkownikTelefon?: string;
  czyKorekta: boolean;
  wielkoscPliku: number;
  raportWalidacjiUrl?: string;
  dataWalidacji?: string;
  opisBledow?: string;
}

export type ReportValidationStatus = 
  | 'Robocze' 
  | 'Przekazane' 
  | 'W trakcie' 
  | 'Proces walidacji zakończony sukcesem' 
  | 'Błędy z reguł walidacji' 
  | 'Błąd techniczny w procesie walidacji' 
  | 'Błąd - przekroczono czas';

export interface ReportFilters {
  statusWalidacji?: ReportValidationStatus;
  okresSprawozdawczy?: string;
  podmiotId?: number;
  czyKorekta?: boolean;
  dataZlozeniaOd?: string;
  dataZlozeniaDo?: string;
  szukaj?: string;
}

export interface ReportSubmission {
  plik: File;
  okresSprawozdawczy: string;
  podmiotId: number;
  czyKorekta: boolean;
  uwagi?: string;
}

export interface ReportStatusHistory {
  id: number;
  raportId: number;
  status: ReportValidationStatus;
  dataZmiany: string;
  uzytkownikId?: number;
  uzytkownikNazwisko?: string;
  uwagi?: string;
}

export interface ApiResponse<T> {
  data: T;
  pagination?: {
    currentPage: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };
}

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/reports`;

  /**
   * Get list of reports with optional filtering, sorting, and pagination
   */
  getReports(
    page: number = 1,
    pageSize: number = 20,
    filters?: ReportFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<ApiResponse<Report[]>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.statusWalidacji) {
        params = params.set('statusWalidacji', filters.statusWalidacji);
      }
      if (filters.okresSprawozdawczy) {
        params = params.set('okresSprawozdawczy', filters.okresSprawozdawczy);
      }
      if (filters.podmiotId) {
        params = params.set('podmiotId', filters.podmiotId.toString());
      }
      if (filters.czyKorekta !== undefined) {
        params = params.set('czyKorekta', filters.czyKorekta.toString());
      }
      if (filters.dataZlozeniaOd) {
        params = params.set('dataZlozeniaOd', filters.dataZlozeniaOd);
      }
      if (filters.dataZlozeniaDo) {
        params = params.set('dataZlozeniaDo', filters.dataZlozeniaDo);
      }
      if (filters.szukaj) {
        params = params.set('szukaj', filters.szukaj);
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
      params = params.set('sortOrder', sortOrder || 'asc');
    }

    console.log('ReportService.getReports called with params:', params.toString());

    return this.http.get<ApiResponse<Report[]>>(this.baseUrl, { params });
  }

  /**
   * Get details of a single report
   */
  getReportDetails(id: number): Observable<Report> {
    console.log('ReportService.getReportDetails called for id:', id);
    return this.http.get<Report>(`${this.baseUrl}/${id}`);
  }

  /**
   * Get status history for a report
   */
  getReportStatusHistory(reportId: number): Observable<ReportStatusHistory[]> {
    console.log('ReportService.getReportStatusHistory called for reportId:', reportId);
    return this.http.get<ReportStatusHistory[]>(`${this.baseUrl}/${reportId}/history`);
  }

  /**
   * Upload a new report with Excel file validation
   */
  uploadReport(submission: ReportSubmission): Observable<Report> {
    console.log('ReportService.uploadReport called with (original submission):', {
      fileName: submission.plik.name,
      fileSize: submission.plik.size,
      fileType: submission.plik.type,
      okresSprawozdawczy: submission.okresSprawozdawczy,
      podmiotId: submission.podmiotId,
      czyKorekta: submission.czyKorekta
    });

    // Validate Excel file (keep permissive like component)
    const validExcelTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/vnd.ms-excel'
    ];
    const validExcelExtensions = ['.xlsx', '.xls'];
    const fileExtension = submission.plik.name.toLowerCase().substring(submission.plik.name.lastIndexOf('.'));
    if (!validExcelTypes.includes(submission.plik.type) && !validExcelExtensions.includes(fileExtension)) {
      throw new Error('Nieprawidłowy format pliku. Dozwolone są tylko pliki Excel (.xlsx, .xls)');
    }

    // Backend expects form keys: File, ReportingPeriod (PascalCase) only.
    // Frontend currently stores periods like "Q1_2025"; backend only accepts Q1/Q2/Q3/Q4.
    const reportingPeriod = submission.okresSprawozdawczy.split('_')[0];

    const formData = new FormData();
    formData.append('File', submission.plik);
    formData.append('ReportingPeriod', reportingPeriod);

    // NOTE: Additional fields (entity, correction flag, remarks) are not yet supported
    // by backend SubmitReport endpoint. We intentionally do NOT send them to avoid 400.
    // TODO: Extend backend contract to include these when requirements are clarified.

    return this.http.post<any>(this.baseUrl, formData).pipe(
      map(apiResponse => {
        // apiResponse is expected to be ReportDto with PascalCase properties
        const mapped: Report = {
          id: apiResponse.id ?? apiResponse.Id ?? 0,
          nazwaPliku: apiResponse.fileName ?? apiResponse.FileName ?? submission.plik.name,
          numerSprawozdania: apiResponse.reportNumber ?? apiResponse.ReportNumber,
          podmiotNazwa: apiResponse.entityName ?? apiResponse.EntityName ?? '',
            // No entity id provided by backend yet – reuse submitted value (may be null)
          podmiotId: submission.podmiotId,
          okresSprawozdawczy: apiResponse.reportingPeriod ?? apiResponse.ReportingPeriod ?? reportingPeriod,
          dataZlozenia: new Date().toISOString(),
          statusWalidacji: 'Robocze',
          uzytkownikNazwisko: undefined,
          uzytkownikImie: undefined,
          uzytkownikEmail: undefined,
          uzytkownikTelefon: undefined,
          czyKorekta: submission.czyKorekta,
          wielkoscPliku: submission.plik.size,
          raportWalidacjiUrl: undefined,
          dataWalidacji: undefined,
          opisBledow: undefined
        };
        console.log('Mapped upload response to Report interface:', mapped);
        return mapped;
      })
    );
  }

  /**
   * Download validation report PDF
   */
  downloadValidationReport(reportId: number): Observable<Blob> {
    console.log('ReportService.downloadValidationReport called for reportId:', reportId);
    return this.http.get(`${this.baseUrl}/${reportId}/validation-report`, {
      responseType: 'blob'
    });
  }

  /**
   * Delete a report (if allowed)
   */
  deleteReport(id: number): Observable<void> {
    console.log('ReportService.deleteReport called for id:', id);
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Get validation status badge configuration
   */
  getStatusBadgeConfig(status: ReportValidationStatus): { class: string; label: string } {
    const statusConfig: Record<ReportValidationStatus, { class: string; label: string }> = {
      'Robocze': { class: 'status-draft', label: 'Robocze' },
      'Przekazane': { class: 'status-submitted', label: 'Przekazane' },
      'W trakcie': { class: 'status-processing', label: 'W trakcie' },
      'Proces walidacji zakończony sukcesem': { class: 'status-success', label: 'Zaakceptowane' },
      'Błędy z reguł walidacji': { class: 'status-validation-error', label: 'Błędy walidacji' },
      'Błąd techniczny w procesie walidacji': { class: 'status-technical-error', label: 'Błąd techniczny' },
      'Błąd - przekroczono czas': { class: 'status-timeout', label: 'Przekroczono czas' }
    };

    return statusConfig[status] || { class: 'status-unknown', label: status };
  }
}

