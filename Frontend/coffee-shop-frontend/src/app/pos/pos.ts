
import { Component, inject ,OnInit,ChangeDetectorRef  } from '@angular/core';
import { OrdersService } from '../services/orders';
import { ProductService, ProductDto } from '../services/product';
import { CategoryService, CategoryDto } from '../services/category';
import { SettingsService, CafeSettings } from '../services/settings';
import { FormsModule } from '@angular/forms';
import { jsPDF } from 'jspdf';

interface PosProduct {
  id: number;
  name: string;
  price: number;
  image: string;
  stock: number;
  category: string;

}

interface CartItem {
  product: PosProduct;
  quantity: number;
}

@Component({
  selector: 'app-pos',
  imports: [FormsModule],
  templateUrl: './pos.html',
  styleUrl: './pos.css',
})
export class Pos implements OnInit {
  private ordersService = inject(OrdersService);
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);
  private settingsService = inject(SettingsService);
  cafeSettings: CafeSettings | null = null;
  products: PosProduct[] = [];
  showConfirmationModal = false;
  private categoryService = inject(CategoryService);

  categories: CategoryDto[] = [];

  // filter 
  searchText = '';

selectedCategory = 'All';

get filteredProducts(): PosProduct[] {
  return this.products.filter(product => {

    const matchesSearch = product.name
      .toLowerCase()
      .includes(this.searchText.toLowerCase().trim());

    const matchesCategory =
      this.selectedCategory === 'All' ||
      product.category === this.selectedCategory;

    return matchesSearch && matchesCategory;
  });
}

  //
  openConfirmationModal(): void {
    if (this.cart.length === 0) {
      return;
    }
  
    this.showConfirmationModal = true;
  }
  
  closeConfirmationModal(): void {
    this.showConfirmationModal = false;
  }

  // when open pos.html => load product from backend
  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      next: (data) => {
        console.log('First product image path:', data[0]?.imagePath);
        this.products = data.map(product => ({
          id: product.productId,
          name: product.name,
          price: product.price,
          image: product.imagePath
          ? `https://localhost:7073/${product.imagePath.replace(/\\/g, '/')}`
         : 'images/default-product.jpg',
          stock: product.stockQuantity,
          category: product.categoryName

        }));
        this.cdr.detectChanges();
        console.log('Products mapped for POS:', this.products);
      },
      error: (error) => {
        console.error('Error loading products:', error);
        alert('Failed to load products from the server.');
      }
    });

    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        console.log('POS Categories:', this.categories);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });

    // load coffee settings
    // Load Cafe Settings from Backend
this.settingsService.getSettings().subscribe({
  next: (data) => {
    this.cafeSettings = data;

    console.log('Cafe Settings loaded for Invoice:', data);

    this.cdr.detectChanges();
  },
  error: (error) => {
    console.error('Error loading Cafe Settings:', error);
  }
});
  }

  cart: CartItem[] = [];
  paymentMethod: 'Cash' | 'Card' = 'Cash';

  get totalItems(): number {
    return this.cart.reduce((sum, item) => sum + item.quantity, 0);
  }

  get totalAmount(): number {
    return this.cart.reduce(
      (sum, item) => sum + item.product.price * item.quantity,
      0
    );
  }

  addToCart(product: PosProduct): void {
    const cartItem = this.cart.find(item => item.product.id === product.id);

    if (cartItem) {
      if (cartItem.quantity >= product.stock) {
        alert('Not enough stock available.');
        return;
      }

      cartItem.quantity++;
    } else {
      if (product.stock <= 0) {
        alert('This product is out of stock.');
        return;
      }

      this.cart.push({ product, quantity: 1 });
    }
  }

  increaseQuantity(item: CartItem): void {
    if (item.quantity >= item.product.stock) {
      alert('Not enough stock available.');
      return;
    }

    item.quantity++;
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      item.quantity--;
    } else {
      this.removeFromCart(item);
    }
  }

  removeFromCart(item: CartItem): void {
    this.cart = this.cart.filter(
      cartItem => cartItem.product.id !== item.product.id
    );
  }

  clearCart(): void {
    this.cart = [];
  }

  // 
  
