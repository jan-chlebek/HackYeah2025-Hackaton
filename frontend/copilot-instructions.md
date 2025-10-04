# Copilot Guide – Frontend (Angular)

## Tech baseline
- Framework: **Angular 20.x** with standalone components and strict TypeScript.
- UI stack: **PrimeNG** (default theme as base) layered with **Tailwind CSS** utilities. Use PrimeFlex layout helpers where possible.
- State & data: Prefer feature-store slices with NgRx Signal Store or Component Store. Derive API clients from the shared OpenAPI schema.
- Routing: Segment by business areas (`/dashboard`, `/reports`, `/messages`, `/cases`, `/library`, `/admin`). Protect routes using auth guards backed by JWT claims.

## Patterns to follow
- Start new features with a feature module folder that contains: page component, presentational components, services, models, facade/store, and tests (`*.spec.ts`).
- Use reactive forms with strong typing for every data-entry surface (registration, access requests, podmiot updates). Surface field-level validation and server validation feedback.
- Implement list screens with PrimeNG tables that support: quick search, multi-filter, column pickers, sticky headers, pagination, selection, and export triggers.
- File flows must support chunked uploads, progress indicators, resumable retry, and accessible drag-and-drop. Defer virus scanning to backend hooks.
- Respect WCAG 2.2: keyboard navigation, ARIA landmarks, focus management, high-contrast toggle, and readable typography.
- Provide localized copy hooks ready for i18n (`@ngx-translate/core` or Angular built-in i18n). Wrap strings in translation pipes even if only `pl-PL` exists for now.

## Testing & quality
- Unit test components/services with Jest (or Angular TestBed) and 80%+ coverage for new code. Stub HTTP interactions via MSW or HttpTestingController.
- Add end-to-end scenarios with Cypress covering cross-role journeys (e.g., podmiot uploads report → admin reviews → confirmation message).
- Run `ng lint` and `ng test` before committing. Capture accessibility checks (e.g., `axe-core`) for complex UIs.

## Integration contracts
- DTOs come from generated TypeScript clients (`openapi-generator-cli`). Never hand-roll request/response interfaces unless prototyping.
- Keep API endpoints, query params, and error shapes in sync with backend definitions. For temporary stubs, add TODOs referencing the backend ticket.
- Emit telemetry (AppInsights or OpenTelemetry JS) for key interactions: uploads, message send, admin actions. Log only anonymised metadata.

## Delivery extras
- Update Storybook stories for any reusable component change and document states (empty, loading, error, populated).
- Ensure Tailwind safelist includes any dynamic PrimeNG class names to avoid purge issues.
- When adding assets, optimise SVG/PNG and register them in `angular.json`.
