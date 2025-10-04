import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import App from './App';

describe('App', () => {
  it('renders the project hero heading', () => {
    render(
      <MemoryRouter>
        <App />
      </MemoryRouter>
    );

    expect(
      screen.getByRole('heading', {
        level: 1,
        name: /frontend starter with typescript, react router, and tailwind/i
      })
    ).toBeInTheDocument();
  });

  it('shows CTA link for getting started', () => {
    render(
      <MemoryRouter>
        <App />
      </MemoryRouter>
    );

    expect(screen.getByRole('link', { name: /getting started guide/i })).toBeInTheDocument();
  });
});