nextOrderId = 1001;
lastSaleMessage = '';
isProcessingOrder = false;
completeSale(): void {
  // Prevent duplicate order submissions.
  if (this.isProcessingOrder || this.cart.length === 0) {
    return;
  }

  this.isProcessingOrder = true;

  // Validate product stock.
  const stockIsValid = this.cart.every(
    item => item.quantity > 0 && item.quantity <= item.product.stock
  );

  if (!stockIsValid) {
    this.isProcessingOrder = false;
    return;
  }

  // Prepare backend request.
  const request = {
    paymentMethod: this.paymentMethod,
    items: this.cart.map(item => ({
      productId: item.product.id,
      quantity: item.quantity
    }))
  };

  // Preserve invoice data before clearing cart.
  const orderTotal = this.totalAmount;
  const orderPaymentMethod = this.paymentMethod;

  const orderItems = this.cart.map(item => ({
    productName: item.product.name,
    quantity: item.quantity,
    unitPrice: item.product.price
  }));

  // Create order.
  this.ordersService.createOrder(request).subscribe({

    next: (response) => {
      // Update local stock.
      for (const item of this.cart) {
        item.product.stock -= item.quantity;
      }

      // Prepare invoice.
      const order = {
        id: response.orderId,
        items: orderItems,
        total: orderTotal,
        paymentMethod: orderPaymentMethod,
        date: new Date()
      };

      this.lastInvoice = order;

      // Display success message.
      this.lastSaleMessage =
        `Order #${order.id} completed successfully — ${order.total.toFixed(2)} DT (${order.paymentMethod}).`;

      console.log('Order created successfully:', response);

      // Clear cart after successful order.
      this.clearCart();

      // Close confirmation modal.
      this.showConfirmationModal = false;

      // Allow next order.
      this.isProcessingOrder = false;

      // Refresh Angular view.
      this.cdr.detectChanges();
    },

    error: (error) => {
      console.error('Error creating order:', error);

      this.showConfirmationModal = false;
      this.isProcessingOrder = false;

      alert(
        error.error?.message ??
        'Failed to complete the sale. Please try again.'
      );

      // Refresh Angular view.
      this.cdr.detectChanges();
    }
  });
}
// 
lastInvoice: {
  id: number;
  items: {
    productName: string;
    quantity: number;
    unitPrice: number;
  }[];
  total: number;
  paymentMethod: 'Cash' | 'Card';
  date: Date;
} | null = null;

// print Methode

printInvoice(): void {
  if (!this.lastInvoice) {
    alert('There is no invoice to print.');
    return;
  }

  const invoice = this.lastInvoice;

  const estimatedHeight = 75 + invoice.items.reduce((total, item) => {
    const lines = Math.ceil(item.productName.length / 25);
    return total + Math.max(lines * 4, 5) + 11;
  }, 0);

  const pdf = new jsPDF({
    orientation: 'portrait',
    unit: 'mm',
    format: [80, estimatedHeight]
  });
  const margin = 5;
  const contentWidth = 70;
  let y = 12;

  // Invoice header
// Invoice header - Dynamic Cafe Settings
const settings = this.cafeSettings;

if (!settings) {
  alert('Cafe settings are still loading. Please try again.');
  return;
}

pdf.setFont('helvetica', 'bold');
pdf.setFontSize(16);

const cafeNameLines = pdf.splitTextToSize(settings.cafeName, 68);

pdf.text(cafeNameLines, 40, y, {
  align: 'center'
});

y += cafeNameLines.length * 6;

pdf.setFont('helvetica', 'normal');
pdf.setFontSize(9);

// Cafe Address
if (settings.address.trim()) {
  const addressLines = pdf.splitTextToSize(settings.address, 68);

  pdf.text(addressLines, 40, y, {
    align: 'center'
  });

  y += addressLines.length * 4;
}

// Cafe Phone Number
if (settings.phoneNumber.trim()) {
  pdf.text(settings.phoneNumber, 40, y, {
    align: 'center'
  });

  y += 5;
}

y += 5;

  // Invoice information
  pdf.setFontSize(9);
  pdf.text(`Invoice: #${invoice.id}`, margin, y);
  y += 5;

  pdf.text(`Date: ${invoice.date.toLocaleString()}`, margin, y);
  y += 5;

  pdf.text(`Payment: ${invoice.paymentMethod}`, margin, y);
  y += 8;

  // Table header
  pdf.setFont('helvetica', 'bold');
  pdf.text('Product', margin, y);
  pdf.text('Qty', 43, y);
  pdf.text('Price', 54, y);
  y += 2;

  pdf.line(margin, y, 75, y);
  y += 5;

  // Invoice items
  pdf.setFont('helvetica', 'normal');

  for (const item of invoice.items) {
    const productLines = pdf.splitTextToSize(
      item.productName,
      34
    );

    pdf.text(productLines, margin, y);
    pdf.text(String(item.quantity), 43, y);
    pdf.text(`${item.unitPrice.toFixed(2)}`, 54, y);

    y += Math.max(productLines.length * 4, 5);

    pdf.text(
      `Total: ${(item.quantity * item.unitPrice).toFixed(2)} DT`,
      75,
      y,
      { align: 'right' }
    );

    y += 6;
  }

  pdf.line(margin, y, 75, y);
  y += 7;

  // Final total
  pdf.setFont('helvetica', 'bold');
  pdf.setFontSize(12);
  pdf.text(
    `TOTAL: ${invoice.total.toFixed(2)} DT`,
    40,
    y,
    { align: 'center' }
  );

  y += 10;

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

  // Fit the PDF page to the actual invoice content.
  pdf.internal.pageSize.height = y + 8;
  pdf.autoPrint();
  window.open(pdf.output('bloburl'), '_blank');}







}
// 
