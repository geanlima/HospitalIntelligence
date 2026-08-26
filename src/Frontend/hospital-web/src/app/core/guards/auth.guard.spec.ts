import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AuthService } from '../services/auth.service';
import { authGuard, guestGuard } from './auth.guard';

describe('authGuard', () => {
  it('redirects to login when unauthenticated', () => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: {
            isAuthenticated: () => false
          }
        }
      ]
    });

    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, {} as never)
    );

    expect(String(result)).toContain('login');
  });

  it('allows navigation when authenticated', () => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: {
            isAuthenticated: () => true
          }
        }
      ]
    });

    const result = TestBed.runInInjectionContext(() =>
      authGuard({} as never, {} as never)
    );

    expect(result).toBe(true);
  });
});

describe('guestGuard', () => {
  it('sends authenticated users to dashboard', () => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: {
            isAuthenticated: () => true
          }
        }
      ]
    });

    const result = TestBed.runInInjectionContext(() =>
      guestGuard({} as never, {} as never)
    );

    expect(String(result)).toContain('dashboard');
  });
});
