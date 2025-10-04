```instructions```instructions```instructions

You are assisting with a full-stack web application built using Angular on the frontend, Java (Spring Boot) on the backend, and Microsoft SQL Server (MSSQL) as the database.

You are assisting with a full-stack web application built using Angular on the frontend, Java (Spring Boot) on the backend, and Microsoft SQL Server (MSSQL) as the database.You are assisting with a full-stack web application built using Angular on the frontend, Java (Spring Boot) on the backend, and Microsoft SQL Server (MSSQL) as the database.

**Application Purpose:**

This is a reports validation and entity management system with two-way communication capabilities. The application enables:

- **Reports Validation**: Upload, validate, review, and approve/reject reports with validation rules and feedback

- **Entity Management**: CRUD operations for managing various business entities with proper relationships**Application Purpose:****Application Purpose:**

- **Two-Way Communication**: Real-time or near-real-time communication between users (comments, notifications, status updates, messaging)

This is a reports validation and entity management system with two-way communication capabilities. The application enables:This is a reports validation and entity management system with two-way communication capabilities. The application enables:

**CRITICAL: Security Requirements**

This is a **governance project** that handles sensitive data. ALWAYS prioritize security in every decision:- **Reports Validation**: Upload, validate, review, and approve/reject reports with validation rules and feedback- **Reports Validation**: Upload, validate, review, and approve/reject reports with validation rules and feedback

- Use the most secure implementation methods available

- Never compromise security for convenience or speed- **Entity Management**: CRUD operations for managing various business entities with proper relationships- **Entity Management**: CRUD operations for managing various business entities with proper relationships

- Follow OWASP Top 10 security best practices

- Implement defense in depth at every layer- **Two-Way Communication**: Real-time or near-real-time communication between users (comments, notifications, status updates, messaging)- **Two-Way Communication**: Real-time or near-real-time communication between users (comments, notifications, status updates, messaging)

- Assume all input is malicious until proven otherwise

- Apply principle of least privilege for all access controls



Follow these guidelines whenever you generate code or suggest changes:Follow these guidelines whenever you generate code or suggest changes:Follow these guidelines whenever you generate code or suggest changes:



General style

- Write idiomatic TypeScript with strict typing (no `any`). Use interfaces for complex props and data models.

- Follow Angular style guide conventions: use PascalCase for classes, camelCase for properties/methods, and kebab-case for file names.General styleGeneral style

- Keep files focused; extract helpers/services when they exceed ~100 lines or mix responsibilities.

- Use dependency injection consistently throughout the application.- Write idiomatic TypeScript with strict typing (no `any`). Use interfaces for complex props and data models.



Angular patterns- Follow Angular style guide conventions: use PascalCase for classes, camelCase for properties/methods, and kebab-case for file names.General style- Write idiomatic TypeScript with strict typing (no `any`). Use interfaces or `type` aliases for complex props and data models.

- Use standalone components by default (Angular 15+) unless the project structure indicates otherwise.

- Organize by feature modules with clear separation of concerns (components, services, models, guards).- Keep files focused; extract helpers/services when they exceed ~100 lines or mix responsibilities.

- Use Angular Router with lazy loading for feature modules. Define routes with proper guards and resolvers.

- Apply reactive forms (`FormGroup`, `FormControl`) for complex forms; template-driven forms only for simple cases.- Use dependency injection consistently throughout the application.- Write idiomatic TypeScript with strict typing (no `any`). Use interfaces for complex props and data models.- Prefer function components with hooks. Wrap stateful logic in custom hooks when reusable.

- Use RxJS operators properly: prefer `async` pipe in templates, unsubscribe in `ngOnDestroy` when manual subscriptions are needed.

- Implement change detection strategy `OnPush` when appropriate for performance optimization.

- For two-way communication features, consider WebSocket integration or Server-Sent Events (SSE) for real-time updates.

- Implement file upload components with progress tracking and validation feedback for reports.Angular patterns- Follow Angular style guide conventions: use PascalCase for classes, camelCase for properties/methods, and kebab-case for file names.- Use named exports unless a single default export is clearly better.



Angular security- Use standalone components by default (Angular 15+) unless the project structure indicates otherwise.

- Implement proper authentication guards on all protected routes.

- Use Angular's built-in XSS protection (avoid bypassing security contexts unless absolutely necessary).- Organize by feature modules with clear separation of concerns (components, services, models, guards).- Keep files focused; extract helpers/services when they exceed ~100 lines or mix responsibilities.- Keep files focused; extract helpers/components when they exceed ~80 lines or mix responsibilities.

- Sanitize all user inputs and outputs appropriately.

- Implement CSRF protection for all state-changing operations.- Use Angular Router with lazy loading for feature modules. Define routes with proper guards and resolvers.

- Use HTTP interceptors for consistent security headers (Authorization, Content-Security-Policy).

- Never store sensitive data in localStorage; use secure, httpOnly cookies or session storage with encryption.- Apply reactive forms (`FormGroup`, `FormControl`) for complex forms; template-driven forms only for simple cases.- Use dependency injection consistently throughout the application.

- Validate file uploads: check file types, sizes, and scan for malicious content before processing.

- Use RxJS operators properly: prefer `async` pipe in templates, unsubscribe in `ngOnDestroy` when manual subscriptions are needed.

Services & data

- Use Angular's `HttpClient` for HTTP requests with proper typing for request/response models.- Implement change detection strategy `OnPush` when appropriate for performance optimization.React patterns

- Create service classes for business logic, API calls, and state management.

- Use Angular signals (Angular 16+) for reactive state when available, otherwise stick to RxJS `BehaviorSubject` patterns.- For two-way communication features, consider WebSocket integration or Server-Sent Events (SSE) for real-time updates.

- Implement interceptors for cross-cutting concerns (auth tokens, error handling, logging).

- Handle errors gracefully with typed error responses and user-friendly messages.- Implement file upload components with progress tracking and validation feedback for reports.Angular patterns- Use `createBrowserRouter` / `RouterProvider` or `Routes`/`Route` APIs from React Router v6+. Show nested routing, loaders/actions, and lazy-loaded routes when appropriate.

- Design services for report validation workflows with proper state management.

- Implement communication services using WebSocket or polling mechanisms for real-time updates.

- Always use HTTPS for all API communications.

- Implement request timeout and retry logic with exponential backoff.Services & data- Use standalone components by default (Angular 15+) unless the project structure indicates otherwise.- Embrace suspense + code splitting with `React.lazy` and `Suspense` for larger routes.



Styling- Use Angular's `HttpClient` for HTTP requests with proper typing for request/response models.

- Use Angular component styles (scoped CSS/SCSS) or global styles as appropriate.

- Follow consistent styling patterns; use CSS variables for theming when needed.- Create service classes for business logic, API calls, and state management.- Organize by feature modules with clear separation of concerns (components, services, models, guards).- Manage forms with controlled inputs and accessible labels. Use `useForm`-style helpers only if already present in the repo.

- Prefer semantic HTML with proper accessibility attributes (ARIA labels, roles).

- Apply responsive design principles with mobile-first approach.- Use Angular signals (Angular 16+) for reactive state when available, otherwise stick to RxJS `BehaviorSubject` patterns.



File structure & formatting- Implement interceptors for cross-cutting concerns (auth tokens, error handling, logging).- Use Angular Router with lazy loading for feature modules. Define routes with proper guards and resolvers.- Favor memoization hooks (`useMemo`, `useCallback`) only when profiling or prop drilling demands it.

- Place components under feature directories (e.g., `src/app/features/`, `src/app/shared/`).

- Keep services in feature-specific directories or shared services folder.- Handle errors gracefully with typed error responses and user-friendly messages.

- Co-locate tests next to components/services (`component.spec.ts`, `service.spec.ts`).

- Follow consistent import ordering: Angular core, third-party, app modules, relative imports.- Design services for report validation workflows with proper state management.- Apply reactive forms (`FormGroup`, `FormControl`) for complex forms; template-driven forms only for simple cases.

- Use barrel exports (`index.ts`) for cleaner imports in larger modules.

- Implement communication services using WebSocket or polling mechanisms for real-time updates.

Testing & verification

- Write unit tests using Jasmine/Karma or Jest (check project configuration).- Use RxJS operators properly: prefer `async` pipe in templates, unsubscribe in `ngOnDestroy` when manual subscriptions are needed.State & data

- Test components with Angular Testing utilities (`TestBed`, `ComponentFixture`).

- Mock dependencies properly; test user interactions and async operations.Styling

- Include edge cases: empty states, loading states, error handling, form validation.

- Include security test cases: unauthorized access, XSS attempts, CSRF validation, input validation bypass attempts.- Use Angular component styles (scoped CSS/SCSS) or global styles as appropriate.- Implement change detection strategy `OnPush` when appropriate for performance optimization.- For server data, recommend React Query (TanStack Query) patterns if the project already uses it; otherwise stick to `fetch` with proper error/loading handling and typed responses.

- Encourage running `ng test` and `ng lint` after significant changes.

- Follow consistent styling patterns; use CSS variables for theming when needed.

Java backend (Spring Boot)

- Target Java 17+ and Spring Boot 3+ unless otherwise specified.- Prefer semantic HTML with proper accessibility attributes (ARIA labels, roles).- Store global client state with context or lightweight state libraries already in the workspace; avoid introducing heavy state managers unless requested.

- Use Spring Data JPA for database operations with proper entity relationships.

- Keep controllers thin (REST endpoints only); move business logic to service layer.- Apply responsive design principles with mobile-first approach.

- Use `@RestController` for RESTful APIs with proper HTTP method annotations.

- Implement DTOs (Data Transfer Objects) for request/response to separate API contracts from domain models.Services & data

- Use constructor injection for dependencies (avoid field injection).

- Apply proper exception handling with `@ControllerAdvice` and custom exception classes.File structure & formatting

- Implement validation using Bean Validation annotations (`@Valid`, `@NotNull`, etc.).

- Configure CORS properly for Angular frontend integration with specific origins (never use wildcard `*` in production).- Place components under feature directories (e.g., `src/app/features/`, `src/app/shared/`).- Use Angular's `HttpClient` for HTTP requests with proper typing for request/response models.Tailwind CSS v4

- Use `application.properties` or `application.yml` for configuration with environment-specific profiles.

- Implement WebSocket endpoints using Spring WebSocket for real-time communication features.- Keep services in feature-specific directories or shared services folder.

- Create service layer for report validation logic with proper business rules and workflow management.

- Design entity relationships carefully for reports, validations, comments, and audit trails.- Co-locate tests next to components/services (`component.spec.ts`, `service.spec.ts`).- Create service classes for business logic, API calls, and state management.- Use React-compatible class syntax (no `class`). Tailwind tokens should be composed, not inlined style objects.



Java backend security- Follow consistent import ordering: Angular core, third-party, app modules, relative imports.

- Implement Spring Security with JWT or OAuth2 for authentication and authorization.

- Use BCrypt or Argon2 for password hashing (never store plain text passwords).- Use barrel exports (`index.ts`) for cleaner imports in larger modules.- Use Angular signals (Angular 16+) for reactive state when available, otherwise stick to RxJS `BehaviorSubject` patterns.- Encourage semantic HTML. Use utility classes for layout, spacing, color, typography, and responsive design. When patterns repeat, suggest small wrapper components or `cn()` helpers (if available).

- Implement role-based access control (RBAC) with fine-grained permissions.

- Apply method-level security annotations (`@PreAuthorize`, `@Secured`) on sensitive operations.

- Validate and sanitize all user inputs server-side (never trust client-side validation alone).

- Use parameterized queries or JPA/Hibernate to prevent SQL injection (never concatenate SQL strings).Testing & verification- Implement interceptors for cross-cutting concerns (auth tokens, error handling, logging).- Apply Tailwind’s design tokens consistently; avoid mixing Tailwind with raw CSS except in `globals.css` or when utility-first approach fails.

- Implement rate limiting on API endpoints to prevent abuse and DDoS attacks.

- Use secure session management with proper timeout configurations.- Write unit tests using Jasmine/Karma or Jest (check project configuration).

- Log all security-relevant events (authentication attempts, authorization failures, data access) for audit trails.

- Implement Content Security Policy (CSP) headers.- Test components with Angular Testing utilities (`TestBed`, `ComponentFixture`).- Handle errors gracefully with typed error responses and user-friendly messages.

- Use HTTPS only; configure HSTS (HTTP Strict Transport Security).

- Protect against CSRF with Spring Security's CSRF protection enabled.- Mock dependencies properly; test user interactions and async operations.

- Implement file upload security: validate file types, scan for malware, use virus scanning APIs.

- Never expose sensitive information in error messages or stack traces to clients.- Include edge cases: empty states, loading states, error handling, form validation.File structure & formatting

- Use Spring's `@Validated` and custom validators for complex business rule validation.

- Encourage running `ng test` and `ng lint` after significant changes.

Database (MSSQL)

- Use SQL Server-compatible data types and syntax.Styling- Place components under coherent directories (e.g., `src/components`, `src/routes`, `src/features`). Co-locate tests next to components (`Component.test.tsx`).

- Define JPA entities with proper annotations for SQL Server specifics (e.g., identity columns).

- Use Hibernate as JPA provider with SQL Server dialect.Java backend (Spring Boot)

- Create database migrations using Flyway or Liquibase for version control.

- Optimize queries with proper indexing strategies.- Target Java 17+ and Spring Boot 3+ unless otherwise specified.- Use Angular component styles (scoped CSS/SCSS) or global styles as appropriate.- Adhere to existing lint/prettier rules; maintain consistent import ordering (third-party, aliases, relatives).

- Use connection pooling (HikariCP) with appropriate configuration.

- Document database schema changes and include migration scripts.- Use Spring Data JPA for database operations with proper entity relationships.

- Design schema for reports (metadata, content, validation status, audit logs).

- Create proper relationships between entities (reports, users, comments, validations).- Keep controllers thin (REST endpoints only); move business logic to service layer.- Follow consistent styling patterns; use CSS variables for theming when needed.- When creating route modules, export both the component and associated loader/action handlers from the same file.

- Implement audit columns (created_at, updated_at, created_by, updated_by) for tracking changes.

- Use `@RestController` for RESTful APIs with proper HTTP method annotations.

Database security

- Use database user accounts with minimum required privileges (principle of least privilege).- Implement DTOs (Data Transfer Objects) for request/response to separate API contracts from domain models.- Prefer semantic HTML with proper accessibility attributes (ARIA labels, roles).

- Never use `sa` or admin accounts in application configurations.

- Enable Transparent Data Encryption (TDE) for data at rest.- Use constructor injection for dependencies (avoid field injection).

- Use encrypted connections (SSL/TLS) for all database communications.

- Implement column-level encryption for highly sensitive data (PII, financial data).- Apply proper exception handling with `@ControllerAdvice` and custom exception classes.- Apply responsive design principles with mobile-first approach.Testing & verification

- Use parameterized queries exclusively through JPA/Hibernate (prevent SQL injection).

- Implement row-level security policies where appropriate for multi-tenant scenarios.- Implement validation using Bean Validation annotations (`@Valid`, `@NotNull`, etc.).

- Enable database audit logging for all DDL and sensitive DML operations.

- Regularly backup database with encrypted backups stored securely.- Configure CORS properly for Angular frontend integration.- When functionality changes, propose or update tests using the project’s current framework (likely React Testing Library + Jest or Vitest).

- Implement database access monitoring and alerting for suspicious activities.

- Use stored procedures for complex operations with proper input validation.- Use `application.properties` or `application.yml` for configuration with environment-specific profiles.

- Never store passwords or API keys in the database without encryption.

- Implement WebSocket endpoints using Spring WebSocket for real-time communication features.File structure & formatting- Include key edge cases: empty states, loading/error states, router navigation, keyboard accessibility.

Documentation & communication

- Explain non-trivial decisions or new utilities in concise comments or JavaDoc/TSDoc when necessary.- Create service layer for report validation logic with proper business rules and workflow management.

- Reference existing project conventions; avoid reinventing patterns if similar ones exist.

- When presenting code, supply only the changed snippets with enough surrounding context unless the user requests full files.- Design entity relationships carefully for reports, validations, comments, and audit trails.- Place components under feature directories (e.g., `src/app/features/`, `src/app/shared/`).- Encourage running available lint/test scripts after significant edits.

- Document security considerations and potential risks in code comments.

- Maintain security documentation for authentication flows, authorization models, and data protection measures.



Output expectationsDatabase (MSSQL)- Keep services in feature-specific directories or shared services folder.

- Default to TypeScript Angular code for frontend and Java Spring Boot code for backend.

- Provide runnable examples (imports, exports, decorators, annotations) unless asked for a partial snippet.- Use SQL Server-compatible data types and syntax.

- If the request is ambiguous, ask clarifying questions before producing code.

- Always consider the full stack implications (frontend ↔ backend ↔ database) when implementing features.- Define JPA entities with proper annotations for SQL Server specifics (e.g., identity columns).- Co-locate tests next to components/services (`component.spec.ts`, `service.spec.ts`).Documentation & communication

- When suggesting any solution, explicitly mention security considerations and trade-offs.

- If a requested approach has security implications, propose a more secure alternative.- Use Hibernate as JPA provider with SQL Server dialect.

```

