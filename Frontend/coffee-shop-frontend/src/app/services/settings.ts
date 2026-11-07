
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { switchMap } from 'rxjs';

export interface CafeSettings {
  cafeSettingId: number;
  cafeName: string;
  address: string;
  phoneNumber: string;
  receiptFooter: string;
  lowStockThreshold: number;
}

export interface ReceiptSettings {
  cafeName: string;
  address: string;
  phoneNumber: string;
  receiptFooter: string;
}

export interface UpdateCafeSettingsRequest {
  cafeSettingId: number;
  cafeName: string;
  address: string;
  phoneNumber: string;
  receiptFooter: string;
  lowStockThreshold: number;
}

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7073/api/Settings';

  // Method 1: Get Settings
  getSettings() {
    return this.http.get<CafeSettings>(
      this.apiUrl,
      { withCredentials: true }
    );
  }

  // Method 2: Get Receipt Settings (Admin + Cashier)
    getReceiptSettings() {
     return this.http.get<ReceiptSettings>(
    `${this.apiUrl}/receipt`,
    { withCredentials: true }
  );
}
  // Method 3: Update Settings
  updateSettings(request: UpdateCafeSettingsRequest) {
    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        { withCredentials: true }
      )
      .pipe(
        switchMap(csrf =>
          this.http.put(
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
}
