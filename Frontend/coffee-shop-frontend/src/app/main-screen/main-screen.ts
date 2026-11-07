
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { DashboardService, DashboardDto } from '../services/dashboard';
import { AuthService } from '../services/auth';
import { UserDto } from '../services/users';

@Component({
  selector: 'app-main-screen',
  imports: [RouterLink,DatePipe],
  templateUrl: './main-screen.html',
  styleUrl: './main-screen.css',
})
export class MainScreen implements OnInit {
  private dashboardService = inject(DashboardService);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);

  constructor(private router: Router) {}

  dashboard: DashboardDto | null = null;
  userName:string|undefined='';


  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadDashboard();
  }
  // 
  getSaleHeight(revenue: number): number {
    const maxRevenue = Math.max(
      ...((this.dashboard?.weeklySales ?? []).map(s => s.revenue)),
      0
    );
  
    if (maxRevenue === 0) {
      return 0;
    }
  
    return (revenue / maxRevenue) * 100;
  }

  // Load Dashboard Data
  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.isLoading = false;
        this.cdr.detectChanges();

      },
      error: (error) => {
        console.error('Dashboard loading error:', error);
        this.errorMessage = 'Failed to load dashboard data.';
        this.isLoading = false;
        this.cdr.detectChanges();

      },
    });
  }

  logout(): void {
    this.authService.getCsrfToken().subscribe({
      next: () => {
        this.authService.logout().subscribe({
          next: () => {
            this.router.navigate(['/login']);
          },
          error: (error) => {
            console.error('Logout error:', error);
          }
        });
      },
      error: (error) => {
        console.error('CSRF token error:', error);
      }
    });
  }

  get currentUser() {
    this.userName=this.authService.currentUser()?.userName;
    return this.authService.currentUser();
  }

  
  get isAdmin(): boolean {
    return this.currentUser?.role === 'Admin';
  }
}
