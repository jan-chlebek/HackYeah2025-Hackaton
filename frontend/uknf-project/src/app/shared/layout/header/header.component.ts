import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ButtonModule,
    MenubarModule,
    BadgeModule,
    AvatarModule,
    SelectModule
  ],
  template: `
    <header class="app-header">
      <div class="header-top">
        <div class="flex justify-content-between align-items-center">
          <!-- Logo and Title -->
          <div class="flex align-items-center gap-3">
            <div class="logo-box">
              <span class="logo-text">UKNF</span>
            </div>
            <button pButton type="button" icon="pi pi-bars" class="p-button-text menu-toggle" (click)="toggleMenu()"></button>
            <h1 class="header-title m-0">System Komunikacji z Podmiotami</h1>
          </div>

          <!-- Right Section -->
          <div class="flex align-items-center gap-4">
            <!-- Session Timer -->
            <div class="flex align-items-center gap-2 text-sm">
              <i class="pi pi-clock"></i>
              <span>Koniec sesji za: <strong>{{ sessionTime }}</strong></span>
            </div>

            <!-- Font Size Controls -->
            <div class="flex align-items-center gap-1">
              <button pButton type="button" label="A" class="p-button-text font-size-btn small" (click)="setFontSize('small')"></button>
              <button pButton type="button" label="A" class="p-button-text font-size-btn medium" (click)="setFontSize('medium')"></button>
              <button pButton type="button" label="A" class="p-button-text font-size-btn large" (click)="setFontSize('large')"></button>
            </div>

            <!-- High Contrast Toggle -->
            <button pButton type="button" icon="pi pi-eye" class="p-button-text" (click)="toggleHighContrast()" title="Wysoki kontrast"></button>

            <!-- User Info -->
            <div class="user-info">
              <span class="user-name">{{ currentUser.name }}</span>
              <span class="user-role">| {{ currentUser.role }}</span>
            </div>

            <!-- Logout -->
            <button pButton type="button" label="Wyloguj" class="p-button-sm logout-btn" (click)="logout()"></button>
          </div>
        </div>
      </div>

      <div class="header-bottom">
        <div class="system-info">
          <span>System: / Podmiot: <strong>{{ selectedPodmiot?.name || 'Instytucja Testowa' }}</strong></span>
          <button pButton type="button" icon="pi pi-sync" label="Zmień" class="p-button-sm p-button-text change-podmiot-btn" (click)="changePodmiot()"></button>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .app-header {
      background-color: white;
      border-bottom: 2px solid #003366;
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .header-top {
      padding: 0.75rem 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .header-bottom {
      padding: 0.5rem 1.5rem;
      background-color: #f3f4f6;
    }

    .logo-box {
      background-color: #003366;
      color: white;
      padding: 0.5rem 1rem;
      font-weight: bold;
      font-size: 1.25rem;
      border-radius: 4px;
    }

    .logo-text {
      letter-spacing: 2px;
    }

    .menu-toggle {
      color: #6b7280;
    }

    .header-title {
      font-size: 1.125rem;
      color: #1f2937;
      font-weight: 500;
    }

    .font-size-btn {
      padding: 0.25rem 0.5rem;
      color: #6b7280;
      min-width: 2rem;
    }

    .font-size-btn.small {
      font-size: 0.75rem;
    }

    .font-size-btn.medium {
      font-size: 1rem;
    }

    .font-size-btn.large {
      font-size: 1.25rem;
    }

    .user-info {
      font-size: 0.875rem;
      color: #4b5563;
    }

    .user-name {
      font-weight: 600;
    }

    .user-role {
      margin-left: 0.25rem;
    }

    .logout-btn {
      background-color: #6b7280;
      border-color: #6b7280;
    }

    .logout-btn:hover {
      background-color: #4b5563;
      border-color: #4b5563;
    }

    .system-info {
      display: flex;
      align-items: center;
      gap: 1rem;
      font-size: 0.875rem;
      color: #4b5563;
    }

    .change-podmiot-btn {
      color: #6b7280;
      padding: 0.25rem 0.5rem;
    }

    @media (max-width: 1024px) {
      .header-title {
        display: none;
      }
    }
  `]
})
export class HeaderComponent {
  currentUser = {
    name: 'Jan Nowak',
    role: 'Użytkownik podmiotu'
  };

  selectedPodmiot: any = { name: 'Instytucja Testowa', code: 'TEST001' };
  sessionTime = '12:46';
  currentFontSize = 'medium';

  constructor(private router: Router) {
    this.startSessionTimer();
  }

  toggleMenu(): void {
    // TODO: Implement mobile menu toggle
    console.log('Toggle menu');
  }

  setFontSize(size: 'small' | 'medium' | 'large'): void {
    this.currentFontSize = size;
    // TODO: Implement font size change logic
    console.log('Font size changed to:', size);
  }

  toggleHighContrast(): void {
    // TODO: Implement high contrast toggle
    console.log('Toggle high contrast');
  }

  changePodmiot(): void {
    // TODO: Implement podmiot change dialog
    console.log('Change podmiot');
  }

  logout(): void {
    // TODO: Implement logout logic
    this.router.navigate(['/auth/login']);
  }

  private startSessionTimer(): void {
    // TODO: Implement real session timer
    setInterval(() => {
      // Update session time countdown
    }, 1000);
  }
}
