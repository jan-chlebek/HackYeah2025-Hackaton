import { Injectable, signal, computed, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, throwError, timer } from 'rxjs';
import { tap, catchError, switchMap } from 'rxjs/operators';
import { LoginRequest, LoginResponse, RefreshTokenRequest, UserInfo } from '../models/auth.models';

/**
 * Authentication service handling login, logout, token management
 * Stores tokens in memory for security (not localStorage)
 * SSR-compatible - checks for browser environment before accessing sessionStorage
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly isBrowser: boolean = isPlatformBrowser(this.platformId);
  
  // API base URL - should be from environment config
  private readonly API_URL = '/api/v1/auth';
  
  // Token storage in memory (more secure than localStorage)
  private accessToken: string | null = null;
  private refreshToken: string | null = null;
  
  // Current user signal
  private currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Computed authentication state
  public isAuthenticated = signal<boolean>(false);
  public currentUser = signal<UserInfo | null>(null);
  
  // Token refresh timer
  private tokenRefreshTimer: any = null;

  constructor() {
    // Try to restore session from sessionStorage on app init (only in browser)
    if (this.isBrowser) {
      this.restoreSession();
    }
  }

  /**
   * Login with email and password
   */
  login(email: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = { email, password };
    
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, request).pipe(
      tap(response => {
        this.handleAuthSuccess(response);
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Logout current user
   */
  logout(reason?: string): Observable<void> {
    const token = this.refreshToken;
    
    // Clear local state immediately
    this.clearAuthState();
    
    // Navigate to login
    this.router.navigate(['/auth/login']);
    
    // Revoke token on server (best effort - don't block logout)
    if (token) {
      return this.http.post<void>(`${this.API_URL}/revoke`, { 
        refreshToken: token,
        reason 
      }).pipe(
        catchError(() => {
          // Ignore errors on logout
          return new Observable<void>(observer => observer.complete());
        })
      );
    }
    
    return new Observable<void>(observer => observer.complete());
  }

  /**
   * Refresh access token using refresh token
   */
  refreshAccessToken(): Observable<LoginResponse> {
    if (!this.accessToken || !this.refreshToken) {
      return throwError(() => new Error('No tokens available for refresh'));
    }

    const request: RefreshTokenRequest = {
      accessToken: this.accessToken,
      refreshToken: this.refreshToken
    };

    return this.http.post<LoginResponse>(`${this.API_URL}/refresh`, request).pipe(
      tap(response => {
        this.handleAuthSuccess(response);
      }),
      catchError(error => {
        // If refresh fails, logout user
        this.logout('Token refresh failed');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get current access token
   */
  getAccessToken(): string | null {
    return this.accessToken;
  }

  /**
   * Check if user has specific role
   */
  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user?.roles.includes(role) ?? false;
  }

  /**
   * Check if user has specific permission
   */
  hasPermission(permission: string): boolean {
    const user = this.currentUser();
    return user?.permissions.includes(permission) ?? false;
  }

  /**
   * Check if user has any of the specified roles
   */
  hasAnyRole(roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  /**
   * Handle successful authentication
   */
  private handleAuthSuccess(response: LoginResponse): void {
    // Store tokens in memory
    this.accessToken = response.accessToken;
    this.refreshToken = response.refreshToken;
    
    // Store minimal session info in sessionStorage (survives page refresh) - browser only
    if (this.isBrowser) {
      sessionStorage.setItem('session', JSON.stringify({
        user: response.user,
        expiresAt: Date.now() + (response.expiresIn * 1000)
      }));
    }
    
    // Update state
    this.currentUser.set(response.user);
    this.isAuthenticated.set(true);
    this.currentUserSubject.next(response.user);
    
    // Schedule token refresh (5 minutes before expiry)
    this.scheduleTokenRefresh(response.expiresIn);
  }

  /**
   * Clear authentication state
   */
  private clearAuthState(): void {
    this.accessToken = null;
    this.refreshToken = null;
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.currentUserSubject.next(null);
    
    // Clear sessionStorage only in browser
    if (this.isBrowser) {
      sessionStorage.removeItem('session');
    }
    
    if (this.tokenRefreshTimer) {
      clearTimeout(this.tokenRefreshTimer);
      this.tokenRefreshTimer = null;
    }
  }

  /**
   * Schedule automatic token refresh
   */
  private scheduleTokenRefresh(expiresIn: number): void {
    // Clear existing timer
    if (this.tokenRefreshTimer) {
      clearTimeout(this.tokenRefreshTimer);
    }
    
    // Refresh 5 minutes before expiry
    const refreshTime = (expiresIn - 300) * 1000;
    
    if (refreshTime > 0) {
      this.tokenRefreshTimer = setTimeout(() => {
        this.refreshAccessToken().subscribe({
          error: (err) => {
            console.error('Token refresh failed:', err);
            this.logout('Session expired');
          }
        });
      }, refreshTime);
    }
  }

  /**
   * Restore session from sessionStorage (browser only)
   */
  private restoreSession(): void {
    // Only attempt to restore session in browser environment
    if (!this.isBrowser) {
      return;
    }
    
    try {
      const sessionData = sessionStorage.getItem('session');
      
      if (sessionData) {
        const session = JSON.parse(sessionData);
        
        // Check if session is still valid
        if (session.expiresAt && session.expiresAt > Date.now()) {
          this.currentUser.set(session.user);
          this.isAuthenticated.set(true);
          this.currentUserSubject.next(session.user);
          
          // Note: We don't have tokens in memory, so first API call will fail
          // and trigger a proper re-authentication
        } else {
          // Session expired
          sessionStorage.removeItem('session');
        }
      }
    } catch (error) {
      console.error('Failed to restore session:', error);
      try {
        sessionStorage.removeItem('session');
      } catch (e) {
        // Ignore errors when clearing sessionStorage
      }
    }
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Wystąpił błąd podczas logowania';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Błąd: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Nieprawidłowy email lub hasło';
      } else if (error.status === 423) {
        errorMessage = 'Konto zostało zablokowane';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }
}
