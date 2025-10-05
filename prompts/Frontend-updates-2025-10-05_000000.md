# Prompt
ERROR TypeError: Cannot convert object to primitive value
(stack trace omitted for brevity in prompt log header)

# Response
Implemented SSR safety fixes to address PrimeNG isNaN error during server-side rendering:

1. Messages list component
   - Injected PLATFORM_ID and computed `isBrowser` using `isPlatformBrowser`.
   - Wrapped `<p-table>` in `*ngIf="isBrowser"` to prevent PrimeNG table/theme code from executing on the server (root cause of `isNaN` inside theme variable processing).
   - Added an `<ng-template>` SSR fallback displaying a simple loading placeholder.

2. Sidebar component
   - Added PLATFORM_ID / `isBrowser` flag.
   - While not in the browser (SSR), filter out elevated-permission items unconditionally so restricted navigation does not flash for non‑elevated users after hydration (prevents UI mismatch).

3. Rationale
   PrimeNG’s dynamic theming for sortable columns computes numeric style values (isNaN checks) and expects a full browser environment. Under Angular SSR/Vite hydration the theme util received a non-primitive object, triggering `Cannot convert object to primitive value` when coercing to number. Deferring component rendering until client side removes the invalid SSR execution path.

4. Next steps
   - Optionally create a lightweight skeleton table for SSR instead of a plain loading div if SEO of column headers matters.
   - Retest permissions (todo #3) to verify no elevated items leak pre-hydration.

This file logs the prompt and response per project instructions.
