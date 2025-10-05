import { Injectable, signal, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type FontSize = 'small' | 'medium' | 'large';

@Injectable({
  providedIn: 'root'
})
export class AccessibilityService {
  // Signal to track current font size
  private fontSizeSignal = signal<FontSize>('medium');
  
  // Signal to track high contrast mode
  private highContrastSignal = signal<boolean>(false);

  // Check if we're running in a browser
  private isBrowser: boolean;

  constructor() {
    const platformId = inject(PLATFORM_ID);
    this.isBrowser = isPlatformBrowser(platformId);

    if (this.isBrowser) {
      // Load saved preferences from localStorage
      this.loadPreferences();
      // Apply initial settings
      this.applyFontSize(this.fontSizeSignal());
      this.applyHighContrast(this.highContrastSignal());
    }
  }

  /**
   * Get current font size
   */
  get fontSize(): FontSize {
    return this.fontSizeSignal();
  }

  /**
   * Get current high contrast state
   */
  get highContrast(): boolean {
    return this.highContrastSignal();
  }

  /**
   * Set font size and apply to document
   */
  setFontSize(size: FontSize): void {
    this.fontSizeSignal.set(size);
    this.applyFontSize(size);
    this.savePreferences();
  }

  /**
   * Toggle high contrast mode
   */
  toggleHighContrast(): void {
    const newState = !this.highContrastSignal();
    this.highContrastSignal.set(newState);
    this.applyHighContrast(newState);
    this.savePreferences();
  }

  /**
   * Apply font size to document root
   */
  private applyFontSize(size: FontSize): void {
    if (!this.isBrowser) return;

    const root = document.documentElement;
    
    // Remove existing font size classes
    root.classList.remove('font-size-small', 'font-size-medium', 'font-size-large');
    
    // Add new font size class
    root.classList.add(`font-size-${size}`);
    
    // Set CSS custom property for dynamic scaling
    const fontSizeMap = {
      small: '14px',
      medium: '16px',
      large: '18px'
    };
    
    root.style.setProperty('--base-font-size', fontSizeMap[size]);
  }

  /**
   * Apply high contrast mode to document
   */
  private applyHighContrast(enabled: boolean): void {
    if (!this.isBrowser) return;

    const root = document.documentElement;
    
    if (enabled) {
      root.classList.add('high-contrast');
    } else {
      root.classList.remove('high-contrast');
    }
  }

  /**
   * Load preferences from localStorage
   */
  private loadPreferences(): void {
    if (!this.isBrowser) return;

    try {
      const savedFontSize = localStorage.getItem('uknf-font-size') as FontSize;
      if (savedFontSize && ['small', 'medium', 'large'].includes(savedFontSize)) {
        this.fontSizeSignal.set(savedFontSize);
      }

      const savedHighContrast = localStorage.getItem('uknf-high-contrast');
      if (savedHighContrast !== null) {
        this.highContrastSignal.set(savedHighContrast === 'true');
      }
    } catch (error) {
      console.error('Error loading accessibility preferences:', error);
    }
  }

  /**
   * Save preferences to localStorage
   */
  private savePreferences(): void {
    if (!this.isBrowser) return;

    try {
      localStorage.setItem('uknf-font-size', this.fontSizeSignal());
      localStorage.setItem('uknf-high-contrast', String(this.highContrastSignal()));
    } catch (error) {
      console.error('Error saving accessibility preferences:', error);
    }
  }
}
