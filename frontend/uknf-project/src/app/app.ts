import { Component, signal, OnInit, OnDestroy, inject, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { ThemeService } from './services/theme.service';
import { AuthService } from './core/services/auth.service';
import { AccessibilityService } from './services/accessibility.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  protected readonly title = signal('uknf-project');
  private themeService = inject(ThemeService);
  private authService = inject(AuthService);
  private accessibilityService = inject(AccessibilityService);
  private router = inject(Router);
  
  // Track if current route is auth route
  private currentUrl = signal<string>('');
  
  // Session timer
  sessionEndTime = '12:46';
  private timerInterval: any;
  
  // Font size and accessibility - using shared service
  get currentFontSize(): string {
    return this.accessibilityService.currentFontSize();
  }
  
  // High contrast mode - using shared service
  get highContrastMode(): boolean {
    return this.accessibilityService.highContrastMode();
  }
  
  // Dark mode
  get darkMode(): boolean {
    return this.themeService.isDarkMode();
  }
  
  // Accessibility section toggle (mobile)
  accessibilitySectionOpen = false;
  
  // User info from AuthService
  currentUser = computed(() => this.authService.currentUser());
  isAuthenticated = computed(() => this.authService.isAuthenticated());
  
  get userName(): string {
    return this.currentUser()?.fullName || 'Gość';
  }
  
  get userRole(): string {
    const roles = this.currentUser()?.roles || [];
    if (roles.includes('Administrator systemu')) return 'Administrator systemu';
    if (roles.includes('Pracownik UKNF')) return 'Pracownik UKNF';
    if (roles.includes('Admin Podmiotu')) return 'Administrator podmiotu';
    if (roles.includes('Pracownik Podmiotu')) return 'Użytkownik podmiotu';
    return 'Użytkownik';
  }
  
  // Menu state
  menuOpen = true;
  
  // Navigation items
  menuItems = [
    { 
      iconClass: 'icon-folder', 
      label: 'Biblioteka - repozytorium plików', 
      route: '/biblioteka',
      active: false 
    },
    { 
      iconClass: 'icon-document', 
      label: 'Wnioski o dostęp', 
      route: '/wnioski',
      active: false 
    },
    { 
      iconClass: 'icon-message', 
      label: 'Wiadomości', 
      route: '/wiadomosci',
      active: false 
    },
    { 
      iconClass: 'icon-clipboard', 
      label: 'Sprawy', 
      route: '/sprawy',
      active: false 
    },
    { 
      iconClass: 'icon-chart', 
      label: 'Sprawozdawczość', 
      route: '/sprawozdania',
      active: false 
    },
    { 
      iconClass: 'icon-question', 
      label: 'Moje pytania', 
      route: '/faq',
      active: false 
    },
    { 
      iconClass: 'icon-book', 
      label: 'Baza wiedzy', 
      route: '/komunikaty',
      active: false 
    }
  ];

  ngOnInit() {
    this.startSessionTimer();
    
    // Track route changes to determine if we're on auth routes
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.currentUrl.set(event.urlAfterRedirects);
    });
    
    // Set initial URL
    this.currentUrl.set(this.router.url);
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }
  
  /**
   * Check if current route is an auth route (login, logout, register, etc.)
   */
  isAuthRoute(): boolean {
    const url = this.currentUrl();
    return url.startsWith('/auth');
  }

  startSessionTimer() {
    // Update session timer every minute
    this.timerInterval = setInterval(() => {
      const now = new Date();
      const minutes = now.getMinutes();
      const hours = now.getHours();
      this.sessionEndTime = `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
    }, 60000);
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  setFontSize(size: 'small' | 'medium' | 'large') {
    this.accessibilityService.setFontSize(size);
  }

  toggleContrast() {
    this.accessibilityService.toggleHighContrast();
  }

  toggleDarkMode() {
    this.themeService.toggleTheme();
  }

  toggleAccessibilitySection() {
    this.accessibilitySectionOpen = !this.accessibilitySectionOpen;
  }

  logout() {
    // Navigate to logout confirmation page
    this.router.navigate(['/auth/logout']);
  }
}
