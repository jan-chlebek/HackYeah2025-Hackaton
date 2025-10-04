---
applyTo: "frontend/**/*.ts,frontend/**/*.tsx,frontend/**/*.html,frontend/**/*.css,frontend/**/*.scss,frontend/**/*.json"
---

# Copilot Guide – Frontend (Angular)

## Tech baseline
- Framework: **Angular 20.x** with standalone components and strict TypeScript.
- UI stack: **PrimeNG** (default theme as base) layered with **Tailwind CSS** utilities. Use **PrimeFlex** grid system and flex utilities for responsive layouts.
- State & data: Prefer feature-store slices with NgRx Signal Store or Component Store. Derive API clients from the shared OpenAPI schema.
- Routing: Segment by business areas with role-based guards:
  - `/dashboard` - personalized home (all roles)
  - `/sprawozdania` - reports submission/management (external: Pracownik/Admin Podmiotu; internal: Pracownik UKNF)
  - `/wiadomosci` - messages inbox/sent (all authenticated users)
  - `/sprawy` - administrative cases (all authenticated users)
  - `/biblioteka` - file repository (all authenticated users, UKNF can manage)
  - `/komunikaty` - announcements board (all authenticated users, UKNF can create/edit)
  - `/faq` - Q&A knowledge base (all users, UKNF staff answers)
  - `/kartoteka` - podmiot registry (external: view own podmioty; UKNF/Admin: manage all)
  - `/wnioski` - access requests (external: own requests; UKNF/Admin Podmiotu: review/approve)
  - `/admin` - user/role/password policy management (Administrator systemu only)
  - Protect routes using auth guards backed by JWT claims with role validation
- Locale: Default to `pl-PL` with proper date/number formatting for Polish locale. Use `@ngx-translate/core` or Angular i18n for all user-facing strings.

## Functional Requirements

For detailed UI/UX specifications and feature requirements (Dashboard, Tables, Forms, File Management, Messaging, Reports, Cases, Access Requests, FAQ, Podmiot Management, Admin Features, Status Labels), see:

📄 **[Frontend Functional Requirements](../requirements/frontend-functional-requirements.md)**

This includes all role-based UI customization rules, component specifications, interaction patterns, and visual design requirements.

---

## Patterns to follow

### Component Architecture
- Start new features with a feature module folder that contains: page component, presentational components, services, models, facade/store, and tests (`*.spec.ts`).
- Use reactive forms with strong typing for every data-entry surface (registration, access requests, podmiot updates). Surface field-level validation and server validation feedback.
- Implement error boundary patterns with proper error handling and user-friendly error messages in Polish.

## Testing & quality
- **Unit Tests**: Test components/services with Jest (or Angular TestBed) and 80%+ coverage for new code. Stub HTTP interactions via MSW or HttpTestingController.
- **Integration Tests**: Test feature flows (form submission → API call → state update → UI feedback).
- **E2E Tests**: Add end-to-end scenarios with Cypress covering cross-role journeys (e.g., podmiot uploads report → admin reviews → confirmation message).
- **Accessibility Tests**: Run `axe-core` automated checks on all major views. Test keyboard navigation manually. Verify screen reader compatibility.
- **Visual Regression**: Consider Chromatic or Percy for Storybook component snapshots.
- **Performance**: Monitor bundle size, lazy-load routes, use OnPush change detection where possible. Target LCP < 2.5s, FID < 100ms, CLS < 0.1.
- **Linting**: Run `ng lint` and `ng test` before committing. Fix all errors and warnings.
- **Code Quality**: Maintain consistent formatting (Prettier), follow Angular style guide, avoid code smells.

## Integration contracts
- DTOs come from generated TypeScript clients (`openapi-generator-cli`). Never hand-roll request/response interfaces unless prototyping.
- Keep API endpoints, query params, and error shapes in sync with backend definitions. For temporary stubs, add TODOs referencing the backend ticket.
- Handle API errors gracefully: show `p-toast` for user-facing errors, log technical details to console/telemetry.
- Implement retry logic for transient failures (network errors, 5xx responses).
- **CSRF Protection**: Include CSRF tokens in headers for state-changing requests.
- **JWT Management**: Store tokens securely, refresh proactively before expiration, handle 401 responses with re-authentication flow.
- Emit telemetry (AppInsights or OpenTelemetry JS) for key interactions: uploads, message send, admin actions. Log only anonymised metadata.
- Track Core Web Vitals: LCP, FID, CLS for performance monitoring.

## Performance & Optimization
- **Lazy Loading**: Use route-level code splitting for all major feature modules.
- **Caching**: Implement HTTP interceptor for GET request caching with configurable TTL.
- **Virtual Scrolling**: Use `p-virtualScroller` for very large lists (1000+ items).
- **OnPush Change Detection**: Apply `ChangeDetectionStrategy.OnPush` to presentational components.
- **Bundle Optimization**: 
  - Target initial bundle < 200KB (gzipped)
  - Use webpack-bundle-analyzer to identify bloat
  - Tree-shake unused PrimeNG components
