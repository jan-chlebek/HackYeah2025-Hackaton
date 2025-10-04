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
    <div class="breadcrumb-container bg-light-gray px-4 py-2">
      <p-breadcrumb [model]="breadcrumbs" [home]="home"></p-breadcrumb>
    </div>
  `,
  styles: [`
    .breadcrumb-container {
      border-bottom: 1px solid #dee2e6;
    }

    :host ::ng-deep {
      .p-breadcrumb {
        background: transparent;
        border: none;
        padding: 0;
      }

      .p-breadcrumb .p-breadcrumb-list li .p-menuitem-link {
        color: var(--uknf-primary);
      }

      .p-breadcrumb .p-breadcrumb-list li .p-menuitem-link:hover {
        color: var(--uknf-accent);
      }

      .p-breadcrumb .p-breadcrumb-list li.p-breadcrumb-chevron {
        color: var(--uknf-dark-gray);
      }
    }
  `]
})
export class BreadcrumbComponent implements OnInit {
  breadcrumbs: MenuItem[] = [];
  home: MenuItem = {
    icon: 'pi pi-home',
    routerLink: '/dashboard',
    label: 'Pulpit'
  };

  private breadcrumbMap: { [key: string]: string } = {
    'dashboard': 'Pulpit',
    'reports': 'Sprawozdania',
    'messages': 'Wiadomości',
    'cases': 'Sprawy',
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
