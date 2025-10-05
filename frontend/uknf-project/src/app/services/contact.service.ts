import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Contact detailed information
 */
export interface Contact {
  id: number;
  name: string;
  position: string | null;
  email: string;
  phone: string | null;
  mobile: string | null;
  supervisedEntityId: number | null;
  supervisedEntityName?: string | null;
  department: string | null;
  notes: string | null;
  isPrimary: boolean;
  isActive: boolean;
  createdByUserId: number;
  createdAt: string;
  updatedAt: string;
  groupMemberships?: ContactGroupMembership[];
}

/**
 * Simplified contact for list views
 */
export interface ContactListItem {
  id: number;
  name: string;
  position: string | null;
  email: string;
  phone: string | null;
  supervisedEntityName: string | null;
  isPrimary: boolean;
  isActive: boolean;
  groupCount: number;
}

/**
 * Contact group membership
 */
export interface ContactGroupMembership {
  id: number;
  contactGroupId: number;
  contactGroupName: string;
  contactId: number;
  addedAt: string;
}

/**
 * Pagination information
 */
export interface ContactPagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

/**
 * Response for contact list with pagination
 */
export interface ContactListResponse {
  data: ContactListItem[];
  pagination: ContactPagination;
}

/**
 * Request to create a new contact
 */
export interface CreateContactRequest {
  name: string;
  position?: string | null;
  email: string;
  phone?: string | null;
  mobile?: string | null;
  supervisedEntityId?: number | null;
  department?: string | null;
  notes?: string | null;
  isPrimary?: boolean;
  isActive?: boolean;
}

/**
 * Request to update an existing contact
 */
export interface UpdateContactRequest {
  name: string;
  position?: string | null;
  email: string;
  phone?: string | null;
  mobile?: string | null;
  supervisedEntityId?: number | null;
  department?: string | null;
  notes?: string | null;
  isPrimary: boolean;
  isActive: boolean;
}

/**
 * Filters for contact search
 */
export interface ContactFilters {
  searchTerm?: string;
  supervisedEntityId?: number;
  isActive?: boolean;
  isPrimary?: boolean;
}

/**
 * Service for managing contacts (receivers/addressees)
 * Provides CRUD operations for contact registry
 */
@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/contacts';

  /**
   * Get list of contacts with pagination and filtering
   */
  getContacts(
    page: number = 1,
    pageSize: number = 20,
    filters?: ContactFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<ContactListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
      if (filters.supervisedEntityId) {
        params = params.set('supervisedEntityId', filters.supervisedEntityId.toString());
      }
      if (filters.isActive !== undefined) {
        params = params.set('isActive', filters.isActive.toString());
      }
      if (filters.isPrimary !== undefined) {
        params = params.set('isPrimary', filters.isPrimary.toString());
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
    }
    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }

    return this.http.get<ContactListResponse>(this.apiUrl, { params });
  }

  /**
   * Get contact by ID with full details
   */
  getContactById(id: number): Observable<Contact> {
    return this.http.get<Contact>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new contact
   */
  createContact(request: CreateContactRequest): Observable<Contact> {
    return this.http.post<Contact>(this.apiUrl, request);
  }

  /**
   * Update an existing contact
   */
  updateContact(id: number, request: UpdateContactRequest): Observable<Contact> {
    return this.http.put<Contact>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Delete a contact (soft delete - sets isActive to false)
   */
  deleteContact(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get contacts by supervised entity ID
   */
  getContactsByEntity(entityId: number): Observable<ContactListItem[]> {
    return this.http.get<ContactListItem[]>(`${this.apiUrl}/by-entity/${entityId}`);
  }

  /**
   * Get primary contact for a supervised entity
   */
  getPrimaryContact(entityId: number): Observable<Contact | null> {
    return this.http.get<Contact | null>(`${this.apiUrl}/primary/${entityId}`);
  }
}
