import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Supervised entity detailed information
 */
export interface SupervisedEntity {
  id: number;
  name: string;
  entityType: string;
  uknfCode: string | null;
  lei: string | null;
  nip: string | null;
  regon: string | null;
  krs: string | null;
  
  // Address
  street: string | null;
  buildingNumber: string | null;
  apartmentNumber: string | null;
  postalCode: string | null;
  city: string | null;
  country: string | null;
  
  // Contact
  phone: string | null;
  email: string | null;
  website: string | null;
  
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  
  // Statistics
  userCount: number;
  reportCount: number;
}

/**
 * Simplified entity for list views
 */
export interface SupervisedEntityListItem {
  id: number;
  name: string;
  entityType: string;
  nip: string | null;
  regon: string | null;
  city: string | null;
  isActive: boolean;
  userCount: number;
  createdAt: string;
}

/**
 * Pagination information
 */
export interface EntityPagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

/**
 * Response for entity list with pagination
 */
export interface EntityListResponse {
  data: SupervisedEntityListItem[];
  pagination: EntityPagination;
}

/**
 * Request to create a new supervised entity
 */
export interface CreateEntityRequest {
  name: string;
  entityType: string;
  uknfCode?: string | null;
  lei?: string | null;
  nip?: string | null;
  regon?: string | null;
  krs?: string | null;
  
  // Address
  street?: string | null;
  buildingNumber?: string | null;
  apartmentNumber?: string | null;
  postalCode?: string | null;
  city?: string | null;
  country?: string | null;
  
  // Contact
  phone?: string | null;
  email?: string | null;
  website?: string | null;
}

/**
 * Request to update an existing supervised entity
 */
export interface UpdateEntityRequest {
  name: string;
  entityType: string;
  lei?: string | null;
  nip?: string | null;
  regon?: string | null;
  krs?: string | null;
  
  // Address
  street?: string | null;
  buildingNumber?: string | null;
  apartmentNumber?: string | null;
  postalCode?: string | null;
  city?: string | null;
  country?: string | null;
  
  // Contact
  phone?: string | null;
  email?: string | null;
  website?: string | null;
  
  isActive: boolean;
}

/**
 * User assigned to an entity
 */
export interface EntityUser {
  id: number;
  email: string;
  fullName: string;
  isActive: boolean;
  roleNames: string[];
  createdAt: string;
  lastLoginAt: string | null;
}

/**
 * Filters for entity search
 */
export interface EntityFilters {
  searchTerm?: string;
  entityType?: string;
  isActive?: boolean;
}

/**
 * Service for managing the register of supervised entities
 * Provides CRUD operations and user management for entities
 */
@Injectable({
  providedIn: 'root'
})
export class SupervisedEntityService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5000/api/v1/entities';

  /**
   * Get list of supervised entities with pagination and filtering
   * @param page Page number (default: 1)
   * @param pageSize Items per page (default: 20)
   * @param filters Optional filters (searchTerm, entityType, isActive)
   * @param sortField Optional field to sort by
   * @param sortOrder Sort direction (asc or desc)
   * @returns Observable of entity list with pagination
   */
  getEntities(
    page: number = 1,
    pageSize: number = 20,
    filters?: EntityFilters,
    sortField?: string,
    sortOrder?: 'asc' | 'desc'
  ): Observable<EntityListResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filters) {
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
      if (filters.entityType) {
        params = params.set('entityType', filters.entityType);
      }
      if (filters.isActive !== undefined) {
        params = params.set('isActive', filters.isActive.toString());
      }
    }

    if (sortField) {
      params = params.set('sortField', sortField);
    }
    if (sortOrder) {
      params = params.set('sortOrder', sortOrder);
    }

    return this.http.get<EntityListResponse>(this.apiUrl, { params });
  }

  /**
   * Get supervised entity by ID with full details
   * @param id Entity ID
   * @returns Observable of entity details
   */
  getEntityById(id: number): Observable<SupervisedEntity> {
    return this.http.get<SupervisedEntity>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new supervised entity
   * @param request Entity creation data
   * @returns Observable of created entity
   */
  createEntity(request: CreateEntityRequest): Observable<SupervisedEntity> {
    return this.http.post<SupervisedEntity>(this.apiUrl, request);
  }

  /**
   * Update an existing supervised entity
   * @param id Entity ID
   * @param request Updated entity data
   * @returns Observable of updated entity
   */
  updateEntity(id: number, request: UpdateEntityRequest): Observable<SupervisedEntity> {
    return this.http.put<SupervisedEntity>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Delete a supervised entity (soft delete - sets isActive to false)
   * @param id Entity ID
   * @returns Observable of void
   */
  deleteEntity(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get list of users assigned to a supervised entity
   * @param id Entity ID
   * @returns Observable of entity users
   */
  getEntityUsers(id: number): Observable<EntityUser[]> {
    return this.http.get<EntityUser[]>(`${this.apiUrl}/${id}/users`);
  }

  /**
   * Bulk import entities from CSV
   * @param file CSV file with entity data
   * @returns Observable of import result
   */
  importEntitiesFromCsv(file: File): Observable<{ imported: number; failed: number; errors: string[] }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imported: number; failed: number; errors: string[] }>(
      `${this.apiUrl}/import`,
      formData
    );
  }
}
