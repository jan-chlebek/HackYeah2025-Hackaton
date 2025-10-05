import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, ActivatedRoute, RouterModule } from '@angular/router';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { MenuItem } from 'primeng/api';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, BreadcrumbModule, RouterModule],
  template: `
    <div class="breadcrumb-container">
      <p-breadcrumb [model]="breadcrumbs" [home]="home"></p-breadcrumb>
    </div>
  `,
  styles: [`
    .breadcrumb-container {
      background-color: #f3f4f6;
      padding: 0.75rem 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    :host ::ng-deep {
      .p-breadcrumb {
        background: transparent;
        border: none;
        padding: 0;
      }

      .p-breadcrumb .p-breadcrumb-list li .p-menuitem-link {
        color: #6b7280;
        font-size: 0.875rem;
      }

      .p-breadcrumb .p-breadcrumb-list li .p-menuitem-link:hover {
        color: #003366;
      }

      .p-breadcrumb .p-breadcrumb-list li.p-breadcrumb-chevron {
        color: #9ca3af;
        margin: 0 0.5rem;
      }

      .p-breadcrumb .p-breadcrumb-home {
        color: #6b7280;
      }
    }
  `]
})
export class BreadcrumbComponent implements OnInit {
  breadcrumbs: MenuItem[] = [];
  home: MenuItem = {
    icon: 'pi pi-home',
    routerLink: '/messages',
    label: 'Strona główna'
  };

  private breadcrumbMap: { [key: string]: string } = {
    'messages': 'Wiadomości',
    'reports': 'Sprawozdania',
    'cases': 'Sprawy',
    'wnioski': 'Wnioski o dostęp',
    'library': 'Biblioteka',
    'announcements': 'Komunikaty',
    'faq': 'FAQ',
    'entities': 'Kartoteka Podmiotów',
    'auth': 'Uwierzytelnienie',
    'admin': 'Administracja',
    'contacts': 'Kontakty',
    'list': 'Lista',
    'create': 'Utwórz',
    'details': 'Szczegóły',
    'edit': 'Edytuj',
    'compose': 'Nowa wiadomość',
    'submit': 'Złóż sprawozdanie',
    'corrections': 'Korekty',
    'upload': 'Dodaj plik',
    'ask': 'Zadaj pytanie',
    'manage': 'Zarządzaj',
    'access-requests': 'Wnioski o dostęp',
    'login': 'Logowanie',
    'register': 'Rejestracja',
    'select-entity': 'Wybór podmiotu',
    'users': 'Użytkownicy',
    'roles': 'Role',
    'password-policy': 'Polityka haseł',
    'update': 'Aktualizuj',
    'groups': 'Grupy'
  };

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.updateBreadcrumbs();

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.updateBreadcrumbs();
      });
  }

  private updateBreadcrumbs(): void {
    const urlSegments = this.router.url.split('/').filter(segment => segment);
    this.breadcrumbs = [];

    let currentPath = '';
    urlSegments.forEach((segment, index) => {
      // Skip dynamic route parameters (IDs, GUIDs, etc.)
      if (this.isRouteParameter(segment)) {
        return;
      }

      currentPath += `/${segment}`;
      const label = this.breadcrumbMap[segment] || this.capitalizeFirst(segment);

      this.breadcrumbs.push({
        label: label,
        routerLink: index === urlSegments.length - 1 ? undefined : currentPath
      });
    });
  }

  private isRouteParameter(segment: string): boolean {
    // Check if segment is a GUID, number, or other common ID pattern
    return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(segment) ||
           /^\d+$/.test(segment);
  }

  private capitalizeFirst(str: string): string {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }
}
