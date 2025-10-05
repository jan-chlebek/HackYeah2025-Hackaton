import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';

export interface LoginPayload {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthenticatedUser {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
}

export interface LoginResponse {
  token: string;
  refreshToken?: string;
  expiresIn?: number;
  user: AuthenticatedUser;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/v1/auth';

  /**
   * Perform login using backend API.
   * For now, simulate a successful response to unblock UI development.
   */
  login(payload: LoginPayload): Observable<LoginResponse> {
    if (!payload.email || !payload.password) {
      throw new Error('Brak wymaganych danych logowania.');
    }

    // Placeholder mocked response. Swap for real HTTP call when backend is ready:
    // return this.http.post<LoginResponse>(`${this.apiUrl}/login`, payload);
    const mockResponse: LoginResponse = {
      token: 'mock-token',
      refreshToken: 'mock-refresh-token',
      expiresIn: 3600,
      user: {
        id: this.generateMockId(),
        email: payload.email,
        displayName: 'Jan Kowalski',
        roles: ['Pracownik Podmiotu']
      }
    };

    return of(mockResponse).pipe(
      delay(600),
      tap(() => {
        // TODO: integrate secure token storage aligned with OAuth2/OIDC once backend ready.
      })
    );
  }

  logout(): void {
    // TODO: integrate logout with backend and session invalidation once API is available.
  }

  private generateMockId(): string {
    const cryptoRef = globalThis.crypto as Crypto | undefined;
    if (cryptoRef?.randomUUID) {
      return cryptoRef.randomUUID();
    }

    return `mock-${Math.random().toString(36).slice(2, 10)}`;
  }
}
