import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoggingOut = signal(false);

  /**
   * Confirm logout action
   */
  confirmLogout(): void {
    this.isLoggingOut.set(true);
    
    this.authService.logout('User initiated logout').subscribe({
      complete: () => {
        // AuthService already redirects to login, but we ensure it here
        this.router.navigate(['/auth/login']);
      }
    });
  }

  /**
   * Cancel logout and return to previous page
   */
  cancelLogout(): void {
    window.history.back();
  }
}
