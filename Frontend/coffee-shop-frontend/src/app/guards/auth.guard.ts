import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {

  const authService = inject(AuthService);
  const router = inject(Router);

  // User already authenticated
  if (authService.currentUser()) {
    return true;
  }

  // Try to restore authentication from JWT Cookie
  return authService.getCurrentUser().pipe(

    map(() => true),

    catchError(() => {
      return of(router.createUrlTree(['/login']));
    })

  );
};