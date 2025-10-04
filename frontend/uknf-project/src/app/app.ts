import { Component, signal, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit, OnDestroy {
  protected readonly title = signal('uknf-project');
  private themeService = inject(ThemeService);
  
  // Session timer
  sessionEndTime = '12:46';
  private timerInterval: any;
  
  // Font size control
  currentFontSize = 'medium'; // 'small', 'medium', 'large'
  
  // High contrast mode
  highContrastMode = false;
  
  // Dark mode
  get darkMode(): boolean {
    return this.themeService.isDarkMode();
  }
  
  // User info
  userName = 'Jan Nowak';
  userRole = 'Użytkownik podmiotu';
  
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
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
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
    this.currentFontSize = size;
    const root = document.documentElement;
    switch (size) {
      case 'small':
        root.style.fontSize = '14px';
        break;
      case 'medium':
        root.style.fontSize = '16px';
        break;
      case 'large':
        root.style.fontSize = '18px';
        break;
    }
  }

  toggleContrast() {
    this.highContrastMode = !this.highContrastMode;
    document.body.classList.toggle('high-contrast', this.highContrastMode);
  }

  toggleDarkMode() {
    this.themeService.toggleTheme();
  }

  logout() {
    console.log('Logout clicked');
    // TODO: Implement logout functionality
  }
}
