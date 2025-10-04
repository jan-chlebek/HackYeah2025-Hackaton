import { Injectable, signal, effect, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type FontSize = 'small' | 'medium' | 'large';

@Injectable({
  providedIn: 'root'
})
export class AccessibilityService {
  private readonly FONT_SIZE_KEY = 'uknf-font-size';
  private readonly HIGH_CONTRAST_KEY = 'uknf-high-contrast';
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);
  
  // Signal to track current font size
  currentFontSize = signal<FontSize>(this.getInitialFontSize());
  
  // Signal to track high contrast mode
  highContrastMode = signal<boolean>(this.getInitialHighContrast());

  constructor() {
    // Apply font size whenever it changes
    effect(() => {
      const fontSize = this.currentFontSize();
      this.applyFontSize(fontSize);
      this.saveFontSize(fontSize);
    });
    
    // Apply high contrast whenever it changes
    effect(() => {
      const highContrast = this.highContrastMode();
      this.applyHighContrast(highContrast);
      this.saveHighContrast(highContrast);
    });
  }

  /**
   * Get the initial font size from localStorage or default
   */
  private getInitialFontSize(): FontSize {
    // Only access browser APIs if running in browser
    if (!this.isBrowser) {
      return 'medium'; // Default for SSR
    }

    // Check localStorage first
    const savedFontSize = localStorage.getItem(this.FONT_SIZE_KEY) as FontSize;
    if (savedFontSize && ['small', 'medium', 'large'].includes(savedFontSize)) {
      return savedFontSize;
    }

    return 'medium';
  }

  /**
   * Get the initial high contrast mode from localStorage or default
   */
  private getInitialHighContrast(): boolean {
    // Only access browser APIs if running in browser
    if (!this.isBrowser) {
      return false; // Default for SSR
    }

    // Check localStorage
    const savedHighContrast = localStorage.getItem(this.HIGH_CONTRAST_KEY);
    return savedHighContrast === 'true';
  }

  /**
   * Apply the font size to the document
   */
  private applyFontSize(fontSize: FontSize): void {
    // Only apply DOM changes in browser
    if (!this.isBrowser) {
      return;
    }

    const root = document.documentElement;
    switch (fontSize) {
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

  /**
   * Apply high contrast mode to the document
   */
  private applyHighContrast(highContrast: boolean): void {
    // Only apply DOM changes in browser
    if (!this.isBrowser) {
      return;
    }

    document.body.classList.toggle('high-contrast', highContrast);
  }

  /**
   * Save font size preference to localStorage
   */
  private saveFontSize(fontSize: FontSize): void {
    // Only save to localStorage in browser
    if (!this.isBrowser) {
      return;
    }

    localStorage.setItem(this.FONT_SIZE_KEY, fontSize);
  }

  /**
   * Save high contrast preference to localStorage
   */
  private saveHighContrast(highContrast: boolean): void {
    // Only save to localStorage in browser
    if (!this.isBrowser) {
      return;
    }

    localStorage.setItem(this.HIGH_CONTRAST_KEY, highContrast.toString());
  }

  /**
   * Set a specific font size
   */
  setFontSize(fontSize: FontSize): void {
    this.currentFontSize.set(fontSize);
  }

  /**
   * Toggle high contrast mode
   */
  toggleHighContrast(): void {
    this.highContrastMode.update(current => !current);
  }

  /**
   * Set high contrast mode
   */
  setHighContrast(enabled: boolean): void {
    this.highContrastMode.set(enabled);
  }

  /**
   * Get current font size
   */
  getFontSize(): FontSize {
    return this.currentFontSize();
  }

  /**
   * Check if high contrast mode is active
   */
  isHighContrastMode(): boolean {
    return this.highContrastMode();
  }
}
