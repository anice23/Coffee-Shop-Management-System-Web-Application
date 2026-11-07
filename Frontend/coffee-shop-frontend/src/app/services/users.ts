import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { switchMap } from 'rxjs';

export interface UserDto {
  userId: number;
  userName: string;
  fullName: string;
  role: 'Admin' | 'Cashier';
  isActive: boolean;
  isSystemUser: boolean;
}

export interface CreateUserRequest {
  userName: string;
  fullName: string;
  password: string;
  role: 'Admin' | 'Cashier';
}

export interface UpdateUserRequest {
  userName: string;
  fullName: string;
  role: 'Admin' | 'Cashier';
  newPassword?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7073/api/Users';

  // Methode N°1 :Get Users
  getUsers() {
    return this.http.get<UserDto[]>(this.apiUrl, {
      withCredentials: true
    });
  }

  // Methode N°2 :Add User
  createUser(request: CreateUserRequest) {
    return this.http.get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    ).pipe(
      switchMap(csrf =>
        this.http.post(
          this.apiUrl,
          request,
          {
            withCredentials: true,
            headers: new HttpHeaders({
              'X-CSRF-TOKEN': csrf.requestToken
            })
          }
        )
      )
    );
  }

  // Methode N°3 :Update User
  updateUser(userId: number, request: UpdateUserRequest) {
    return this.http.get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    ).pipe(
      switchMap(csrf =>
        this.http.put(
          `${this.apiUrl}/${userId}`,
          request,
          {
            withCredentials: true,
            headers: new HttpHeaders({
              'X-CSRF-TOKEN': csrf.requestToken
            })
          }
        )
      )
    );
  }


  // Methode N°4 :Desactivate User
  deactivateUser(userId: number) {
    return this.http.get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    ).pipe(
      switchMap(csrf =>
        this.http.patch(
          `${this.apiUrl}/${userId}/deactivate`,
          {},
          {
            withCredentials: true,
            headers: new HttpHeaders({
              'X-CSRF-TOKEN': csrf.requestToken
            })
          }
        )
      )
    );
  }
  
  // Methode N°5 :Restore User
  restoreUser(userId: number) {
    return this.http.get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    ).pipe(
      switchMap(csrf =>
        this.http.patch(
          `${this.apiUrl}/${userId}/restore`,
          {},
          {
            withCredentials: true,
            headers: new HttpHeaders({
              'X-CSRF-TOKEN': csrf.requestToken
            })
          }
        )
      )
    );
  }

  // Methode N°6 : Get Inactive Users
  getInactiveUsers() {
    return this.http.get<UserDto[]>(
      `${this.apiUrl}/inactive`,
      { withCredentials: true }
    );
  }

  // Methode N°7
  
  
}