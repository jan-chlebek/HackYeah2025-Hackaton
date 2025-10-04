import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TimelineModule } from 'primeng/timeline';
import { BadgeModule } from 'primeng/badge';

interface DashboardTile {
  title: string;
  icon: string;
  count: number;
  description: string;
  link: string;
  severity?: 'success' | 'info' | 'warn' | 'danger';
}

interface TimelineEvent {
  date: string;
  time: string;
  title: string;
  description: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    CardModule,
    ButtonModule,
    TagModule,
    TimelineModule,
    BadgeModule
  ],
  template: `
    <div class="dashboard-container">
      <!-- Welcome Panel -->
      <p-card class="welcome-panel mb-4">
        <div class="flex justify-content-between align-items-center">
          <div>
            <h1 class="text-3xl font-bold text-primary m-0 mb-2">Witaj, {{ currentUser.name }}!</h1>
            <p class="text-lg text-gray-700 m-0">
              Rola: <span class="font-semibold">{{ currentUser.role }}</span> | 
              Podmiot: <span class="font-semibold">{{ currentPodmiot.name }}</span>
            </p>
          </div>
          <div class="text-right">
            <p class="text-sm text-gray-600 m-0">Ostatnie logowanie:</p>
            <p class="font-semibold m-0">{{ lastLoginDate }}</p>
          </div>
        </div>
      </p-card>

      <!-- Dashboard Tiles -->
      <div class="grid">
        <div class="col-12 md:col-6 lg:col-4" *ngFor="let tile of dashboardTiles">
          <p-card class="dashboard-tile hover:shadow-4 transition-duration-200 cursor-pointer" 
                  [routerLink]="tile.link">
            <div class="flex align-items-center justify-content-between mb-3">
              <div class="flex align-items-center gap-2">
                <i [class]="tile.icon + ' text-4xl text-primary'"></i>
                <h3 class="text-xl font-semibold m-0">{{ tile.title }}</h3>
              </div>
              <p-tag [value]="tile.count.toString()" 
                     [severity]="tile.severity || 'info'" 
                     [rounded]="true"
                     class="text-lg">
              </p-tag>
            </div>
            <p class="text-gray-600 m-0">{{ tile.description }}</p>
          </p-card>
        </div>
      </div>

      <!-- Recent Activity and Quick Actions -->
      <div class="grid mt-4">
        <!-- Recent Activity Timeline -->
        <div class="col-12 lg:col-8">
          <p-card>
            <ng-template pTemplate="header">
              <div class="px-3 pt-3">
                <h2 class="text-xl font-bold text-primary m-0">
                  <i class="pi pi-clock mr-2"></i>
                  Ostatnie zdarzenia
                </h2>
              </div>
            </ng-template>

            <p-timeline [value]="recentEvents" align="left">
              <ng-template pTemplate="content" let-event>
                <div class="flex flex-column">
                  <div class="flex align-items-center gap-2 mb-1">
                    <i [class]="event.icon + ' text-primary'"></i>
                    <span class="font-semibold">{{ event.title }}</span>
                  </div>
                  <small class="text-gray-600">{{ event.description }}</small>
                  <small class="text-gray-500 mt-1">{{ event.date }} {{ event.time }}</small>
                </div>
              </ng-template>
              <ng-template pTemplate="marker" let-event>
                <span class="flex w-2rem h-2rem align-items-center justify-content-center text-white border-circle z-1"
                      [style.background-color]="event.color">
                  <i [class]="event.icon"></i>
                </span>
              </ng-template>
            </p-timeline>

            <div class="text-center mt-3">
              <button pButton 
                      label="Zobacz wszystkie zdarzenia" 
                      icon="pi pi-arrow-right"
                      class="p-button-text"
                      [routerLink]="['/timeline']">
              </button>
            </div>
          </p-card>
        </div>

        <!-- Quick Actions and Status Panel -->
        <div class="col-12 lg:col-4">
          <!-- Quick Actions -->
          <p-card class="mb-3">
            <ng-template pTemplate="header">
              <div class="px-3 pt-3">
                <h2 class="text-xl font-bold text-primary m-0">
                  <i class="pi pi-bolt mr-2"></i>
                  Szybkie akcje
                </h2>
              </div>
            </ng-template>

            <div class="flex flex-column gap-2">
              <button pButton 
                      label="Złóż sprawozdanie" 
                      icon="pi pi-file-plus"
                      class="p-button-primary w-full justify-content-start"
                      [routerLink]="['/reports/submit']">
              </button>
              <button pButton 
                      label="Nowa wiadomość" 
                      icon="pi pi-envelope"
                      class="p-button-primary w-full justify-content-start"
                      [routerLink]="['/messages/compose']">
              </button>
              <button pButton 
                      label="Nowa sprawa" 
                      icon="pi pi-briefcase"
                      class="p-button-primary w-full justify-content-start"
                      [routerLink]="['/cases/create']">
              </button>
              <button pButton 
                      label="Przeglądaj bibliotekę" 
                      icon="pi pi-folder-open"
                      class="p-button-outlined w-full justify-content-start"
                      [routerLink]="['/library']">
              </button>
            </div>
          </p-card>

          <!-- Security Indicators -->
          <p-card>
            <ng-template pTemplate="header">
              <div class="px-3 pt-3">
                <h2 class="text-xl font-bold text-primary m-0">
                  <i class="pi pi-shield mr-2"></i>
                  Wskaźniki bezpieczeństwa
                </h2>
              </div>
            </ng-template>

            <div class="flex flex-column gap-3">
              <div class="flex justify-content-between align-items-center">
                <span class="text-sm text-gray-700">Ostatnie logowanie:</span>
                <span class="font-semibold text-sm">{{ lastLoginDate }}</span>
              </div>
              <div class="flex justify-content-between align-items-center">
                <span class="text-sm text-gray-700">Zmiana hasła:</span>
                <span class="font-semibold text-sm">{{ passwordChangeDate }}</span>
              </div>
              <div class="flex justify-content-between align-items-center">
                <span class="text-sm text-gray-700">Status konta:</span>
                <p-tag value="Aktywne" severity="success"></p-tag>
              </div>
              <div class="flex justify-content-between align-items-center">
                <span class="text-sm text-gray-700">Aktywne sesje:</span>
                <p-tag value="1" severity="info"></p-tag>
              </div>
            </div>

            <div class="mt-3">
              <button pButton 
                      label="Zmień hasło" 
                      icon="pi pi-key"
                      class="p-button-sm p-button-outlined w-full"
                      [routerLink]="['/profile/change-password']">
              </button>
            </div>
          </p-card>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 0;
    }

    .welcome-panel {
      background: linear-gradient(135deg, var(--uknf-light-blue) 0%, var(--uknf-light) 100%);
    }

    .dashboard-tile {
      height: 100%;
    }

    .dashboard-tile:hover {
      transform: translateY(-2px);
    }

    :host ::ng-deep {
      .p-card-body {
        padding: 1.5rem;
      }

      .p-timeline-event-content {
        padding-bottom: 1rem;
      }

      .p-timeline-event-opposite {
        display: none;
      }
    }
  `]
})
export class DashboardComponent {
  currentUser = {
    name: 'Jan Kowalski',
    role: 'Administrator Podmiotu Nadzorowanego'
  };

