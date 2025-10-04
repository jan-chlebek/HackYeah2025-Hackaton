import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { BreadcrumbComponent } from '../breadcrumb/breadcrumb.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    HeaderComponent,
    BreadcrumbComponent,
    FooterComponent
  ],
  template: `
    <div class="main-layout flex flex-column min-h-screen">
      <app-header></app-header>
      <app-breadcrumb></app-breadcrumb>
      
      <main class="flex-1 p-4 bg-light">
        <div class="main-content-container max-w-screen-xl mx-auto">
          <router-outlet></router-outlet>
        </div>
      </main>

      <app-footer></app-footer>
    </div>
  `,
  styles: [`
    .main-layout {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    main {
      flex: 1;
      background-color: var(--uknf-light);
    }

    .main-content-container {
      width: 100%;
      max-width: 1400px;
    }
  `]
})
export class MainLayoutComponent {}
