import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';

import { AuthService } from '../../../services/auth.service';
import { AccessibilityService } from '../../../shared/services/accessibility.service';
import { AccessibilityControlsComponent } from '../../../shared/components/accessibility-controls/accessibility-controls.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AccessibilityControlsComponent],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService: AuthService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly accessibilityService = inject(AccessibilityService);

  loginForm!: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  showPassword = signal(false);
  formSubmitted = signal(false);
  highContrastMode = computed(() => this.accessibilityService.highContrast);

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    this.formSubmitted.set(true);

    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      this.errorMessage.set('Uzupełnij wymagane pola przed zalogowaniem.');
      return;
    }

    this.errorMessage.set(null);
    this.isLoading.set(true);

    const { email, password } = this.loginForm.getRawValue();

    this.authService
      .login({ email, password })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (response) => {
          // Log user info for debugging
          console.log('Login successful:', {
            email: response.user.email,
            fullName: response.user.fullName,
            roles: response.user.roles,
            permissions: response.user.permissions,
            hasElevatedPermissions: this.authService.hasElevatedPermissions()
          });

          this.router.navigate(['/messages']).catch(err => console.error('Navigation error:', err));
        },
        error: (error: unknown) => {
          console.error('Login error:', error);
          this.errorMessage.set('Nie udało się zalogować. Sprawdź dane i spróbuj ponownie.');
        }
      });
  }

  togglePasswordVisibility(): void {
    this.showPassword.update(current => !current);
  }


  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);

    if (!control || (!control.touched && !control.dirty) || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return 'To pole jest wymagane';
    }

    if (control.errors['email']) {
      return 'Nieprawidłowy format adresu email';
    }

    if (control.errors['minlength']) {
      if (controlName === 'password' && !this.formSubmitted()) {
        return '';
      }
      const minLength = control.errors['minlength'].requiredLength;
      return `Minimalna długość to ${minLength} znaków`;
    }

    return 'Nieprawidłowa wartość';
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}
