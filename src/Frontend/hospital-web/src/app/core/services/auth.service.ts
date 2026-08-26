import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';

import {
  AuthSession,
  LoginRequest,
  LoginResponse
} from '../models/auth.model';

const STORAGE_KEY = 'hi.auth.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly sessionSignal = signal<AuthSession | null>(
    this.readStoredSession()
  );

  readonly session = this.sessionSignal.asReadonly();
  readonly isAuthenticated = computed(() => {
    const session = this.sessionSignal();
    return !!session && !this.isExpired(session);
  });
  readonly username = computed(() => this.sessionSignal()?.username ?? '');
  readonly roles = computed(() => this.sessionSignal()?.roles ?? []);

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>('/auth/login', request).pipe(
      tap(response => {
        const session: AuthSession = {
          accessToken: response.accessToken,
          expiresAtUtc: response.expiresAtUtc,
          username: response.username,
          roles: response.roles
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
        this.sessionSignal.set(session);
      })
    );
  }

  logout(navigateToLogin = true): void {
    localStorage.removeItem(STORAGE_KEY);
    this.sessionSignal.set(null);
    if (navigateToLogin) {
      void this.router.navigateByUrl('/login');
    }
  }

  getAccessToken(): string | null {
    const session = this.sessionSignal();
    if (!session || this.isExpired(session)) {
      return null;
    }
    return session.accessToken;
  }

  private readStoredSession(): AuthSession | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        return null;
      }
      const session = JSON.parse(raw) as AuthSession;
      if (this.isExpired(session)) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return session;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }

  private isExpired(session: AuthSession): boolean {
    return Date.parse(session.expiresAtUtc) <= Date.now();
  }
}
