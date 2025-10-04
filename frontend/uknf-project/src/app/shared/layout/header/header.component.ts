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
    <header class="app-header bg-primary-dark text-white shadow-md">
      <div class="flex justify-content-between align-items-center px-4 py-3">
        <!-- Logo and Title -->
        <div class="flex align-items-center gap-3">
          <img src="/assets/images/knf-logo.svg" alt="UKNF Logo" class="h-3rem" />
          <h1 class="text-xl font-bold m-0">Platforma Komunikacyjna UKNF</h1>
        </div>

        <!-- User Info and Actions -->
        <div class="flex align-items-center gap-4">
          <!-- Podmiot Selector -->
          <div class="flex flex-column gap-1">
            <label for="podmiotSelector" class="text-xs opacity-80">Reprezentowany podmiot:</label>
            <p-select
              inputId="podmiotSelector"
              [options]="availablePodmioty"
              [(ngModel)]="selectedPodmiot"
              optionLabel="name"
              placeholder="Wybierz podmiot"
              [style]="{'min-width': '250px'}"
              styleClass="bg-white">
            </p-select>
          </div>

          <!-- Notifications -->
          <button 
            pButton 
            type="button" 
            icon="pi pi-bell" 
            [label]="notificationCount > 0 ? notificationCount.toString() : ''"
            class="p-button-rounded p-button-text p-button-plain"
            pBadge
            [value]="notificationCount > 0 ? notificationCount.toString() : ''"
            severity="danger"
            (click)="showNotifications()">
          </button>

          <!-- User Menu -->
          <div class="flex align-items-center gap-2">
            <p-avatar 
              icon="pi pi-user" 
              shape="circle" 
              [style]="{'background-color': 'var(--uknf-accent)', 'color': 'white'}">
            </p-avatar>
            <div class="flex flex-column">
              <span class="font-semibold text-sm">{{ currentUser.name }}</span>
              <span class="text-xs opacity-80">{{ currentUser.role }}</span>
            </div>
            <button 
              pButton 
              type="button" 
              icon="pi pi-sign-out" 
              class="p-button-rounded p-button-text p-button-plain"
              (click)="logout()"
              title="Wyloguj">
            </button>
          </div>
        </div>
      </div>

      <!-- Main Navigation -->
      <nav class="navigation-bar bg-primary">
        <p-menubar [model]="menuItems" styleClass="border-none bg-primary">
          <ng-template pTemplate="start">
            <i class="pi pi-home text-white mr-2"></i>
          </ng-template>
        </p-menubar>
      </nav>
    </header>
  `,
  styles: [`
    .app-header {
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .navigation-bar {
      border-top: 1px solid rgba(255, 255, 255, 0.1);
    }

    :host ::ng-deep {
      .p-menubar {
        background: var(--uknf-primary);
        border: none;
        border-radius: 0;
        padding: 0.5rem 1rem;
      }

      .p-menubar .p-menubar-root-list > .p-menuitem > .p-menuitem-link {
        color: white;
        padding: 0.75rem 1rem;
      }

      .p-menubar .p-menubar-root-list > .p-menuitem > .p-menuitem-link:hover {
        background: rgba(255, 255, 255, 0.1);
      }

      .p-menubar .p-menubar-root-list > .p-menuitem.p-menuitem-active > .p-menuitem-link {
        background: rgba(255, 255, 255, 0.15);
      }

      .p-select {
        border: 1px solid rgba(255, 255, 255, 0.3);
      }
    }
  `]
})
export class HeaderComponent {
  currentUser = {
    name: 'Jan Kowalski',
    role: 'Administrator Podmiotu'
  };

  selectedPodmiot: any = null;
  availablePodmioty: any[] = [
    { id: 1, name: 'PKO Bank Polski S.A.', code: 'PKO001' },
    { id: 2, name: 'Alior Bank S.A.', code: 'ALI002' },
    { id: 3, name: 'mBank S.A.', code: 'MBK003' }
  ];

  notificationCount = 5;

  menuItems: MenuItem[] = [
    {
      label: 'Pulpit',
      icon: 'pi pi-home',
      routerLink: '/dashboard'
    },
    {
      label: 'Sprawozdania',
      icon: 'pi pi-file',
      routerLink: '/reports'
    },
    {
      label: 'Wiadomości',
      icon: 'pi pi-envelope',
      routerLink: '/messages',
      badge: '3'
    },
    {
      label: 'Sprawy',
      icon: 'pi pi-briefcase',
      routerLink: '/cases'
    },
    {
      label: 'Biblioteka',
      icon: 'pi pi-folder',
      routerLink: '/library'
    },
    {
      label: 'Komunikaty',
      icon: 'pi pi-megaphone',
      routerLink: '/announcements'
    },
    {
      label: 'FAQ',
      icon: 'pi pi-question-circle',
      routerLink: '/faq'
    },
    {
      label: 'Kartoteka',
      icon: 'pi pi-database',
      routerLink: '/entities'
    },
    {
      label: 'Wnioski',
      icon: 'pi pi-file-check',
      routerLink: '/auth/access-requests'
    },
    {
      label: 'Administracja',
      icon: 'pi pi-cog',
      routerLink: '/admin',
      visible: false // Will be shown based on user role
    }
  ];

  constructor(private router: Router) {
    // Initialize selected podmiot
    if (this.availablePodmioty.length > 0) {
      this.selectedPodmiot = this.availablePodmioty[0];
    }
  }

  showNotifications(): void {
    // TODO: Implement notifications panel
    console.log('Show notifications');
  }

  logout(): void {
    // TODO: Implement logout logic
    this.router.navigate(['/auth/login']);
  }
}
