import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

// File/Document interfaces
export interface LibraryFile {
  id: number;
  name: string;
  description?: string;
  fileName: string;
  fileSize: number;
  fileSizeFormatted?: string;
  contentType: string;
  category?: string;
  uploadedAt: string;
  uploadedByUserId: number;
  uploadedByName?: string;
  uploadedByEmail?: string;
  permissionCount?: number;
  // Legacy/UI fields for compatibility
  fileType?: string;
  uploadDate?: string;
  updateDate?: string;
  reportingPeriod?: string;
  isArchived?: boolean;
  uploadedBy?: string;
  entityName?: string;
  entityId?: number;
  tags?: string[];
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
  private apiUrl = 'http://localhost:5000/api/v1/library/files';

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
        params = params.set('search', filters.fileName);
      }
      if (filters.reportingPeriod) {
        params = params.set('category', filters.reportingPeriod);
      }
    }

    return this.http.get<LibraryFile[]>(this.apiUrl, { params, observe: 'response' }).pipe(
      map(response => {
        const paginationHeader = response.headers.get('X-Pagination');
        let pagination: LibraryPagination = {
          page,
          pageSize,
          totalCount: 0,
          totalPages: 0
        };

        if (paginationHeader) {
          try {
            const paginationData = JSON.parse(paginationHeader);
            pagination = {
              page: paginationData.currentPage || page,
              pageSize: paginationData.pageSize || pageSize,
              totalCount: paginationData.totalCount || 0,
              totalPages: paginationData.totalPages || 0
            };
          } catch (e) {
            console.error('Error parsing pagination header:', e);
          }
        }

        return {
          data: response.body || [],
          pagination
        };
      })
    );
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
