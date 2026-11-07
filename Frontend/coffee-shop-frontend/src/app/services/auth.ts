
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  message: string;
  expiresIn: number;
  user: {
    userId: number;
    userName: string;
    fullName: string;
    role: string;
  };
}

export interface CsrfResponse {
  requestToken: string;
}

export interface CurrentUser {
  userId: number;
  userName: string;
  fullName: string;
  role: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7073/api/Auth';

  private csrfToken: string | null = null;

  private currentUserSignal = signal<CurrentUser | null>(null);

  currentUser = this.currentUserSignal.asReadonly();

  getCsrfToken(): Observable<CsrfResponse> {
    return this.http
      .get<CsrfResponse>(`${this.apiUrl}/csrf`, {
        withCredentials: true,
      })
      .pipe(
        tap((response) => {
          this.csrfToken = response.requestToken;
        })
      );
  }
  login(request: LoginRequest): Observable<LoginResponse> {
    if (!this.csrfToken) {
      throw new Error('CSRF token has not been initialized.');
    }
  
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      request,
      {
        withCredentials: true,
        headers: {
          'X-CSRF-TOKEN': this.csrfToken,
        },
      }
    ).pipe(
      tap((response) => {
        this.currentUserSignal.set(response.user);
      })
    );
  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.http.get<CurrentUser>(
      `${this.apiUrl}/me`,
      {
        withCredentials: true,
      }
    ).pipe(
      tap((user) => {
        this.currentUserSignal.set(user);
      })
    );
  }


  logout(): Observable<{ message: string }> {
    if (!this.csrfToken) {
      throw new Error('CSRF token has not been initialized.');
    }
  
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/logout`,
      {},
      {
        withCredentials: true,
        headers: {
          'X-CSRF-TOKEN': this.csrfToken,
        },
      }
    ).pipe(
      tap(() => {
        this.currentUserSignal.set(null);
      })
    );
  }


}
