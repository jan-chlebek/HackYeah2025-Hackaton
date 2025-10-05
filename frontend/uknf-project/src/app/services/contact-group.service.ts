import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Contact, ContactListItem } from './contact.service';

/**
 * Contact group detailed information
 */
export interface ContactGroup {
  id: number;
  name: string;
  description: string | null;
  createdByUserId: number;
  createdByUserName?: string;
  createdAt: string;
  memberCount: number;
  members?: ContactGroupMember[];
}

/**
 * Simplified contact group for list views
 */
export interface ContactGroupListItem {
  id: number;
  name: string;
  description: string | null;
  memberCount: number;
  createdAt: string;
}

/**
 * Contact group member information
 */
export interface ContactGroupMember {
  id: number;
  contactGroupId: number;
  contactId: number;
  contactName: string;
  contactEmail: string;
  contactPhone: string | null;
  supervisedEntityName: string | null;
  addedAt: string;
}

/**
 * Pagination information
 */
export interface ContactGroupPagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

/**
 * Response for contact group list with pagination
 */
export interface ContactGroupListResponse {
  data: ContactGroupListItem[];
  pagination: ContactGroupPagination;
}

/**
 * Request to create a new contact group
 */
export interface CreateContactGroupRequest {
  name: string;
  description?: string | null;
}

/**
 * Request to update an existing contact group
 */
export interface UpdateContactGroupRequest {
  name: string;
  description?: string | null;
}

/**
 * Request to add member to contact group
 */
export interface AddMemberRequest {
  contactId: number;
}

/**
 * Request to add multiple members to contact group
 */
export interface AddMembersRequest {
  contactIds: number[];
}

/**
 * Filters for contact group search
 */
export interface ContactGroupFilters {
  searchTerm?: string;
}

/**
 * Service for managing contact groups
 * Provides CRUD operations for organizing contacts into groups for bulk messaging
 */
@Injectable({
  providedIn: 'root'
})
export class ContactGroupService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/contact-groups';

  /**
   * Get list of contact groups with pagination and filtering
   */
  getContactGroups(
    page: number = 1,
    pageSize: number = 20,
    filters?: ContactGroupFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<ContactGroupListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
    }
    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }

    return this.http.get<ContactGroupListResponse>(this.apiUrl, { params });
  }

  /**
   * Get all contact groups (no pagination) - useful for dropdowns
   */
  getAllContactGroups(): Observable<ContactGroupListItem[]> {
    return this.http.get<ContactGroupListItem[]>(`${this.apiUrl}/all`);
  }

  /**
   * Get contact group by ID with full details including members
   */
  getContactGroupById(id: number): Observable<ContactGroup> {
    return this.http.get<ContactGroup>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new contact group
   */
  createContactGroup(request: CreateContactGroupRequest): Observable<ContactGroup> {
    return this.http.post<ContactGroup>(this.apiUrl, request);
  }

  /**
   * Update an existing contact group
   */
  updateContactGroup(id: number, request: UpdateContactGroupRequest): Observable<ContactGroup> {
    return this.http.put<ContactGroup>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Delete a contact group
   */
  deleteContactGroup(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get members of a contact group
   */
  getGroupMembers(groupId: number): Observable<ContactGroupMember[]> {
    return this.http.get<ContactGroupMember[]>(`${this.apiUrl}/${groupId}/members`);
  }

  /**
   * Add a single member to contact group
   */
  addMember(groupId: number, request: AddMemberRequest): Observable<ContactGroupMember> {
    return this.http.post<ContactGroupMember>(`${this.apiUrl}/${groupId}/members`, request);
  }

  /**
   * Add multiple members to contact group
   */
  addMembers(groupId: number, request: AddMembersRequest): Observable<ContactGroupMember[]> {
    return this.http.post<ContactGroupMember[]>(`${this.apiUrl}/${groupId}/members/bulk`, request);
  }

  /**
   * Remove a member from contact group
   */
  removeMember(groupId: number, memberId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${groupId}/members/${memberId}`);
  }

  /**
   * Remove multiple members from contact group
   */
  removeMembers(groupId: number, memberIds: number[]): Observable<void> {
    return this.http.request<void>('delete', `${this.apiUrl}/${groupId}/members/bulk`, {
      body: { memberIds }
    });
  }

  /**
   * Get available contacts to add to group (contacts not already in the group)
   */
  getAvailableContacts(groupId: number): Observable<ContactListItem[]> {
    return this.http.get<ContactListItem[]>(`${this.apiUrl}/${groupId}/available-contacts`);
  }
}
