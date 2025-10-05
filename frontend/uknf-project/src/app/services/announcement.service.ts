import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

/**
 * Announcement list item (with content preview)
 */
export interface AnnouncementListItem {
  id: number;
  title: string;
  contentPreview: string;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  isReadByCurrentUser: boolean;
}

/**
 * Full announcement details
 */
export interface Announcement {
  id: number;
  title: string;
  content: string;
  createdByUserId: number;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  isReadByCurrentUser: boolean;
  readAt?: string;
}

/**
 * Create announcement request
 */
export interface CreateAnnouncementRequest {
  title: string;
  content: string;
}

/**
 * Update announcement request
 */
export interface UpdateAnnouncementRequest {
  title: string;
  content: string;
}

/**
 * Announcement filters
 */
export interface AnnouncementFilters {
  isRead?: boolean;
  searchTerm?: string;
  dateFrom?: string;
  dateTo?: string;
}

/**
 * Paginated response
 */
export interface PaginatedAnnouncementResponse {
  items: AnnouncementListItem[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/announcements`;

  /**
   * Get paginated announcements list with optional filters
   */
  getAnnouncements(
    page: number = 1,
    pageSize: number = 10,
    filters?: AnnouncementFilters
  ): Observable<PaginatedAnnouncementResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters?.isRead !== undefined) {
      params = params.set('isRead', filters.isRead.toString());
    }

    if (filters?.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    if (filters?.dateFrom) {
      params = params.set('dateFrom', filters.dateFrom);
    }

    if (filters?.dateTo) {
      params = params.set('dateTo', filters.dateTo);
    }

    return this.http.get<PaginatedAnnouncementResponse>(this.apiUrl, { params });
  }

  /**
   * Get announcement by ID
   */
  getAnnouncementById(id: number): Observable<Announcement> {
    return this.http.get<Announcement>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create new announcement (UKNF staff only)
   */
  createAnnouncement(request: CreateAnnouncementRequest): Observable<Announcement> {
    return this.http.post<Announcement>(this.apiUrl, request);
  }

  /**
   * Update announcement (UKNF staff only)
   */
  updateAnnouncement(id: number, request: UpdateAnnouncementRequest): Observable<Announcement> {
    return this.http.put<Announcement>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Delete announcement (UKNF staff only)
   */
  deleteAnnouncement(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Mark announcement as read
   */
  markAsRead(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/read`, {});
  }
}
