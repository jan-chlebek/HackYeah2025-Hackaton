# UKNF Color Palette Configuration

## Overview
This document describes the color palette configuration for the UKNF Angular project using PrimeNG v20 and Tailwind CSS v3.

## Version Requirements
- **Tailwind CSS**: v3.x (stable PostCSS integration)
- **PrimeNG**: v20.x (uses provider-based configuration)
- **@primeng/themes**: v20.x (required for theming)
- **Angular**: v20.x
- **Node.js**: v22.x

## Color Palette

### Primary Colors
- **Primary Blue**: `#003366` - Main brand color, used for primary actions and headers
- **Accent Blue**: `#0073E6` - Secondary brand color, used for interactive elements and links
- **Light Blue**: `#E6F3FF` - Background tints and hover states

### Neutral Colors
- **White**: `#FFFFFF` - Main background color
- **Light Gray**: `#F5F5F5` - Surface backgrounds, cards
- **Medium Gray**: `#666666` - Secondary text, borders
- **Dark Gray**: `#333333` - Primary text color

## Usage

### 1. Tailwind CSS Classes

The color palette is integrated into Tailwind CSS and can be used with standard Tailwind utilities:

```html
<!-- Background Colors -->
<div class="bg-primary">Primary Blue Background</div>
<div class="bg-accent">Accent Blue Background</div>
<div class="bg-light">Light Blue Background</div>
<div class="bg-uknf-gray-light">Light Gray Background</div>

<!-- Text Colors -->
<p class="text-primary">Primary Blue Text</p>
<p class="text-accent">Accent Blue Text</p>
<p class="text-uknf-gray-dark">Dark Gray Text</p>

<!-- Border Colors -->
<div class="border border-primary">Primary Border</div>
<div class="border-2 border-accent">Accent Border</div>
```

### 2. CSS Custom Properties

Colors are also available as CSS variables for use in component styles:

```css
.custom-component {
  background-color: var(--uknf-primary-blue);
  color: var(--uknf-white);
  border: 1px solid var(--uknf-accent-blue);
}

/* Semantic aliases */
.text-content {
  color: var(--color-text-primary);
  background: var(--color-background);
}
```

### 3. Custom Utility Classes

Additional utility classes are provided for convenience:

```html
<!-- Background utilities -->
<div class="bg-primary">Primary Background</div>
<div class="bg-accent">Accent Background</div>
<div class="bg-light">Light Background</div>
<div class="bg-surface">Surface Background</div>

<!-- Text utilities -->
<p class="text-primary">Primary Text</p>
<p class="text-accent">Accent Text</p>
<p class="text-dark">Dark Text</p>
<p class="text-medium">Medium Gray Text</p>

<!-- Border utilities -->
<div class="border-primary">Primary Border</div>
<div class="border-accent">Accent Border</div>
<div class="border-gray">Gray Border</div>
```

### 4. PrimeNG Components

PrimeNG v20 components automatically use the Aura theme configured in `app.config.ts`:

```html
<!-- Buttons automatically use theme colors -->
<p-button label="Primary Action"></p-button>
<p-button label="Secondary Action" severity="secondary"></p-button>

<!-- Other components inherit the theme -->
<p-card>
  <p-table [value]="data">
    <!-- Table styling uses theme colors -->
  </p-table>
</p-card>
```

**Important**: PrimeNG v20 requires programmatic theme configuration, not CSS imports!

## Implementation Details

### Files Modified/Created

1. **tailwind.config.js** - Tailwind configuration with custom color palette
2. **postcss.config.js** - PostCSS configuration for Tailwind v3
3. **src/styles.css** - Global styles with:
   - PrimeIcons and PrimeFlex imports
   - Tailwind directives
   - CSS custom properties
   - Component overrides
4. **src/app/app.config.ts** - PrimeNG v20 theme configuration

### PrimeNG v20 Theme Integration

**CRITICAL**: PrimeNG v20 uses a provider-based configuration system instead of CSS imports.

#### Configuration in app.config.ts:

```typescript
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

export const appConfig: ApplicationConfig = {
  providers: [
    // ... other providers
    provideAnimationsAsync(),  // Required for PrimeNG animations
    providePrimeNG({
      theme: {
        preset: Aura,  // Using Aura theme preset
        options: {
          darkModeSelector: false,
          cssLayer: false
        }
      }
    })
  ]
};
```

