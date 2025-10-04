import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  active?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  template: `
    <aside class="sidebar" [class.sidebar-collapsed]="isCollapsed">
      <nav class="sidebar-nav">
        <ul class="menu-list">
          <li *ngFor="let item of menuItems" class="menu-item">
            <a [routerLink]="item.route" 
               routerLinkActive="active"
               class="menu-link">
              <i [class]="item.icon + ' menu-icon'"></i>
              <span class="menu-label" *ngIf="!isCollapsed">{{ item.label }}</span>
            </a>
          </li>
        </ul>
      </nav>

      <button 
        pButton 
        type="button"
        [icon]="isCollapsed ? 'pi pi-angle-right' : 'pi pi-angle-left'"
        class="p-button-text collapse-btn"
        (click)="toggleSidebar()"
        [title]="isCollapsed ? 'Rozwiń menu' : 'Zwiń menu'">
      </button>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 240px;
      background-color: #f8f9fa;
      border-right: 1px solid #dee2e6;
      display: flex;
      flex-direction: column;
      transition: width 0.3s ease;
      position: relative;
      min-height: calc(100vh - 140px);
    }

    .sidebar-collapsed {
      width: 60px;
    }

    .sidebar-nav {
      flex: 1;
      padding: 1rem 0;
    }

    .menu-list {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .menu-item {
      margin-bottom: 0.25rem;
    }

    .menu-link {
      display: flex;
      align-items: center;
      padding: 0.75rem 1rem;
      color: #495057;
      text-decoration: none;
      transition: all 0.2s ease;
      border-left: 3px solid transparent;
    }

    .menu-link:hover {
      background-color: #e9ecef;
      color: var(--uknf-primary);
    }

    .menu-link.active {
      background-color: rgba(0, 51, 102, 0.1);
      color: var(--uknf-primary);
      border-left-color: var(--uknf-primary);
      font-weight: 600;
    }

    .menu-icon {
      font-size: 1.25rem;
      min-width: 1.5rem;
    }

    .menu-label {
      margin-left: 0.75rem;
      white-space: nowrap;
    }

    .sidebar-collapsed .menu-label {
      display: none;
    }

    .collapse-btn {
      position: absolute;
      bottom: 1rem;
      left: 50%;
      transform: translateX(-50%);
      width: 2rem;
      height: 2rem;
      padding: 0;
      border-radius: 50%;
    }

    @media (max-width: 768px) {
      .sidebar {
        width: 60px;
      }

      .menu-label {
        display: none;
      }
    }
  `]
})
export class SidebarComponent {
  isCollapsed = false;

  menuItems: MenuItem[] = [
    { label: 'Biblioteka - repozytorium plików', icon: 'pi pi-folder', route: '/library' },
    { label: 'Wnioski o dostęp', icon: 'pi pi-file-check', route: '/auth/access-requests' },
    { label: 'Sprawy', icon: 'pi pi-briefcase', route: '/cases' },
    { label: 'Sprawozdawczość', icon: 'pi pi-file', route: '/reports' },
    { label: 'Moje pytania', icon: 'pi pi-question-circle', route: '/faq' },
    { label: 'Baza wiedzy', icon: 'pi pi-book', route: '/faq/manage' }
  ];

  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}
