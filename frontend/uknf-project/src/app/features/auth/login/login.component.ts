import { Component, OnInit, signal, inject, PLATFORM_ID, computed } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../services/theme.service';
import { AccessibilityService } from '../../../services/accessibility.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);
  private themeService = inject(ThemeService);
  private accessibilityService = inject(AccessibilityService);

  loginForm!: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  showPassword = signal(false);

  // Accessibility controls - using shared service
  accessibilityPanelOpen = signal(false);
  
  // Computed signals from accessibility service
  currentFontSize = computed(() => this.accessibilityService.currentFontSize());
  highContrastMode = computed(() => this.accessibilityService.highContrastMode());
  darkMode = computed(() => this.themeService.currentTheme() === 'dark');

  ngOnInit(): void {
    // Initialize login form without validation (mock login)
    this.loginForm = this.fb.group({
      email: [''],
      password: [''],
      rememberMe: [false]
    });
  }

  /**
   * Handle form submission (mock - accepts any values and bypasses authentication)
   */
  onSubmit(): void {
    console.log('Login button clicked - mock login');
    this.isLoading.set(true);
    this.errorMessage.set(null);

    // Simple mock login - just navigate to dashboard after short delay
    setTimeout(() => {
      console.log('Navigating to dashboard...');
      this.isLoading.set(false);
      this.router.navigate(['/dashboard']).then(
        success => console.log('Navigation success:', success),
        error => console.error('Navigation error:', error)
      );
    }, 500);
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.showPassword.update(value => !value);
  }

  /**
   * Toggle accessibility panel
   */
  toggleAccessibilityPanel(): void {
    this.accessibilityPanelOpen.update(value => !value);
  }

  /**
   * Set font size using shared service
   */
  setFontSize(size: 'small' | 'medium' | 'large'): void {
    this.accessibilityService.setFontSize(size);
  }

  /**
   * Toggle high contrast mode using shared service
   */
  toggleContrast(): void {
    this.accessibilityService.toggleHighContrast();
  }

  /**
   * Toggle dark mode using shared service
   */
  toggleDarkMode(): void {
    this.themeService.toggleTheme();
  }

  /**
   * Get form control error message
   */
  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);
    
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return 'To pole jest wymagane';
    }
    
    if (control.errors['email']) {
      return 'Nieprawidłowy format adresu email';
    }
    
    if (control.errors['minlength']) {
      const minLength = control.errors['minlength'].requiredLength;
      return `Minimalna długość to ${minLength} znaków`;
    }

    return 'Nieprawidłowa wartość';
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
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