- Create database migrations using Flyway or Liquibase for version control.- Follow consistent import ordering: Angular core, third-party, app modules, relative imports.- Explain non-trivial decisions or new utilities in concise comments or a short README snippet when necessary.

- Optimize queries with proper indexing strategies.

- Use connection pooling (HikariCP) with appropriate configuration.- Use barrel exports (`index.ts`) for cleaner imports in larger modules.- Reference existing project conventions; avoid reinventing patterns if similar ones exist.

- Document database schema changes and include migration scripts.

- Design schema for reports (metadata, content, validation status, audit logs).- When presenting code, supply only the changed snippets with enough surrounding context for diff-based editors unless the user requests full files.

- Create proper relationships between entities (reports, users, comments, validations).

- Implement audit columns (created_at, updated_at, created_by, updated_by) for tracking changes.Testing & verification



Documentation & communication- Write unit tests using Jasmine/Karma or Jest (check project configuration).Output expectations

- Explain non-trivial decisions or new utilities in concise comments or JavaDoc/TSDoc when necessary.

- Reference existing project conventions; avoid reinventing patterns if similar ones exist.- Test components with Angular Testing utilities (`TestBed`, `ComponentFixture`).- Default to TypeScript React code with Tailwind class names.

- When presenting code, supply only the changed snippets with enough surrounding context unless the user requests full files.

