import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard to protect routes that require authentication
 * SSR-compatible - checks for browser environment before accessing sessionStorage
 * MOCK MODE: Always allows access
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);
  const isBrowser = isPlatformBrowser(platformId);
  
  // MOCK MODE: Always return true to allow access
  console.log('Auth guard - MOCK MODE: allowing access to', state.url);
  return true;
  
  /* Original auth check - commented out for mock
  if (authService.isAuthenticated()) {
    return true;
  }
  
  // Store the attempted URL for redirecting after login (browser only)
  if (isBrowser) {
    try {
      sessionStorage.setItem('redirectUrl', state.url);
    } catch (e) {
      // Ignore sessionStorage errors
    }
  }
  
  // Redirect to login page
  return router.createUrlTree(['/auth/login']);
  */
};

/**
 * Guard to protect routes that require specific roles
 * SSR-compatible - checks for browser environment before accessing sessionStorage
 * MOCK MODE: Always allows access
 */
export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const platformId = inject(PLATFORM_ID);
    const isBrowser = isPlatformBrowser(platformId);
    
    // MOCK MODE: Always return true to allow access
    console.log('Role guard - MOCK MODE: allowing access to', state.url);
    return true;
    
    /* Original role check - commented out for mock
    if (!authService.isAuthenticated()) {
      // Store the attempted URL for redirecting after login (browser only)
      if (isBrowser) {
        try {
          sessionStorage.setItem('redirectUrl', state.url);
        } catch (e) {
          // Ignore sessionStorage errors
        }
      }
      return router.createUrlTree(['/auth/login']);
    }
    
    if (authService.hasAnyRole(allowedRoles)) {
      return true;
    }
    
    // User doesn't have required role - redirect to dashboard
    return router.createUrlTree(['/dashboard']);
    */
  };
};
