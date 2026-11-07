
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SettingsService, CafeSettings } from '../services/settings';

@Component({
  selector: 'app-settings',
  imports: [FormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.css',
})
export class Settings implements OnInit {
  private settingsService = inject(SettingsService);
  private cdr = inject(ChangeDetectorRef);

  isSaving = false;
  showSuccessModal = false;
  showResetModal = false;

  openResetModal(): void {
    this.showResetModal = true;
  }
  
  closeResetModal(): void {
    this.showResetModal = false;
  }
  
  confirmReset(): void {
    this.showResetModal = false;
    this.resetSettings();
  }


  settings: CafeSettings = {
    cafeSettingId: 0,
    cafeName: '',
    address: '',
    phoneNumber: '',
    receiptFooter: '',
    lowStockThreshold: 0
  };

  savedMessage = '';
  errorMessage = '';
  isLoading = false;

  ngOnInit(): void {
    this.loadSettings();
  }

  // Load Settings from API
  
loadSettings(): void {
  this.isLoading = true;
  this.errorMessage = '';

  this.settingsService.getSettings().subscribe({
    next: (data) => {
      console.log('Settings received:', data);

      this.settings = data;
      this.isLoading = false;

      this.cdr.detectChanges();
    },
    error: (error) => {
      console.error('Error loading settings:', error);

      this.errorMessage = 'Failed to load settings.';
      this.isLoading = false;

      this.cdr.detectChanges();
    }
  });
}



saveSettings(): void {
  if (this.isSaving) return;

  if (!this.settings.cafeName.trim()) {
    alert('Please enter the cafe name.');
    return;
  }

  if (
    !Number.isInteger(this.settings.lowStockThreshold) ||
    this.settings.lowStockThreshold < 0
  ) {
    alert('Stock threshold must be a whole number greater than or equal to 0.');
    return;
  }

  const request: CafeSettings = {
    ...this.settings,
    cafeName: this.settings.cafeName.trim(),
    address: this.settings.address.trim(),
    phoneNumber: this.settings.phoneNumber.trim(),
    receiptFooter: this.settings.receiptFooter.trim()
  };

  this.isSaving = true;
  this.errorMessage = '';
  this.savedMessage = '';

  this.settingsService.updateSettings(request).subscribe({
    next: () => {
      this.settings = request;
      this.isSaving = false;
      this.showSuccessModal = true;

      this.cdr.detectChanges();
    },

    error: (error) => {
      console.error('Update Settings Error:', error);

      this.isSaving = false;
      this.errorMessage =
        error.error?.message || 'Failed to save settings.';

      this.cdr.detectChanges();
    }
  });
}

closeSuccessModal(): void {
  this.showSuccessModal = false;
  this.cdr.detectChanges();
}


  // Reset Settings
  resetSettings(): void {
    this.savedMessage = '';
    this.errorMessage='';
    this.loadSettings();
  }
}