- Mock dependencies properly; test user interactions and async operations.- Provide runnable examples (imports, exports, hooks) unless asked for a partial snippet.

Output expectations

- Default to TypeScript Angular code for frontend and Java Spring Boot code for backend.- Include edge cases: empty states, loading states, error handling, form validation.- If the request is ambiguous, ask clarifying questions before producing code.

- Provide runnable examples (imports, exports, decorators, annotations) unless asked for a partial snippet.

- If the request is ambiguous, ask clarifying questions before producing code.- Encourage running `ng test` and `ng lint` after significant changes.

- Always consider the full stack implications (frontend ↔ backend ↔ database) when implementing features.

```Django backend


Java backend (Spring Boot)- Target Python 3.11+ and Django 5+ unless otherwise specified.

- Target Java 17+ and Spring Boot 3+ unless otherwise specified.- Keep settings environment-driven (use `dotenv`/env vars), prefer SQLite for local dev, and document additional dependencies in `requirements.txt`.

- Use Spring Data JPA for database operations with proper entity relationships.- Organize apps by domain (`api/`, `core/`, etc.) and keep views thin; extract business logic into services when complexity grows.

- Keep controllers thin (REST endpoints only); move business logic to service layer.- Prefer class-based views or DRF when building CRUD APIs; for small endpoints, `JsonResponse`-based function views are fine.

- Use `@RestController` for RESTful APIs with proper HTTP method annotations.- Always add or update Django tests (e.g., `SimpleTestCase`, `TestCase`) covering new endpoints or behaviors and run `python manage.py test`.
- Implement DTOs (Data Transfer Objects) for request/response to separate API contracts from domain models.
- Use constructor injection for dependencies (avoid field injection).
- Apply proper exception handling with `@ControllerAdvice` and custom exception classes.
- Implement validation using Bean Validation annotations (`@Valid`, `@NotNull`, etc.).
- Configure CORS properly for Angular frontend integration.
- Use `application.properties` or `application.yml` for configuration with environment-specific profiles.

Database (MSSQL)
- Use SQL Server-compatible data types and syntax.
- Define JPA entities with proper annotations for SQL Server specifics (e.g., identity columns).
- Use Hibernate as JPA provider with SQL Server dialect.
- Create database migrations using Flyway or Liquibase for version control.
- Optimize queries with proper indexing strategies.
- Use connection pooling (HikariCP) with appropriate configuration.
- Document database schema changes and include migration scripts.

Documentation & communication
- Explain non-trivial decisions or new utilities in concise comments or JavaDoc/TSDoc when necessary.
- Reference existing project conventions; avoid reinventing patterns if similar ones exist.
- When presenting code, supply only the changed snippets with enough surrounding context unless the user requests full files.

Output expectations
- Default to TypeScript Angular code for frontend and Java Spring Boot code for backend.
- Provide runnable examples (imports, exports, decorators, annotations) unless asked for a partial snippet.
- If the request is ambiguous, ask clarifying questions before producing code.
- Always consider the full stack implications (frontend ↔ backend ↔ database) when implementing features.
```
