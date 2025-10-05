import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';

interface Announcement {
  date: string;
  topic: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableModule,
    ButtonModule
  ],
  template: `
    <div class="dashboard-container">
      <!-- Tabs Navigation -->
      <div class="tabs-wrapper">
        <div class="tabs-header">
          <button 
            *ngFor="let tab of tabs(); let i = index"
            class="tab-button"
            [class.active]="selectedTab === i"
            (click)="selectTab(i)">
            {{ tab.label }}
          </button>
        </div>

        <div class="tabs-content">
          <!-- Tab 0: Pulpit użytkownika -->
          <div *ngIf="selectedTab === 0" class="tab-panel">
            <div class="tab-content">
              <p class="text-gray-600">Pulpit użytkownika - główny widok z przeglądem aktywności</p>
            </div>
          </div>

          <!-- Tab 1: Wnioski o dostęp -->
          <div *ngIf="selectedTab === 1" class="tab-panel">
            <div class="tab-content">
              <p class="text-gray-600">Zarządzanie wnioskami o dostęp do systemu</p>
            </div>
          </div>

          <!-- Tab 2: Biblioteka - repozytorium plików -->
          <div *ngIf="selectedTab === 2" class="tab-panel">
            <div class="tab-content">
              <!-- Section Header -->
              <div class="section-header mb-4">
                <h2 class="section-title">Komunikaty</h2>
                <div class="action-buttons">
                  <button pButton type="button" label="Podgląd" icon="pi pi-search" class="p-button-sm p-button-outlined mr-2"></button>
                  <button pButton type="button" label="Eksportuj" icon="pi pi-download" class="p-button-sm p-button-outlined"></button>
                </div>
              </div>

              <!-- Search and Filter Section -->
              <div class="search-section mb-3">
                <h3 class="subsection-title">Wyszukiwanie</h3>
                <button pButton type="button" icon="pi pi-angle-down" class="p-button-text p-button-sm filter-toggle"></button>
              </div>

              <!-- Data Table -->
              <p-table 
                [value]="announcements" 
                [rows]="10"
                [paginator]="true"
                [rowsPerPageOptions]="[10, 25, 50]"
                [showCurrentPageReport]="true"
                currentPageReportTemplate="Showing 1 to 10 of 200 entries"
                styleClass="p-datatable-sm">
                
                <ng-template pTemplate="header">
                  <tr>
                    <th>Data publikacji</th>
                    <th>Temat</th>
                  </tr>
                </ng-template>
                
                <ng-template pTemplate="body" let-announcement>
                  <tr>
                    <td>{{ announcement.date }}</td>
                    <td>{{ announcement.topic }}</td>
                  </tr>
                </ng-template>

                <ng-template pTemplate="emptymessage">
                  <tr>
                    <td colspan="2" class="text-center py-4">Brak danych do wyświetlenia</td>
                  </tr>
                </ng-template>
              </p-table>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      background-color: white;
      border: 1px solid #e5e7eb;
      border-radius: 4px;
    }

    .tab-content {
      padding: 1.5rem;
    }

    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-bottom: 1rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .section-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: #1f2937;
      margin: 0;
    }

    .search-section {
      background-color: #f9fafb;
      padding: 1rem;
      border: 1px solid #e5e7eb;
      border-radius: 4px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .subsection-title {
      font-size: 1rem;
      font-weight: 500;
      color: #374151;
      margin: 0;
    }

    .filter-toggle {
      cursor: pointer;
      color: #6b7280;
    }

    .tabs-wrapper {
      background: white;
      border: 1px solid #e5e7eb;
      border-radius: 4px;
      overflow: hidden;
    }

    .tabs-header {
      display: flex;
      background-color: #f3f4f6;
      border-bottom: 2px solid #003366;
    }

    .tab-button {
      padding: 1rem 1.5rem;
      background: transparent;
      border: none;
      color: #4b5563;
      font-size: 0.875rem;
      cursor: pointer;
      transition: all 0.2s ease;
      border-bottom: 3px solid transparent;
      margin-bottom: -2px;
    }

    .tab-button:hover {
      background-color: rgba(0, 51, 102, 0.05);
      color: #003366;
    }

    .tab-button.active {
      background-color: white;
      color: #003366;
      font-weight: 600;
      border-bottom-color: #003366;
    }

    .tabs-content {
      background: white;
    }

    .tab-panel {
      animation: fadeIn 0.2s ease-in;
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }

    :host ::ng-deep {

      .p-datatable .p-datatable-thead > tr > th {
        background-color: #f9fafb;
        color: #374151;
        font-weight: 600;
        border-bottom: 2px solid #e5e7eb;
        padding: 0.75rem;
      }

      .p-datatable .p-datatable-tbody > tr > td {
        padding: 0.75rem;
        border-bottom: 1px solid #e5e7eb;
      }

      .p-datatable .p-datatable-tbody > tr:hover {
        background-color: #f9fafb;
      }

      .p-paginator {
        background-color: white;
        border-top: 1px solid #e5e7eb;
        padding: 0.75rem;
      }
    }
  `]
})
export class DashboardComponent {
  private authService = inject(AuthService);
  
  selectedTab = 2; // Default to "Biblioteka - repozytorium plików" tab
  
  // Example: Check if user has elevated permissions
  hasElevatedPermissions = computed(() => this.authService.hasElevatedPermissions());
  currentUser = computed(() => this.authService.getCurrentUser());
  
  private allTabs = [
    { label: 'Pulpit użytkownika', requiresElevatedPermissions: false },
    { label: 'Wnioski o dostęp', requiresElevatedPermissions: true },
    { label: 'Biblioteka - repozytorium plików', requiresElevatedPermissions: false }
  ];

  // Filter tabs based on permissions
  tabs = computed(() => {
    const hasElevatedPermissions = this.authService.hasElevatedPermissions();
    return this.allTabs.filter(tab => 
      !tab.requiresElevatedPermissions || hasElevatedPermissions
    );
  });

  announcements: Announcement[] = [
    { date: '2025-02-14', topic: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
    { date: '2025-03-21', topic: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.' },
    { date: '2025-02-14', topic: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
    { date: '2025-03-21', topic: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.' },
    { date: '2025-02-14', topic: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
    { date: '2025-03-21', topic: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.' },
    { date: '2025-02-14', topic: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
    { date: '2025-03-21', topic: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.' },
    { date: '2025-02-14', topic: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.' },
    { date: '2025-03-21', topic: 'Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisl ut aliquip ex ea commodo consequat.' }
  ];

  selectTab(index: number): void {
    this.selectedTab = index;
  }
}
