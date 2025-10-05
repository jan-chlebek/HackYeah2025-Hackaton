import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { LoginComponent } from './login.component';
import { AuthService, LoginResponse } from '../../../services/auth.service';
import { AccessibilityService } from '../../../shared/services/accessibility.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login']);

    TestBed.configureTestingModule({
      imports: [LoginComponent, RouterTestingModule],
      providers: [
  { provide: AuthService, useValue: authServiceSpy },
  AccessibilityService
      ]
    });

    const fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should display validation error when form invalid', () => {
    component.onSubmit();

    expect(component.errorMessage()).toContain('Uzupełnij wymagane pola');
  });

  it('should call auth service when form is valid', async () => {
    const loginResponse: LoginResponse = {
      token: 'mock',
      user: {
        id: '1',
        email: 'test@example.com',
        displayName: 'Test User',
        roles: ['Pracownik']
      }
    };

    authService.login.and.returnValue(of(loginResponse));
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));

    component.loginForm.setValue({
      email: 'test@example.com',
      password: 'password123',
      rememberMe: true
    });

    component.onSubmit();

    expect(component.isLoading()).toBeFalse();
    expect(authService.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123',
      rememberMe: true
    });
    expect(router.navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should toggle password visibility', () => {
    expect(component.showPassword()).toBeFalse();
    component.togglePasswordVisibility();
    expect(component.showPassword()).toBeTrue();
  });

  it('should defer password minlength error until submit', () => {
    const passwordControl = component.loginForm.get('password');
    passwordControl?.setValue('short');
    passwordControl?.markAsTouched();

    expect(component.getErrorMessage('password')).toBe('');

    component.onSubmit();

    expect(component.getErrorMessage('password')).toContain('Minimalna długość');
  });
});
