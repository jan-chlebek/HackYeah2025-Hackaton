import { TestBed } from '@angular/core/testing';
import { AccessibilityService, FontSize } from './accessibility.service';

describe('AccessibilityService', () => {
  let service: AccessibilityService;
  let localStorageMock: { [key: string]: string };

  beforeEach(() => {
    // Mock localStorage
    localStorageMock = {};
    
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      return localStorageMock[key] || null;
    });
    
    spyOn(localStorage, 'setItem').and.callFake((key: string, value: string) => {
      localStorageMock[key] = value;
    });

    TestBed.configureTestingModule({});
    service = TestBed.inject(AccessibilityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('Font Size', () => {
    it('should have medium as default font size', () => {
      expect(service.fontSize).toBe('medium');
    });

    it('should set font size to small', () => {
      service.setFontSize('small');
      expect(service.fontSize).toBe('small');
      expect(document.documentElement.classList.contains('font-size-small')).toBe(true);
    });

    it('should set font size to large', () => {
      service.setFontSize('large');
      expect(service.fontSize).toBe('large');
      expect(document.documentElement.classList.contains('font-size-large')).toBe(true);
    });

    it('should save font size to localStorage', () => {
      service.setFontSize('large');
      expect(localStorage.setItem).toHaveBeenCalledWith('uknf-font-size', 'large');
    });

    it('should apply CSS custom property', () => {
      service.setFontSize('large');
      const fontSize = document.documentElement.style.getPropertyValue('--base-font-size');
      expect(fontSize).toBe('18px');
    });

    it('should remove old font size class when setting new one', () => {
      service.setFontSize('small');
      expect(document.documentElement.classList.contains('font-size-small')).toBe(true);
      
      service.setFontSize('large');
      expect(document.documentElement.classList.contains('font-size-small')).toBe(false);
      expect(document.documentElement.classList.contains('font-size-large')).toBe(true);
    });
  });

  describe('High Contrast', () => {
    it('should have high contrast disabled by default', () => {
      expect(service.highContrast).toBe(false);
    });

    it('should enable high contrast', () => {
      service.toggleHighContrast();
      expect(service.highContrast).toBe(true);
      expect(document.documentElement.classList.contains('high-contrast')).toBe(true);
    });

    it('should disable high contrast when toggled twice', () => {
      service.toggleHighContrast();
      service.toggleHighContrast();
      expect(service.highContrast).toBe(false);
      expect(document.documentElement.classList.contains('high-contrast')).toBe(false);
    });

    it('should save high contrast state to localStorage', () => {
      service.toggleHighContrast();
      expect(localStorage.setItem).toHaveBeenCalledWith('uknf-high-contrast', 'true');
    });
  });

  describe('Persistence', () => {
    it('should load saved font size from localStorage', () => {
      localStorageMock['uknf-font-size'] = 'large';
      const newService = new AccessibilityService();
      expect(newService.fontSize).toBe('large');
    });

    it('should load saved high contrast state from localStorage', () => {
      localStorageMock['uknf-high-contrast'] = 'true';
      const newService = new AccessibilityService();
      expect(newService.highContrast).toBe(true);
    });

    it('should handle invalid localStorage values gracefully', () => {
      localStorageMock['uknf-font-size'] = 'invalid-size';
      const newService = new AccessibilityService();
      expect(newService.fontSize).toBe('medium'); // Should fallback to default
    });
  });
});
