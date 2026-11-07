import { Component, inject, OnInit, ChangeDetectorRef, ViewChild, ElementRef} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe ,DecimalPipe } from '@angular/common';
import { jsPDF } from 'jspdf';
import { OrdersService, OrderDto, TodayOrderStatsDto } from '../services/orders';
import { SettingsService, ReceiptSettings } from '../services/settings';
@Component({
  selector: 'app-orders',
  imports: [FormsModule, DatePipe,DecimalPipe],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class Orders implements OnInit {

  private ordersService = inject(OrdersService);
  private cdr = inject(ChangeDetectorRef);

  private settingsService = inject(SettingsService);

  cafeSettings: ReceiptSettings | null = null;
  // Orders
  orders: OrderDto[] = [];
  todayStats: TodayOrderStatsDto[] = [];

  // Loading and Error
  isLoading = false;
  errorMessage = '';

  // Search
  searchText = '';

  // filter by : user & date

  selectedCashier = '';
  selectedStartDate = '';
  selectedEndDate = '';
  // Order Details
  selectedOrder: OrderDto | null = null;


  @ViewChild('orderDetails')
orderDetails!: ElementRef<HTMLElement>;
  // Lifecycle
  ngOnInit(): void {
    this.loadOrders();
    this.loadTodayStats();
     // Load Cafe Settings
  this.settingsService.getReceiptSettings().subscribe({
    next: (data) => {
      this.cafeSettings = data;
      console.log('Cafe Settings loaded for Orders Invoice:', data);
      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error loading Cafe Settings:', error);
    }
  });
  }


  //
  get todayOrdersCount(): number {
    return this.todayStats.reduce(
      (total, stat) => total + stat.ordersCount,
      0
    );
  }
  
  //
  get todayRevenue(): number {
    return this.todayStats.reduce(
      (total, stat) => total + stat.revenue,
      0
    );
  }
  // Methode N°1 : Load All Orders
  loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';
  
    this.ordersService.getOrders().subscribe({
      next: (response) => {
        this.orders = response;
        console.log('Orders loaded successfully:', response);
  
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading orders:', error);
  
        this.errorMessage = 'Failed to load orders. Please try again.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // 
  get cashiers(): string[] {
    return [...new Set(
      this.orders.map(order => order.cashierName)
    )].sort();
  }
  // Methode N°2 : Search Orders
  get filteredOrders(): OrderDto[] {
    const search = this.searchText.trim().toLowerCase();
  
    return this.orders.filter(order => {
  
      // Search by Order ID or Cashier
      const matchesSearch =
        order.orderId.toString().includes(search) ||
        order.cashierName.toLowerCase().includes(search);
  
      // Filter by Cashier
      const matchesCashier =
        !this.selectedCashier ||
        order.cashierName === this.selectedCashier;
  
      // Filter by Date
      const orderDate = new Date(order.orderDate)
      .toLocaleDateString('en-CA');
    
    const matchesDate =
      (!this.selectedStartDate || orderDate >= this.selectedStartDate) &&
      (!this.selectedEndDate || orderDate <= this.selectedEndDate);
      return matchesSearch && matchesCashier && matchesDate;
    });
  }

  // Methode N°3 : Calculate Total Revenue
  get totalRevenue(): number {
    return this.orders.reduce(
      (sum, order) => sum + order.totalAmount,
      0
    );
  }

  // Methode N°4 : View Order Details
  viewOrderDetails(order: OrderDto): void {
    this.selectedOrder = order;
  
    setTimeout(() => {
      this.orderDetails?.nativeElement.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
      });
    }, 100);
  }

  // Methode N°5 : Close Order Details
  closeOrderDetails(): void {
    this.selectedOrder = null;
  }

  // Methode N°6 : Print Order
  // Methode : Print Order
printOrder(order: OrderDto): void {
  if (!this.cafeSettings) {
    alert('Cafe settings are still loading. Please try again.');
    return;
  }

  const estimatedHeight = 100 + order.items.length * 18;
    const pdf = new jsPDF({
    orientation: 'portrait',
    unit: 'mm',
    format: [80, estimatedHeight]
  });

  let y = 10;
  const left = 5;
  const right = 75;

  // Header
 // Dynamic Cafe Header
const settings = this.cafeSettings;

pdf.setFont('helvetica', 'bold');
pdf.setFontSize(16);

const cafeNameLines = pdf.splitTextToSize(
  settings.cafeName,
  68
);

pdf.text(cafeNameLines, 40, y, {
  align: 'center'
});

y += cafeNameLines.length * 6;

pdf.setFont('helvetica', 'normal');
pdf.setFontSize(9);

// Cafe Address
if (settings.address.trim()) {
  const addressLines = pdf.splitTextToSize(
    settings.address,
    68
  );

  pdf.text(addressLines, 40, y, {
    align: 'center'
  });

  y += addressLines.length * 4;
}

// Cafe Phone
if (settings.phoneNumber.trim()) {
  pdf.text(settings.phoneNumber, 40, y, {
    align: 'center'
  });

  y += 5;
}

y += 3;
  pdf.setFontSize(10);
  pdf.setFont('helvetica', 'bold');
  pdf.text(`ORDER #${order.orderId}`, 40, y, { align: 'center' });

  y += 8;
  pdf.setDrawColor(120);
  pdf.line(left, y, right, y);

  // Order Information
  y += 7;
  pdf.setFont('helvetica', 'normal');
  pdf.setFontSize(9);

  pdf.text(`Date: ${new Date(order.orderDate).toLocaleString()}`, left, y);
  y += 6;

  pdf.text(`Cashier: ${order.cashierName}`, left, y);
  y += 6;

  pdf.text(`Payment: ${order.paymentMethod}`, left, y);

  // Items Header
  y += 8;
  pdf.line(left, y, right, y);
  y += 7;

  pdf.setFont('helvetica', 'bold');
  pdf.text('PRODUCT', left, y);
  pdf.text('QTY', 43, y, { align: 'center' });
  pdf.text('TOTAL', right, y, { align: 'right' });

  y += 4;
  pdf.line(left, y, right, y);
  y += 6;

  // Items
  pdf.setFont('helvetica', 'normal');

  for (const item of order.items) {

    const productLines = pdf.splitTextToSize(item.productName, 35);

    pdf.text(productLines, left, y);

    pdf.text(
      item.quantity.toString(),
      43,
      y,
      { align: 'center' }
    );

    pdf.text(
      `${item.totalPrice.toFixed(2)}`,
      right,
      y,
      { align: 'right' }
    );

    y += Math.max(productLines.length * 5, 6);

    pdf.setFontSize(8);
    pdf.text(
      `Unit price: ${item.unitPrice.toFixed(2)} DT`,
      left,
      y
    );

    pdf.setFontSize(9);
    y += 8;
  }

  // Total
  pdf.line(left, y, right, y);
  y += 9;

  pdf.setFont('helvetica', 'bold');
  pdf.setFontSize(12);

  pdf.text(
    `TOTAL: ${order.totalAmount.toFixed(2)} DT`,
    40,
    y,
    { align: 'center' }
  );

  y += 10;

  // Footer
  // Dynamic Receipt Footer from Settings
pdf.setFont('helvetica', 'normal');
pdf.setFontSize(9);

const footerLines = pdf.splitTextToSize(
  this.cafeSettings!.receiptFooter,
  68
);

pdf.text(footerLines, 40, y, {
  align: 'center'
});

y += footerLines.length * 5;

  // Adjust PDF height to actual content
//const finalHeight = y + 10;

//pdf.internal.pageSize.height = finalHeight;
pdf.autoPrint();
window.open(pdf.output('bloburl'), '_blank');
}

// 
loadTodayStats(): void {
  this.ordersService.getTodayStats().subscribe({
    next: (response) => {
      this.todayStats = response;
      console.log('Today stats loaded:', response);
      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error loading today stats:', error);
    }
  });
}
// 
getHourlyHeight(value: number, type: 'orders' | 'revenue'): number {

  const maxValue = Math.max(
    ...this.todayStats.map(stat =>
      type === 'orders' ? stat.ordersCount : stat.revenue
    ),
    0
  );

  if (maxValue === 0) {
    return 0;
  }

  return (value / maxValue) * 100;
}

}