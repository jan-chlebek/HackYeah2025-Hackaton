import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { AuthService } from '../services/auth.service';

// Functional interceptor (Angular 16+ style) adding Authorization header if access token present
export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getAccessToken();
  
  console.log('[AuthInterceptor] Request:', req.url);
  console.log('[AuthInterceptor] Token present:', !!token);
  
  if (token) {
    console.log('[AuthInterceptor] Adding Authorization header');
    const cloned: HttpRequest<any> = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(cloned);
  } else {
    console.warn('[AuthInterceptor] No token available - request will be sent without authentication');
  }
  
  return next(req);
};
