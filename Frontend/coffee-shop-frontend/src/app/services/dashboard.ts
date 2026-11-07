
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

// Interface N°1: Daily Revenue
export interface DailyRevenueDto {
  date: string;
  revenue: number;
}

// Interface N°2: Recent Order
export interface RecentOrderDto {
  orderId: number;
  cashierName: string;
  totalAmount: number;
  paymentMethod: string;
  orderDate: string;
}

// Interface N°3: Dashboard Data
export interface DashboardDto {
  todayRevenue: number;
  todayOrders: number;
  totalOrders: number;
  totalProducts: number;
  lowStockProducts: number;
  weeklySales: DailyRevenueDto[];
  recentOrders: RecentOrderDto[];
}

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private http = inject(HttpClient);

  // Get Dashboard Statistics
  getDashboard() {
    return this.http.get<DashboardDto>(
      'https://localhost:7073/api/Dashboard',
      {
        withCredentials: true,
      }
    );
  }
}
