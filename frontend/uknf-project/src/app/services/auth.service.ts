import { inject, Injectable } from '@angular/core';
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
  
  private currentUserSubject = new BehaviorSubject<UserInfoDto | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  /**
   * Perform login using backend API endpoint
   */
  login(payload: LoginPayload): Observable<LoginResponse> {
    if (!payload.email || !payload.password) {
      throw new Error('Brak wymaganych danych logowania.');
    }

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        // Store tokens in localStorage
        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);
        localStorage.setItem('tokenType', response.tokenType);
        localStorage.setItem('expiresIn', response.expiresIn.toString());
        
        // Store user info
        localStorage.setItem('currentUser', JSON.stringify(response.user));
        this.currentUserSubject.next(response.user);
      })
    );
  }

  /**
   * Logout user and clear stored tokens
   */
  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('tokenType');
    localStorage.removeItem('expiresIn');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    
    // Optional: Call backend logout endpoint
    // this.http.post(`${this.apiUrl}/logout`, {}).subscribe();
  }

  /**
   * Get current user from localStorage
   */
  getCurrentUser(): UserInfoDto | null {
    const userJson = localStorage.getItem('currentUser');
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
    return localStorage.getItem('accessToken');
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  /**
   * Check if user has elevated permissions (more than Internal User)
   * Internal User typically has 4 permissions
   * Supervisor has 7 permissions
   * Administrator has 9 permissions
   */
  hasElevatedPermissions(): boolean {
    const user = this.getCurrentUser();
    if (!user) {
      return false;
    }

    // Check by role
    const elevatedRoles = ['Administrator', 'Supervisor'];
    const hasElevatedRole = user.roles.some(role => elevatedRoles.includes(role));
    
    // Or check by permission count (more than 4 means elevated)
    const hasMorePermissions = user.permissions.length > 4;

    return hasElevatedRole || hasMorePermissions;
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
}
