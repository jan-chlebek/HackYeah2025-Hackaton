import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { LoginResponse, UserInfo } from '../models/auth.models';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockLoginResponse: LoginResponse = {
    accessToken: 'mock-access-token',
    refreshToken: 'mock-refresh-token',
    tokenType: 'Bearer',
    expiresIn: 3600,
    user: {
      id: 1,
      email: 'test@example.com',
      fullName: 'Test User',
      roles: ['Pracownik UKNF'],
      permissions: ['view:dashboard', 'manage:messages'],
      supervisedEntityId: undefined
    }
  };

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    
    // Clear sessionStorage before each test
    sessionStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  describe('Login', () => {
    it('should login successfully and store user data', (done) => {
      service.login('test@example.com', 'password123').subscribe({
        next: (response) => {
          expect(response).toEqual(mockLoginResponse);
          expect(service.isAuthenticated()).toBe(true);
          expect(service.currentUser()).toEqual(mockLoginResponse.user);
          expect(service.getAccessToken()).toBe('mock-access-token');
          done();
        }
      });

      const req = httpMock.expectOne('/api/v1/auth/login');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({
        email: 'test@example.com',
        password: 'password123'
      });
      req.flush(mockLoginResponse);
    });

    it('should handle login errors', (done) => {
      service.login('test@example.com', 'wrongpassword').subscribe({
        error: (error) => {
          expect(error.message).toContain('Nieprawidłowy email lub hasło');
          expect(service.isAuthenticated()).toBe(false);
          done();
        }
      });

      const req = httpMock.expectOne('/api/v1/auth/login');
      req.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });
    });

    it('should store session in sessionStorage', (done) => {
      service.login('test@example.com', 'password123').subscribe({
        next: () => {
          const session = sessionStorage.getItem('session');
          expect(session).toBeTruthy();
          
          const sessionData = JSON.parse(session!);
          expect(sessionData.user).toEqual(mockLoginResponse.user);
          expect(sessionData.expiresAt).toBeGreaterThan(Date.now());
          done();
        }
      });

      const req = httpMock.expectOne('/api/v1/auth/login');
      req.flush(mockLoginResponse);
    });
  });

  describe('Logout', () => {
    beforeEach((done) => {
      // Login first
      service.login('test@example.com', 'password123').subscribe(() => {
        const req = httpMock.expectOne('/api/v1/auth/login');
        req.flush(mockLoginResponse);
        done();
      });
    });

    it('should clear authentication state on logout', (done) => {
      service.logout('User logout').subscribe({
        complete: () => {
          expect(service.isAuthenticated()).toBe(false);
          expect(service.currentUser()).toBeNull();
          expect(service.getAccessToken()).toBeNull();
          expect(sessionStorage.getItem('session')).toBeNull();
          expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
          done();
        }
      });

      // May or may not have a revoke request depending on token availability
      const requests = httpMock.match('/api/v1/auth/revoke');
      if (requests.length > 0) {
        requests[0].flush({});
      }
    });
  });

  describe('Token Refresh', () => {
    beforeEach((done) => {
      // Login first
      service.login('test@example.com', 'password123').subscribe(() => {
        const req = httpMock.expectOne('/api/v1/auth/login');
        req.flush(mockLoginResponse);
        done();
      });
    });

    it('should refresh access token successfully', (done) => {
      const refreshedResponse: LoginResponse = {
        ...mockLoginResponse,
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token'
      };

      service.refreshAccessToken().subscribe({
        next: (response) => {
          expect(response).toEqual(refreshedResponse);
          expect(service.getAccessToken()).toBe('new-access-token');
          done();
        }
      });

      const req = httpMock.expectOne('/api/v1/auth/refresh');
      expect(req.request.method).toBe('POST');
      req.flush(refreshedResponse);
    });

    it('should logout on refresh failure', (done) => {
      service.refreshAccessToken().subscribe({
        error: () => {
          expect(service.isAuthenticated()).toBe(false);
          expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
          done();
        }
      });

      const req = httpMock.expectOne('/api/v1/auth/refresh');
      req.flush({ message: 'Invalid token' }, { status: 401, statusText: 'Unauthorized' });
    });
  });

  describe('Role and Permission Checks', () => {
    beforeEach((done) => {
      service.login('test@example.com', 'password123').subscribe(() => {
        const req = httpMock.expectOne('/api/v1/auth/login');
        req.flush(mockLoginResponse);
        done();
      });
    });

    it('should check if user has role', () => {
      expect(service.hasRole('Pracownik UKNF')).toBe(true);
      expect(service.hasRole('Administrator systemu')).toBe(false);
    });

    it('should check if user has permission', () => {
      expect(service.hasPermission('view:dashboard')).toBe(true);
      expect(service.hasPermission('manage:users')).toBe(false);
    });

    it('should check if user has any of specified roles', () => {
      expect(service.hasAnyRole(['Administrator systemu', 'Pracownik UKNF'])).toBe(true);
      expect(service.hasAnyRole(['Administrator systemu', 'Admin Podmiotu'])).toBe(false);
    });
  });

  describe('Session Restoration', () => {
    it('should restore session from sessionStorage on init', () => {
      const sessionData = {
        user: mockLoginResponse.user,
        expiresAt: Date.now() + 3600000 // 1 hour from now
      };
      
      sessionStorage.setItem('session', JSON.stringify(sessionData));

      // Create new service instance
      const newService = new AuthService();

      expect(newService.isAuthenticated()).toBe(true);
      expect(newService.currentUser()).toEqual(mockLoginResponse.user);
    });

    it('should not restore expired session', () => {
      const sessionData = {
        user: mockLoginResponse.user,
        expiresAt: Date.now() - 1000 // Expired
      };
      
      sessionStorage.setItem('session', JSON.stringify(sessionData));

      // Create new service instance
      const newService = new AuthService();

      expect(newService.isAuthenticated()).toBe(false);
      expect(sessionStorage.getItem('session')).toBeNull();
    });
  });
});
