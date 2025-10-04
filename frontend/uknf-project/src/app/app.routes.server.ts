import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Static pages - can be prerendered
  {
    path: '',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'dashboard',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'wiadomosci',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'sprawozdania',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'biblioteka',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'komunikaty',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'kartoteka',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'wnioski',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'sprawy',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'admin/**',
    renderMode: RenderMode.Prerender
  },
  // Dynamic routes with parameters - use Client-Side Rendering
  {
    path: 'sprawozdania/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'wiadomosci/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'sprawy/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'biblioteka/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'komunikaty/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'kartoteka/:id',
    renderMode: RenderMode.Client
  },
  {
    path: 'wnioski/:id',
    renderMode: RenderMode.Client
  },
  // Fallback for any other route
  {
    path: '**',
    renderMode: RenderMode.Client
  }
];