- **Image Optimization**: Use WebP with PNG/JPEG fallback, implement lazy loading for images.
- **Debouncing**: Debounce search inputs (300ms) and resize handlers.
- **Pagination**: Default to 25 items per page, never load unbounded lists.

## Security Patterns
- **Input Sanitization**: Sanitize all user input, especially in WYSIWYG editor outputs.
- **XSS Prevention**: Use Angular's built-in sanitization, avoid `bypassSecurityTrust*` unless absolutely necessary with documented justification.
- **CSRF**: Include anti-forgery tokens in state-changing requests.
- **File Upload Security**: 
  - Validate file types on client (user feedback) and server (enforcement)
  - Scan files for viruses via backend integration hook
  - Limit file sizes and total upload volume per session
- **JWT Security**: 
  - Store tokens in memory or httpOnly cookies (never localStorage)
  - Implement token refresh flow
  - Clear tokens on logout
  - Validate token expiry before each request
- **Content Security Policy**: Configure CSP headers to prevent inline script execution.

## Delivery extras
- Update Storybook stories for any reusable component change and document states (empty, loading, error, populated).
- Document component inputs/outputs with JSDoc comments for better IDE support.
- Ensure Tailwind safelist includes any dynamic PrimeNG class names to avoid purge issues.
- When adding assets, optimise SVG/PNG and register them in `angular.json`.
- Generate or update API documentation when changing DTOs or service contracts.
- Include README updates for new features with screenshots and usage examples.
- Tag releases with semantic versioning (major.minor.patch) and maintain CHANGELOG.md.

## Accessibility & UX

### WCAG 2.2 AA Standards
- Minimum 4.5:1 contrast ratios
- Keyboard navigation
- ARIA landmarks
- Focus management
- High-contrast toggle

### Keyboard Navigation
- Full support for Tab, Enter, Escape, Arrow keys
- Implement focus traps in modals (`p-dialog`)

### Focus Management
- Visible focus indicators
- Logical tab order
- Auto-focus on modal open
- Return focus on close

### Screen Reader Support
- Proper ARIA labels
- Live regions for dynamic content
- Descriptive link text

### High-Contrast Mode
- Provide theme toggle between default and high-contrast PrimeNG themes

### Typography
- Minimum 14px body text
- Clear size hierarchy
- Readable sans-serif fonts (Arial, Lato, Roboto)

### Responsive Design
- Mobile-first approach
- PrimeFlex breakpoints (sm: 576px, md: 768px, lg: 992px, xl: 1200px)

### Additional notes
- Do not use emojis, implement own svg-based icons or PrimeIcons

---

## Localization

- Provide localized copy hooks ready for i18n (`@ngx-translate/core` or Angular built-in i18n)
- Wrap all user-facing strings in translation pipes even if only `pl-PL` exists for now
- Configure Angular locale data for `pl-PL`: dates, numbers, currency (PLN)
- Use Polish terminology consistently: podmiot, sprawozdanie, wniosek, komunikat, etc.

---

## PrimeNG Component Mapping

Use the following PrimeNG components for specific functional requirements:

| Functional Area | PrimeNG Component | Usage |
|-----------------|-------------------|-------|
| Tables & Lists | `p-table` | All list views with sorting, filtering, pagination, export |
| Navigation | `p-breadcrumb` | Contextual navigation on all subpages |
| Dashboard Cards | `p-card` | Kafelki for dashboard tiles |
| Status Labels | `p-tag` | Color-coded status indicators |
| Notifications | `p-badge` | Unread counts, notification counters |
| Messages | `p-toast` | Success/error feedback notifications |
| Dialogs | `p-dialog` | Modals, confirmations, detail views |
| File Upload | `p-fileUpload` | Chunked uploads with progress |
| Rich Text | `p-editor` | WYSIWYG for komunikaty and messages |
| Dropdowns | `p-dropdown`, `p-multiSelect` | Filters, podmiot switcher, role selection |
| Date Pickers | `p-calendar` | Date range filters, expiration dates |
| Progress | `p-progressBar`, `p-progressSpinner` | Upload progress, loading states |
| Menus | `p-menu`, `p-menubar` | Main navigation, contextual actions |
| Timeline | `p-timeline` | Activity feeds, event history |
| Tree | `p-tree`, `p-treeTable` | Folder hierarchies, categorized data |
| Accordion | `p-accordion` | FAQ sections, collapsible panels |
| Tabs | `p-tabView` | Multi-section views, settings panels |
| Chips | `p-chip` | Tags, user selections, filters |
| Rating | `p-rating` | FAQ answer ratings (1-5 stars) |