  currentPodmiot = {
    name: 'PKO Bank Polski S.A.',
    code: 'PKO001'
  };

  lastLoginDate = '2025-10-04 08:30';
  passwordChangeDate = '2025-09-15';

  dashboardTiles: DashboardTile[] = [
    {
      title: 'Dostępne podmioty',
      icon: 'pi pi-building',
      count: 3,
      description: 'Podmioty, do których masz przypisane uprawnienia',
      link: '/entities',
      severity: 'info'
    },
    {
      title: 'Wnioski o dostęp',
      icon: 'pi pi-file-check',
      count: 2,
      description: 'Oczekujące i rozpatrzone wnioski',
      link: '/auth/access-requests',
      severity: 'warn'
    },
    {
      title: 'Nowe wiadomości',
      icon: 'pi pi-envelope',
      count: 5,
      description: 'Nieprzeczytane wiadomości wymagające uwagi',
      link: '/messages',
      severity: 'danger'
    },
    {
      title: 'Sprawozdania',
      icon: 'pi pi-file',
      count: 8,
      description: 'Do przesłania, w walidacji, zaakceptowane',
      link: '/reports',
      severity: 'info'
    },
    {
      title: 'Tablica ogłoszeń',
      icon: 'pi pi-megaphone',
      count: 3,
      description: 'Nowe komunikaty i ogłoszenia',
      link: '/announcements',
      severity: 'info'
    },
    {
      title: 'Sprawy',
      icon: 'pi pi-briefcase',
      count: 4,
      description: 'Aktywne sprawy administracyjne',
      link: '/cases',
      severity: 'warn'
    }
  ];

  recentEvents: TimelineEvent[] = [
    {
      date: '2025-10-04',
      time: '11:00',
      title: 'Nowy komunikat w tablicy ogłoszeń',
      description: 'Zmiana terminów składania sprawozdań kwartalnych',
      icon: 'pi pi-megaphone',
      color: 'var(--uknf-accent)'
    },
    {
      date: '2025-10-04',
      time: '09:45',
      title: 'Złożono sprawozdanie "RIP Q3 2025"',
      description: 'Sprawozdanie zostało przekazane do walidacji',
      icon: 'pi pi-file-check',
      color: 'var(--uknf-primary)'
    },
    {
      date: '2025-10-03',
      time: '17:30',
      title: 'Zmieniono uprawnienia użytkownika',
      description: 'Anna Nowak - dodano dostęp do sprawozdawczości',
      icon: 'pi pi-user-edit',
      color: 'var(--uknf-medium-gray)'
    },
    {
      date: '2025-10-03',
      time: '14:20',
      title: 'Nowa wiadomość od UKNF',
      description: 'Prośba o uzupełnienie danych rejestrowych',
      icon: 'pi pi-envelope',
      color: 'var(--uknf-accent)'
    },
    {
      date: '2025-10-02',
      time: '10:15',
      title: 'Zakończono sprawę #12345',
      description: 'Zmiana danych rejestrowych - zatwierdzona',
      icon: 'pi pi-check-circle',
      color: '#22c55e'
    }
  ];
}
