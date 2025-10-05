import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

// File/Document interfaces
export interface LibraryFile {
  id: number;
  fileName: string;
  fileSize: number;
  fileType: string;
  uploadDate: string;
  updateDate: string;
  reportingPeriod: string;
  isArchived: boolean;
  uploadedBy: string;
  entityName?: string;
  entityId?: number;
  tags?: string[];
  description?: string;
  version?: string;
  // Polish UI fields
  dataAktualizacji?: string;
  nazwaPilku?: string;
  okresSprawozdawczy?: string;
}

export interface LibraryPagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface LibraryResponse {
  data: LibraryFile[];
  pagination: LibraryPagination;
}

export interface LibraryFilters {
  fileName?: string;
  reportingPeriod?: string;
  uploadDateFrom?: string;
  uploadDateTo?: string;
  entityName?: string;
  isArchived?: boolean;
  fileType?: string;
}

@Injectable({
  providedIn: 'root'
})
export class LibraryService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/library';

  getFiles(
    page: number = 1,
    pageSize: number = 10,
    filters?: LibraryFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<LibraryResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.fileName) {
        params = params.set('fileName', filters.fileName);
      }
      if (filters.reportingPeriod) {
        params = params.set('reportingPeriod', filters.reportingPeriod);
      }
      if (filters.uploadDateFrom) {
        params = params.set('uploadDateFrom', filters.uploadDateFrom);
      }
      if (filters.uploadDateTo) {
        params = params.set('uploadDateTo', filters.uploadDateTo);
      }
      if (filters.entityName) {
        params = params.set('entityName', filters.entityName);
      }
      if (filters.isArchived !== undefined) {
        params = params.set('isArchived', filters.isArchived.toString());
      }
      if (filters.fileType) {
        params = params.set('fileType', filters.fileType);
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
    }
    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }

    return this.http.get<LibraryResponse>(this.apiUrl, { params });
  }

  getFileById(id: number): Observable<LibraryFile> {
    return this.http.get<LibraryFile>(`${this.apiUrl}/${id}`);
  }

  uploadFile(file: File, metadata: any): Observable<LibraryFile> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('metadata', JSON.stringify(metadata));
    return this.http.post<LibraryFile>(`${this.apiUrl}/upload`, formData);
  }

  updateFile(id: number, updates: Partial<LibraryFile>): Observable<LibraryFile> {
    return this.http.put<LibraryFile>(`${this.apiUrl}/${id}`, updates);
  }

  deleteFile(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  downloadFile(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/download`, {
      responseType: 'blob'
    });
  }
}
