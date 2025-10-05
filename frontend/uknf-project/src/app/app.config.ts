import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';
import { definePreset } from '@primeng/themes';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

// UKNF Custom Color Preset - Override PrimeNG Aura's default green with UKNF blues
const UknfPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '{sky.50}',
      100: '{sky.100}',
      200: '{sky.200}',
      300: '{sky.300}',
      400: '{sky.400}',
      500: '{sky.500}',
      600: '{sky.600}',
      700: '{sky.700}',
      800: '{sky.800}',
      900: '{sky.900}',
      950: '{sky.950}'
    },
    colorScheme: {
      light: {
        primary: {
          color: '#003366',  // UKNF Primary Blue
          contrastColor: '#ffffff',
          hoverColor: '#0073E6',  // UKNF Accent Blue
          activeColor: '#002952'
        },
        highlight: {
          background: '#E6F3FF',  // UKNF Light Blue
          focusBackground: '#0073E6',
          color: '#003366',
          focusColor: '#ffffff'
        }
      }
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes), 
    provideClientHydration(withEventReplay()),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: UknfPreset,
        options: {
          darkModeSelector: false,
          cssLayer: false
        }
      }
    })
  ]
};
