import { Injectable, signal, effect, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'uknf-theme';
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);
  
  // Signal to track current theme
  currentTheme = signal<Theme>(this.getInitialTheme());

  constructor() {
    // Apply theme whenever it changes
    effect(() => {
      const theme = this.currentTheme();
      this.applyTheme(theme);
      this.saveTheme(theme);
    });
  }

  /**
   * Get the initial theme from localStorage or system preference
   */
  private getInitialTheme(): Theme {
    // Only access browser APIs if running in browser
    if (!this.isBrowser) {
      return 'light'; // Default for SSR
    }

    // Check localStorage first
    const savedTheme = localStorage.getItem(this.THEME_KEY) as Theme;
    if (savedTheme) {
      return savedTheme;
    }

    // Check system preference
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }

    return 'light';
  }

  /**
   * Apply the theme to the document
   */
  private applyTheme(theme: Theme): void {
    // Only apply DOM changes in browser
    if (!this.isBrowser) {
      return;
    }

    const body = document.body;
    
    if (theme === 'dark') {
      body.classList.add('dark-theme');
      body.classList.remove('light-theme');
    } else {
      body.classList.add('light-theme');
      body.classList.remove('dark-theme');
    }

    // Set data attribute for easier CSS targeting
    document.documentElement.setAttribute('data-theme', theme);
  }

  /**
   * Save theme preference to localStorage
   */
  private saveTheme(theme: Theme): void {
    // Only save to localStorage in browser
    if (!this.isBrowser) {
      return;
    }

    localStorage.setItem(this.THEME_KEY, theme);
  }

  /**
   * Toggle between light and dark themes
   */
  toggleTheme(): void {
    this.currentTheme.update(current => current === 'light' ? 'dark' : 'light');
  }

  /**
   * Set a specific theme
   */
  setTheme(theme: Theme): void {
    this.currentTheme.set(theme);
  }

  /**
   * Check if dark mode is currently active
   */
  isDarkMode(): boolean {
    return this.currentTheme() === 'dark';
  }
}
