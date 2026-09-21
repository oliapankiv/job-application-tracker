import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';
import { AuthRoute } from '../../../../shared/features/auth/routes/auth.route';
import { AuthConfig } from '../configs/auth.config';

const STORAGE_KEY = 'jobapplicationtracker.auth';

interface StoredAuth {
  token: string;
  expiresAt: string;
  email: string;
  displayName?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authState = signal<StoredAuth | null>(this.loadFromStorage());

  readonly isAuthenticated = computed(() => {
    const auth = this.authState();
    return !!auth && new Date(auth.expiresAt) > new Date();
  });

  readonly currentUser = computed(() => this.authState());

  constructor(private http: HttpClient, private router: Router) {}

  get token(): string | null {
    return this.authState()?.token ?? null;
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}${AuthConfig.REGISTER}`, data).pipe(
      tap((response) => this.setAuth(response))
    );
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}${AuthConfig.LOGIN}`, data).pipe(
      tap((response) => this.setAuth(response))
    );
  }

  logout(): void {
    this.authState.set(null);
    localStorage.removeItem(STORAGE_KEY);
    this.router.navigate([AuthRoute.Login]);
  }

  private setAuth(response: AuthResponse): void {
    const stored: StoredAuth = {
      token: response.token,
      expiresAt: response.expiresAt,
      email: response.email,
      displayName: response.displayName
    };
    this.authState.set(stored);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(stored));
  }

  private loadFromStorage(): StoredAuth | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as StoredAuth) : null;
    } catch {
      return null;
    }
  }
}
