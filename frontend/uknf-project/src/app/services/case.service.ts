import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Case status enum matching backend
 */
export enum CaseStatus {
  Working = 'Working',           // Draft
  New = 'New',                   // Submitted
  InProgress = 'InProgress',     // Being processed
  AwaitingUserResponse = 'AwaitingUserResponse', // Waiting for user
  Accepted = 'Accepted',         // Approved
  Blocked = 'Blocked',           // Rejected
  Updated = 'Updated',           // Modified
  Resolved = 'Resolved',         // Completed
  Closed = 'Closed'              // Archived
}

/**
 * Case type enum
 */
export enum CaseType {
  AccessRequest = 'AccessRequest',
  Investigation = 'Investigation',
  Audit = 'Audit',
  Compliance = 'Compliance',
  General = 'General'
}

/**
 * Case list item (summary view)
 */
export interface CaseListItem {
  id: number;
  caseNumber: string;
  type: CaseType;
  title: string;
  description: string;
  category: string;
  status: CaseStatus;
  priority: number;
  supervisedEntityId: number;
  supervisedEntityName: string;
  handlerId?: number;
  handlerName?: string;
  createdByUserId: number;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  submittedAt?: string;
  statusPolish: string;
  typePolish: string;
}

/**
 * Full case details
 */
export interface Case {
  id: number;
  caseNumber: string;
  type: CaseType;
  title: string;
  description: string;
  category: string;
  status: CaseStatus;
  priority: number;
  supervisedEntityId: number;
  supervisedEntityName: string;
  
  // Applicant info (for Access Requests)
  applicantName?: string;
  applicantEmail?: string;
  applicantPhone?: string;
  applicantPeselMasked?: string;
  
  // Permissions (for Access Requests)
  requestedPermissions?: number;
  requestedPermissionsList?: string[];
  entityNotificationEmail?: string;
  
  // Handlers
  handlerId?: number;
  handlerName?: string;
  createdByUserId: number;
  createdByName: string;
  
  // Approval/Block info
  approvedByUserId?: number;
  approvedByName?: string;
  approvedAt?: string;
  blockedByUserId?: number;
  blockedByName?: string;
  blockedAt?: string;
  blockReason?: string;
  
  // Dates
  createdAt: string;
  updatedAt: string;
  submittedAt?: string;
  resolvedAt?: string;
  closedAt?: string;
  
  // Cancellation
  isCancelled: boolean;
  cancelledAt?: string;
  cancellationReason?: string;
  
  // Polish UI fields
  statusPolish: string;
  typePolish: string;
  
  // Counts
  documentCount: number;
  messageCount: number;
}

/**
 * Create case request
 */
export interface CreateCaseRequest {
  type: CaseType;
  title: string;
  description: string;
  category: string;
  supervisedEntityId: number;
  priority?: number;
  requestedPermissions?: number;
  entityNotificationEmail?: string;
}

/**
 * Update case request
 */
export interface UpdateCaseRequest {
  title?: string;
  description?: string;
  category?: string;
  supervisedEntityId?: number;
  priority?: number;
  requestedPermissions?: number;
  entityNotificationEmail?: string;
}

/**
 * Case filters
 */
export interface CaseFilters {
  type?: CaseType;
  status?: CaseStatus;
  supervisedEntityId?: number;
  handlerId?: number;
  category?: string;
  createdFrom?: string;
  createdTo?: string;
  searchTerm?: string;
}

/**
 * Paginated response
 */
