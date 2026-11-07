import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {ReportsService, DailySalesReportItem, ProductSalesReportItem,  SalesSummaryReport} from '../services/reports';


@Component({
  selector: 'app-reports',
  imports: [FormsModule],
  templateUrl: './reports.html',
  styleUrl: './reports.css',
})
export class Reports implements OnInit {
 
 
  private reportsService = inject(ReportsService);
  private cdr = inject(ChangeDetectorRef);
  reports: DailySalesReportItem[] = [];
  bestSellingProducts: ProductSalesReportItem[] = [];
  salesSummary: SalesSummaryReport | null = null;

  ngOnInit(): void {
    const today = new Date();

    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(today.getDate() - 6);

    this.startDate = this.formatDate(sevenDaysAgo);
    this.endDate = this.formatDate(today);
    this.loadReports();
  }
  loadReports(): void {
    this.salesSummary = null;
    this.bestSellingProducts = [];
    this.reports = [];
    if (!this.startDate || !this.endDate || this.startDate > this.endDate) {
      return;
    }


  this.reportsService.getDailySalesReport(this.startDate, this.endDate);
    this.reportsService
      .getDailySalesReport(this.startDate, this.endDate)
      .subscribe({
        next: (data) => {
          this.reports = data;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Error loading daily reports:', error);
          this.cdr.detectChanges();
        }
      });
  
    this.loadBestSellingProducts();
    this.loadSalesSummary();
  }
  loadSalesSummary(): void {
    this.reportsService
      .getSalesSummary(this.startDate, this.endDate)
      .subscribe({
        next: (data) => {
          this.salesSummary = data;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Error loading sales summary:', error);
          this.cdr.detectChanges();
        }
      });
  }

  //
  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
  
    return `${year}-${month}-${day}`;
  }
  startDate = '';
  endDate = '';

  // 
  onDateFilterChange(): void {
    if (!this.startDate || !this.endDate) {
      return;
    }
  
    if (this.startDate > this.endDate) {
      return;
    }
  
    this.loadReports();
  }

  // 
  loadBestSellingProducts(): void {
    this.reportsService
      .getBestSellingProducts(this.startDate, this.endDate)
      .subscribe({
        next: (data) => {
          this.bestSellingProducts = data;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Error loading best selling products:', error);
          this.cdr.detectChanges();
        }
      });
  }
  get filteredReports(): DailySalesReportItem[] {
    return this.reports.filter(report =>
      report.date >= this.startDate &&
      report.date <= this.endDate
    );
  }

  get totalRevenue(): number {
    return this.salesSummary?.totalRevenue ??
      this.filteredReports.reduce(
        (sum, report) => sum + report.revenue,
        0
      );
  }

  get totalOrders(): number {
    return this.salesSummary?.totalOrders ??
      this.filteredReports.reduce(
        (sum, report) => sum + report.ordersCount,
        0
      );
  }
  get averageRevenuePerOrder(): number {
    return this.salesSummary?.averageOrderValue ??
      (this.totalOrders === 0
        ? 0
        : this.totalRevenue / this.totalOrders);
  }


  get bestSalesDay(): DailySalesReportItem | null {
    if (this.filteredReports.length === 0) return null;

    return this.filteredReports.reduce((best, report) =>
      report.revenue > best.revenue ? report : best
    );
  }
  //
  
get maxDailyRevenue(): number {
  return Math.max(
    1,
    ...this.filteredReports.map(report => report.revenue)
  );
}

}
