
import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { finalize, switchMap } from 'rxjs';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  showPassword: boolean = false;

  username: string = '';
  password: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private router: Router,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  // login() : methode 
  login(): void {
    if (this.isLoading) {
      return;
    }

    if (
      this.username.trim() === '' ||
      this.password.trim() === ''
    ) {
      this.errorMessage = 'Please enter username & password.';
      return;
    }

    this.errorMessage = '';
    this.isLoading = true;

    this.authService
      .getCsrfToken()
      .pipe(
        switchMap(() =>
          this.authService.login({
            userName: this.username.trim(),
            password: this.password,
          })
        ),
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: () => {
          this.errorMessage = '';
          this.router.navigate(['/main']);
        },

        error: (error) => {
          console.log('LOGIN ERROR:', error.status);

          if (error.status === 401) {
            this.errorMessage =
              'Invalid username or password.';
          } else if (error.status === 400 || error.status === 403) {
            this.errorMessage =
              'Security validation failed. Please try again.';
          } else {
            this.errorMessage =
              'Unable to connect to the server. Please try again.';
          }
        },
      });
  }
}