export interface PaginatedCaseResponse {
  items: CaseListItem[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

/**
 * Case document
 */
export interface CaseDocument {
  id: number;
  caseId: number;
  fileName: string;
  filePath: string;
  fileSize: number;
  contentType: string;
  description?: string;
  uploadedAt: string;
  uploadedByUserId: number;
  uploadedByName: string;
}

/**
 * Case history entry
 */
export interface CaseHistoryEntry {
  id: number;
  caseId: number;
  changedByUserId: number;
  changedByName: string;
  changeDate: string;
  changeType: string;
  oldValue?: string;
  newValue?: string;
  oldStatus?: CaseStatus;
  newStatus?: CaseStatus;
  comments?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CaseService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/cases';

  /**
   * Get paginated cases list with optional filters
   */
  getCases(
    page: number = 1,
    pageSize: number = 10,
    filters?: CaseFilters
  ): Observable<PaginatedCaseResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters?.type) {
      params = params.set('type', filters.type);
    }

    if (filters?.status) {
      params = params.set('status', filters.status);
    }

    if (filters?.supervisedEntityId) {
      params = params.set('supervisedEntityId', filters.supervisedEntityId.toString());
    }

    if (filters?.handlerId) {
      params = params.set('handlerId', filters.handlerId.toString());
    }

    if (filters?.category) {
      params = params.set('category', filters.category);
    }

    if (filters?.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    if (filters?.createdFrom) {
      params = params.set('createdFrom', filters.createdFrom);
    }

    if (filters?.createdTo) {
      params = params.set('createdTo', filters.createdTo);
    }

    return this.http.get<PaginatedCaseResponse>(this.apiUrl, { params });
  }

  /**
   * Get case by ID
   */
  getCaseById(id: number): Observable<Case> {
    return this.http.get<Case>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create new case
   */
  createCase(request: CreateCaseRequest): Observable<Case> {
    return this.http.post<Case>(this.apiUrl, request);
  }

  /**
   * Update case
   */
  updateCase(id: number, request: UpdateCaseRequest): Observable<Case> {
    return this.http.put<Case>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Delete case (drafts only)
   */
  deleteCase(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Submit case (Working -> New)
   */
  submitCase(id: number, comments?: string): Observable<Case> {
    return this.http.post<Case>(`${this.apiUrl}/${id}/submit`, { comments });
  }

  /**
   * Approve case (UKNF only)
   */
  approveCase(id: number, comments?: string): Observable<Case> {
    return this.http.post<Case>(`${this.apiUrl}/${id}/approve`, { comments });
  }

  /**
   * Block/reject case (UKNF only)
   */
  blockCase(id: number, blockReason: string, comments?: string): Observable<Case> {
    return this.http.post<Case>(`${this.apiUrl}/${id}/block`, { blockReason, comments });
  }

  /**
   * Cancel case
   */
  cancelCase(id: number, reason: string): Observable<Case> {
    return this.http.post<Case>(`${this.apiUrl}/${id}/cancel`, { reason });
  }

  /**
   * Assign case to handler (UKNF only)
   */
  assignCase(id: number, handlerId: number): Observable<Case> {
    return this.http.post<Case>(`${this.apiUrl}/${id}/assign`, { handlerId });
  }

  /**
   * Get case documents
   */
  getCaseDocuments(caseId: number): Observable<CaseDocument[]> {
    return this.http.get<CaseDocument[]>(`${this.apiUrl}/${caseId}/documents`);
  }

  /**
   * Upload document to case
   */
  uploadDocument(caseId: number, file: File, description?: string): Observable<CaseDocument> {
    const formData = new FormData();
    formData.append('file', file);
    if (description) {
      formData.append('description', description);
    }

    return this.http.post<CaseDocument>(`${this.apiUrl}/${caseId}/documents`, formData);
  }

  /**
   * Download case document
   */
  downloadDocument(caseId: number, documentId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${caseId}/documents/${documentId}`, {
      responseType: 'blob'
    });
  }

  /**
   * Delete case document
   */
  deleteDocument(caseId: number, documentId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${caseId}/documents/${documentId}`);
  }

  /**
   * Get case history
   */
  getCaseHistory(caseId: number): Observable<CaseHistoryEntry[]> {
    return this.http.get<CaseHistoryEntry[]>(`${this.apiUrl}/${caseId}/history`);
  }

  /**
   * Get status label in Polish
   */
  getStatusLabel(status: CaseStatus): string {
    const labels: Record<CaseStatus, string> = {
      [CaseStatus.Working]: 'Roboczy',
      [CaseStatus.New]: 'Nowy',
      [CaseStatus.InProgress]: 'W trakcie',
      [CaseStatus.AwaitingUserResponse]: 'Oczekuje na odpowiedź',
      [CaseStatus.Accepted]: 'Zaakceptowany',
      [CaseStatus.Blocked]: 'Zablokowany',
      [CaseStatus.Updated]: 'Zaktualizowany',
      [CaseStatus.Resolved]: 'Rozwiązany',
      [CaseStatus.Closed]: 'Zamknięty'
    };
    return labels[status] || status;
  }

  /**
   * Get type label in Polish
   */
  getTypeLabel(type: CaseType): string {
    const labels: Record<CaseType, string> = {
      [CaseType.AccessRequest]: 'Wniosek o dostęp',
      [CaseType.Investigation]: 'Postępowanie',
      [CaseType.Audit]: 'Audyt',
      [CaseType.Compliance]: 'Zgodność',
      [CaseType.General]: 'Ogólny'
    };
    return labels[type] || type;
  }
}
