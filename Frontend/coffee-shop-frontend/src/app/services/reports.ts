import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface DailySalesReportItem {
  date: string;
  ordersCount: number;
  revenue: number;
}

export interface SalesSummaryReport {
  totalOrders: number;
  totalRevenue: number;
  averageOrderValue: number;
  bestSellingProduct: string;
  bestSellingProductQuantity: number;
}

export interface ProductSalesReportItem {
  productName: string;
  totalQuantity: number;
  totalRevenue: number;
}

export interface SalesReportItem {
  orderId: number;
  orderDate: string;
  paymentMethod: string;
  totalAmount: number;
}


@Injectable({
  providedIn: 'root'
})
export class ReportsService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7073/api/Reports';



  // Methode N°1 :
  getDailySalesReport(fromDate: string, toDate: string) {
    return this.http.get<DailySalesReportItem[]>(
      `${this.apiUrl}/daily-sales`,
      {
        params: {
          fromDate: fromDate,
          toDate: toDate
        },
        withCredentials: true
      }
    );
  }

  // Methode N°2 
  getSalesSummary(fromDate: string, toDate: string) {
    return this.http.get<SalesSummaryReport>(
      `${this.apiUrl}/summary`,
      {
        params: {
          fromDate: fromDate,
          toDate: toDate
        },
        withCredentials: true
      }
    );
  }

  // Methode N°3
  getBestSellingProducts(fromDate: string, toDate: string) {
    return this.http.get<ProductSalesReportItem[]>(
      `${this.apiUrl}/best-selling-products`,
      {
        params: {
          fromDate: fromDate,
          toDate: toDate
        },
        withCredentials: true
      }
    );
  }

  // Methode N°4
  getSalesReport(fromDate: string, toDate: string) {
    return this.http.get<SalesReportItem[]>(
      `${this.apiUrl}/sales`,
      {
        params: {
          fromDate: fromDate,
          toDate: toDate
        },
        withCredentials: true
      }
    );
  }

}
@Injectable({
  providedIn: 'root',
})


export class Reports {}
