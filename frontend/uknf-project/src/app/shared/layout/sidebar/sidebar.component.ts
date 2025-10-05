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
    <aside class="sidebar">
      <div class="sidebar-header">
        <h2 class="sidebar-title">MENU</h2>
      </div>
      
      <nav class="sidebar-nav">
        <ul class="menu-list">
          <li *ngFor="let item of menuItems" class="menu-item">
            <a [routerLink]="item.route" 
               routerLinkActive="active"
               class="menu-link">
              <i [class]="item.icon + ' menu-icon'"></i>
              <span class="menu-label">{{ item.label }}</span>
            </a>
          </li>
        </ul>
      </nav>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 280px;
      background-color: #f5f5f5;
      border-right: 1px solid #e0e0e0;
      display: flex;
      flex-direction: column;
      position: relative;
      min-height: calc(100vh - 140px);
      flex-shrink: 0;
    }

    .sidebar-header {
      padding: 1.5rem 1rem 1rem 1rem;
      border-bottom: 1px solid #e0e0e0;
      background-color: #f5f5f5;
    }

    .sidebar-title {
      font-size: 0.875rem;
      font-weight: 600;
      color: #666;
      letter-spacing: 0.5px;
      margin: 0;
    }

    .sidebar-nav {
      flex: 1;
      padding: 0.5rem 0;
      overflow-x: hidden;
      overflow-y: auto;
    }

    .menu-list {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .menu-item {
      margin-bottom: 0;
    }

    .menu-link {
      display: flex;
      align-items: center;
      padding: 1rem 1.25rem;
      color: #333;
      text-decoration: none;
      transition: all 0.2s ease;
      border-left: 0;
      min-height: 52px;
      background-color: transparent;
    }

    .menu-link:hover {
      background-color: #e8e8e8;
      color: #003366;
    }

    .menu-link.active {
      background-color: #d4e7f7;
      color: #003366;
      font-weight: 500;
    }

    .menu-icon {
      font-size: 1.5rem;
      min-width: 1.75rem;
      flex-shrink: 0;
      color: #555;
    }

    .menu-link.active .menu-icon {
      color: #003366;
    }

    /* High Contrast Mode Styles */
    :host-context(html.high-contrast) .sidebar {
      background-color: #000000 !important;
      border-right: 2px solid #FFFF00 !important;
    }

    :host-context(html.high-contrast) .sidebar-header {
      background-color: #000000 !important;
      border-bottom: 2px solid #FFFF00 !important;
    }

    :host-context(html.high-contrast) .sidebar-title {
      color: #FFFF00 !important;
    }

    :host-context(html.high-contrast) .menu-link {
      background-color: #000000 !important;
      color: #FFFF00 !important;
      border-left: 0 !important;
    }

    :host-context(html.high-contrast) .menu-link:hover {
      background-color: #FFFF00 !important;
      color: #000000 !important;
    }

    :host-context(html.high-contrast) .menu-link:hover .menu-icon,
    :host-context(html.high-contrast) .menu-link:hover .menu-label {
      color: #000000 !important;
    }

    :host-context(html.high-contrast) .menu-link.active {
      background-color: #FFFF00 !important;
      color: #000000 !important;
      border-left: 0 !important;
    }

    :host-context(html.high-contrast) .menu-link.active .menu-icon,
    :host-context(html.high-contrast) .menu-link.active .menu-label {
      color: #000000 !important;
    }

    /* Keep active menu item yellow when hovering */
    :host-context(html.high-contrast) .menu-link.active:hover {
      background-color: #FFFF00 !important;
      color: #000000 !important;
    }

    :host-context(html.high-contrast) .menu-icon {
      color: #FFFF00 !important;
    }

    .menu-label {
      margin-left: 1rem;
      white-space: normal;
      word-wrap: break-word;
      line-height: 1.4;
      font-size: 0.95rem;
    }

    @media (max-width: 768px) {
      .sidebar {
        width: 60px;
      }

      .menu-label {
        display: none;
      }

      .sidebar-header {
        display: none;
      }
    }
  `]
})
export class SidebarComponent {
  menuItems: MenuItem[] = [
    { label: 'Wiadomości', icon: 'pi pi-envelope', route: '/messages' },
    { label: 'Komunikaty', icon: 'pi pi-megaphone', route: '/announcements' },
    { label: 'Biblioteka - repozytorium plików', icon: 'pi pi-folder-open', route: '/library' },
    { label: 'Wnioski o dostęp', icon: 'pi pi-file', route: '/wnioski' },
    { label: 'Sprawozdawczość', icon: 'pi pi-chart-line', route: '/reports' },
    { label: 'Moje pytania', icon: 'pi pi-question-circle', route: '/faq' },
    { label: 'Kartoteka Podmiotów', icon: 'pi pi-building', route: '/entities' },
    { label: 'Sprawy', icon: 'pi pi-clipboard', route: '/cases' },
    { label: 'Adresaci', icon: 'pi pi-users', route: '/contacts' }
  ];
}
