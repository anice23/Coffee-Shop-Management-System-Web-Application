import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { switchMap } from 'rxjs';

// DTOs

// DTO N°1 : Create Order
export interface CreateOrderRequest {
  paymentMethod: 'Cash' | 'Card';
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderResponse {
  orderId: number;
  message: string;
}

// DTO N°2 : Get Orders
export interface OrderItemDto {
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface OrderDto {
  orderId: number;
  cashierName: string;
  orderDate: string;
  totalAmount: number;
  paymentMethod: string;
  items: OrderItemDto[];
}
export interface TodayOrderStatsDto {
  hour: number;
  ordersCount: number;
  revenue: number;
}

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  private http = inject(HttpClient);

  // Methode N°1 : Create Order
  createOrder(request: CreateOrderRequest) {
    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        { withCredentials: true }
      )
      .pipe(
        switchMap(csrf =>
          this.http.post<CreateOrderResponse>(
            'https://localhost:7073/api/Orders',
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

  // Methode N°2 : Get All Orders
  getOrders() {
    return this.http.get<OrderDto[]>(
      'https://localhost:7073/api/Orders',
      {
        withCredentials: true,
      }
    );
  }

  // Methode N°3 : Get Today's Orders Stats
getTodayStats() {
  return this.http.get<TodayOrderStatsDto[]>(
    'https://localhost:7073/api/Orders/today-stats',
    { withCredentials: true }
  );
}
}