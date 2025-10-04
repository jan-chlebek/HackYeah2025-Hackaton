You are assisting with a full-stack project built using TypeScript/React 18+ (React Router v6+, Tailwind CSS v4) on the frontend and Python 3.11+/Django 5+ on the backend. Follow these guidelines whenever you generate code or suggest changes:

General style
- Write idiomatic TypeScript with strict typing (no `any`). Use interfaces or `type` aliases for complex props and data models.
- Prefer function components with hooks. Wrap stateful logic in custom hooks when reusable.
- Use named exports unless a single default export is clearly better.
- Keep files focused; extract helpers/components when they exceed ~80 lines or mix responsibilities.

React patterns
- Use `createBrowserRouter` / `RouterProvider` or `Routes`/`Route` APIs from React Router v6+. Show nested routing, loaders/actions, and lazy-loaded routes when appropriate.
- Embrace suspense + code splitting with `React.lazy` and `Suspense` for larger routes.
- Manage forms with controlled inputs and accessible labels. Use `useForm`-style helpers only if already present in the repo.
- Favor memoization hooks (`useMemo`, `useCallback`) only when profiling or prop drilling demands it.

State & data
- For server data, recommend React Query (TanStack Query) patterns if the project already uses it; otherwise stick to `fetch` with proper error/loading handling and typed responses.
- Store global client state with context or lightweight state libraries already in the workspace; avoid introducing heavy state managers unless requested.

Tailwind CSS v4
- Use React-compatible class syntax (no `class`). Tailwind tokens should be composed, not inlined style objects.
- Encourage semantic HTML. Use utility classes for layout, spacing, color, typography, and responsive design. When patterns repeat, suggest small wrapper components or `cn()` helpers (if available).
- Apply Tailwind’s design tokens consistently; avoid mixing Tailwind with raw CSS except in `globals.css` or when utility-first approach fails.

File structure & formatting
- Place components under coherent directories (e.g., `src/components`, `src/routes`, `src/features`). Co-locate tests next to components (`Component.test.tsx`).
- Adhere to existing lint/prettier rules; maintain consistent import ordering (third-party, aliases, relatives).
- When creating route modules, export both the component and associated loader/action handlers from the same file.

Testing & verification
- When functionality changes, propose or update tests using the project’s current framework (likely React Testing Library + Jest or Vitest).
- Include key edge cases: empty states, loading/error states, router navigation, keyboard accessibility.
- Encourage running available lint/test scripts after significant edits.

Documentation & communication
- Explain non-trivial decisions or new utilities in concise comments or a short README snippet when necessary.
- Reference existing project conventions; avoid reinventing patterns if similar ones exist.
- When presenting code, supply only the changed snippets with enough surrounding context for diff-based editors unless the user requests full files.

Output expectations
- Default to TypeScript React code with Tailwind class names.
- Provide runnable examples (imports, exports, hooks) unless asked for a partial snippet.
- If the request is ambiguous, ask clarifying questions before producing code.

Django backend
- Target Python 3.11+ and Django 5+ unless otherwise specified.
- Keep settings environment-driven (use `dotenv`/env vars), prefer SQLite for local dev, and document additional dependencies in `requirements.txt`.
- Organize apps by domain (`api/`, `core/`, etc.) and keep views thin; extract business logic into services when complexity grows.
- Prefer class-based views or DRF when building CRUD APIs; for small endpoints, `JsonResponse`-based function views are fine.
- Always add or update Django tests (e.g., `SimpleTestCase`, `TestCase`) covering new endpoints or behaviors and run `python manage.py test`.