#### Required Package:

```bash
npm install @primeng/themes
```

### Tailwind CSS v3 Integration

Tailwind CSS v3 is used for utility classes. Configuration in `tailwind.config.js` extends the default theme with UKNF colors:

```javascript
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        'primary': {
          DEFAULT: '#003366',
          blue: '#003366',
        },
        'accent': {
          DEFAULT: '#0073E6',
          blue: '#0073E6',
        },
        // ... other colors
      }
    }
  }
}
```

PostCSS configuration (`postcss.config.js`) processes Tailwind:

```javascript
module.exports = {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
}
```

### Integration Summary

The project combines:
- **PrimeNG v20** with Aura theme (provider-based configuration)
- **Tailwind CSS v3** for utilities (PostCSS integration)
- **CSS Custom Properties** for theme variables
- **PrimeFlex** for flexbox/grid utilities

All three systems reference the same UKNF color palette, ensuring visual consistency.

### Accessibility

The color palette includes proper focus states for accessibility:
- Focus outline uses Accent Blue (`#0073E6`) with 2px width
- Sufficient contrast ratios between text and backgrounds
- Hover states provide visual feedback

## Best Practices

1. **Prefer Tailwind classes** for quick styling: `class="bg-primary text-white"`
2. **Use CSS variables** in component styles for consistency
3. **Leverage PrimeNG components** - they're pre-styled with the palette
4. **Test contrast ratios** when combining colors
5. **Use semantic aliases** (e.g., `--color-primary`) for flexible theming

## Examples

### Button Component
```html
<button class="bg-primary text-white hover:bg-accent px-4 py-2 rounded">
  Submit
</button>
```

### Card Component
```html
<div class="bg-surface border border-gray rounded-lg p-6">
  <h2 class="text-primary text-2xl mb-4">Card Title</h2>
  <p class="text-dark">Card content goes here</p>
</div>
```

### PrimeNG Integration
```html
<p-card>
  <ng-template pTemplate="header">
    <div class="bg-light p-4">
      <h3 class="text-primary">Header</h3>
    </div>
  </ng-template>
  <p class="text-dark">Content</p>
</p-card>
```

## Troubleshooting

### CSS @tailwind warnings
The CSS linter may show warnings for `@tailwind` directives. These are expected and won't affect functionality. Tailwind processes these at build time.

### PrimeNG v20 Error: "Could not resolve primeng/resources/..."
**PrimeNG v20 does NOT use CSS imports!**

❌ **Wrong** (old PrimeNG versions):
```css
@import "primeng/resources/themes/aura-light-blue/theme.css";
@import "primeng/resources/primeng.css";
```

✅ **Correct** (PrimeNG v20):
```typescript
// In app.config.ts
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

providers: [
  providePrimeNG({
    theme: { preset: Aura }
  })
]
```

### Tailwind CSS PostCSS Error
If you see: "PostCSS plugin has moved to a separate package"

**Solution**: Use Tailwind CSS v3.x, not v4:
```bash
npm install -D tailwindcss@^3 postcss autoprefixer
```

### Import Order Warnings
CSS `@import` rules must come before `@tailwind` directives:

✅ **Correct order**:
```css
@import "primeicons/primeicons.css";
@import "primeflex/primeflex.css";
@tailwind base;
@tailwind components;
@tailwind utilities;
```

### Colors not showing in Tailwind IntelliSense
Restart the TypeScript/Angular language server or VS Code to refresh IntelliSense. You may need to reload the window after changing `tailwind.config.js`.

### PrimeNG Components Not Styled
Ensure you have:
1. ✅ Installed `@primeng/themes` package
2. ✅ Added `provideAnimationsAsync()` to providers
3. ✅ Added `providePrimeNG()` with theme configuration
4. ✅ Imported component modules where needed

## Resources

- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [PrimeNG Documentation](https://primeng.org/)
- [PrimeFlex Documentation](https://primeflex.org/)
- [CSS Custom Properties (MDN)](https://developer.mozilla.org/en-US/docs/Web/CSS/--*)
