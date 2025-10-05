import { inject, Injectable, signal, computed, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface LoginPayload {
  email: string;
  password: string;
}

export interface UserInfoDto {
  id: number;
  email: string;
  fullName: string;
  firstName?: string;
  lastName?: string;
  roles: string[];
  permissions: string[];
  supervisedEntityId?: number | null;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserInfoDto;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api/v1/Auth';
  private readonly platformId = inject(PLATFORM_ID);
  private readonly isBrowser = isPlatformBrowser(this.platformId);
  
  private currentUserSubject = new BehaviorSubject<UserInfoDto | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Signal-based current user for reactive components
  private currentUserSignal = signal<UserInfoDto | null>(null);
  public currentUser = this.currentUserSignal.asReadonly();
  
  // Computed signal for elevated permissions check
  public hasElevatedPermissionsSignal = computed(() => {
    const user = this.currentUserSignal();
    
    console.log('[AuthService] Computing hasElevatedPermissions:', {
      user,
      roles: user?.roles,
      permissions: user?.permissions,
      permissionCount: user?.permissions.length
    });
    
    if (!user) {
      console.log('[AuthService] No user, returning false');
      return false;
    }

    // Check by role name first - most reliable
    const elevatedRoles = ['Administrator', 'Supervisor'];
    const hasElevatedRole = user.roles.some(role => elevatedRoles.includes(role));
    
    // Or check by permission count (more than 4 means elevated)
    const hasMoreThanInternalUser = user.permissions.length > 4;

    const result = hasElevatedRole || hasMoreThanInternalUser;
    
    console.log('[AuthService] hasElevatedPermissions result:', {
      hasElevatedRole,
      hasMoreThanInternalUser,
      result
    });

    return result;
  });
  
  private loadUserFromStorage(): UserInfoDto | null {
    if (!this.isBrowser) {
      // SSR: do not attempt to access localStorage
      return null;
    }
    const userJson = this.safeGetItem('currentUser');
    console.log('[AuthService] Loading user from storage:', userJson);
    if (!userJson) {
      return null;
    }
    try {
      const user = JSON.parse(userJson) as UserInfoDto;
      console.log('[AuthService] Loaded user:', user);
      return user;
    } catch (error) {
      console.error('[AuthService] Error parsing user from storage:', error);
      return null;
    }
  }

  /**
   * Perform login using backend API endpoint
   */
  login(payload: LoginPayload): Observable<LoginResponse> {
    if (!payload.email || !payload.password) {
      throw new Error('Brak wymaganych danych logowania.');
    }

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        console.log('[AuthService] Login successful:', response.user);
        
        if (this.isBrowser) {
          // Store tokens in localStorage (browser only)
          this.safeSetItem('accessToken', response.accessToken);
          this.safeSetItem('refreshToken', response.refreshToken);
          this.safeSetItem('tokenType', response.tokenType);
            this.safeSetItem('expiresIn', response.expiresIn.toString());
          // Store user info
          this.safeSetItem('currentUser', JSON.stringify(response.user));
        }
        this.currentUserSubject.next(response.user);
        this.currentUserSignal.set(response.user); // Update signal
        
        console.log('[AuthService] Signal updated, new value:', this.currentUserSignal());
      })
    );
  }

  /**
   * Logout user and clear stored tokens
   */
  logout(): void {
    if (this.isBrowser) {
      this.safeRemoveItem('accessToken');
      this.safeRemoveItem('refreshToken');
      this.safeRemoveItem('tokenType');
      this.safeRemoveItem('expiresIn');
      this.safeRemoveItem('currentUser');
    }
    this.currentUserSubject.next(null);
    this.currentUserSignal.set(null); // Update signal
    
    // Optional: Call backend logout endpoint
    // this.http.post(`${this.apiUrl}/logout`, {}).subscribe();
  }

  /**
   * Get current user from localStorage
   */
  getCurrentUser(): UserInfoDto | null {
    if (!this.isBrowser) {
      return null;
    }
    const userJson = this.safeGetItem('currentUser');
    if (!userJson) {
      return null;
    }
    try {
      return JSON.parse(userJson) as UserInfoDto;
    } catch {
      return null;
    }
  }

  /**
   * Get access token from localStorage
   */
  getAccessToken(): string | null {
    if (!this.isBrowser) {
      return null;
    }
    return this.safeGetItem('accessToken');
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  /**
   * Check if user has elevated permissions (more than Internal User)
   * External User has 0 permissions → NOT elevated
   * Internal User has 4 permissions → NOT elevated
   * Supervisor has 7 permissions → ELEVATED
   * Administrator has 9 permissions → ELEVATED
   * 
   * Returns true only for Supervisor and Administrator (more than 4 permissions)
   */
  hasElevatedPermissions(): boolean {
    const user = this.getCurrentUser();
    if (!user) {
      return false;
    }

    // Check by role name first - most reliable
    const elevatedRoles = ['Administrator', 'Supervisor'];
    const hasElevatedRole = user.roles.some(role => elevatedRoles.includes(role));
    
    // Or check by permission count (more than 4 means elevated - Supervisor or Admin)
    // External users: 0 permissions → false
    // Internal users: 4 permissions → false
    // Supervisors: 7 permissions → true
    // Administrators: 9 permissions → true
    const hasMoreThanInternalUser = user.permissions.length > 4;

    return hasElevatedRole || hasMoreThanInternalUser;
  }

  /**
   * Check if user has a specific permission
   */
  hasPermission(permission: string): boolean {
    const user = this.getCurrentUser();
    return user?.permissions.includes(permission) ?? false;
  }

  /**
   * Check if user has a specific role
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.roles.includes(role) ?? false;
  }

  // ---- Safe storage helpers (SSR aware) ----
  private safeGetItem(key: string): string | null {
    try {
      if (!this.isBrowser) return null;
      return window.localStorage.getItem(key);
    } catch (e) {
      console.warn('[AuthService] safeGetItem failed', { key, e });
      return null;
    }
  }

  private safeSetItem(key: string, value: string): void {
    try {
      if (!this.isBrowser) return;
      window.localStorage.setItem(key, value);
    } catch (e) {
      console.warn('[AuthService] safeSetItem failed', { key, e });
    }
  }

  private safeRemoveItem(key: string): void {
    try {
      if (!this.isBrowser) return;
      window.localStorage.removeItem(key);
    } catch (e) {
      console.warn('[AuthService] safeRemoveItem failed', { key, e });
    }
  }

  // Initialize user from storage only in browser to avoid SSR errors
  constructor() {
    if (this.isBrowser) {
      const stored = this.loadUserFromStorage();
      if (stored) {
        this.currentUserSubject.next(stored);
        this.currentUserSignal.set(stored);
      }
    }
  }
}
