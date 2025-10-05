import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Dynamic routes with parameters - use Server rendering
  {
    path: 'auth/access-requests/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'messages/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'reports/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'library/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'cases/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'wnioski/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'announcements/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'contacts/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'faq/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'entities/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'admin/users/:id',
    renderMode: RenderMode.Server
  },
  {
    path: 'admin/roles/:id',
    renderMode: RenderMode.Server
  },
  // Static routes - use Prerender
  {
    path: '**',
    renderMode: RenderMode.Server
  }
];
