
import {Component,OnInit,inject,ChangeDetectorRef,NgZone}from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService, ProductDto } from '../services/product';
import { Toast } from '../shared/toast/toast';
import { forkJoin } from 'rxjs';
import { CategoryService, CategoryDto } from '../services/category';
import { AuthService } from '../services/auth';
interface Product {
  id: number;
  name: string;
  price: number;
  stockQuantity: number;
  category: string;
  isActive: boolean;
  imageUrl: string;

}

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule,Toast],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);
  private zone = inject(NgZone);
  private categoryService = inject(CategoryService);
  private authService = inject(AuthService);

  get isAdmin(): boolean {
    return this.authService.currentUser()?.role === 'Admin';
  }

categories: CategoryDto[] = [];

  toastTitle = '';
  toastMessage = '';
  toastType: 'success' | 'error' = 'success';

  // Real products loaded from the API
  products: Product[] = [];

  isLoading = false;
  errorMessage = '';
  showPermissionModal = false;

  openPermissionModal(): void {
    this.showPermissionModal = true;
  }
  
  closePermissionModal(): void {
    this.showPermissionModal = false;
  }

  // Load products when the page opens
  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();

  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';
  
    forkJoin({
      active: this.productService.getProducts(),
      inactive: this.productService.getInactiveProducts(),
    }).subscribe({
      next: ({ active, inactive }) => {
        const mapProduct = (
          item: ProductDto,
          isActive: boolean
        ): Product => ({
          id: item.productId,
          name: item.name,
          price: item.price,
          stockQuantity: item.stockQuantity,
          category: item.categoryName,
          isActive,
          imageUrl: item.imagePath
            ? `https://localhost:7073/${item.imagePath.replace(/\\/g, '/')}`
            : '',
        });
  
        this.products = [
          ...active.map((item) => mapProduct(item, true)),
          ...inactive.map((item) => mapProduct(item, false)),
        ];
  
        this.isLoading = false;
        this.cdr.detectChanges();
      },
  
      error: (error) => {
        console.error('Products API error:', error);
  
        this.errorMessage =
          error.status === 401 || error.status === 403
            ? 'Your session has expired or you are not authorized.'
            : 'Unable to load products. Please try again.';
  
        this.isLoading = false;
      },
    });
  }

  
loadCategories(): void {
  this.categoryService.getCategories().subscribe({
    next: (categories) => {
      this.categories = categories;
      console.log('Categories:', this.categories);
    },
    error: (error) => {
      console.error('Categories API error:', error);
    },
  });
}

  // Search and status filter
  searchText = '';
  statusFilter: 'All' | 'Active' | 'Inactive' = 'All';
  categoryFilter: string = 'All';

  // Editing variables
  selectedProduct: Product | null = null;
  isAddingProduct = false;
  newProductName = '';
  newProductPrice = 0;
  newProductStockQuantity = 0;
  newProductCategoryId = 0;

  editName = '';
  editPrice = 0;
  editStockQuantity = 0;
  editCategoryId = 0;
  newProductImage: File | null = null;
  newProductImagePreview: string | null = null;
  editProductImage: File | null = null;
  editProductImagePreview: string | null = null;

  get filteredProducts(): Product[] {
    return this.products.filter((product) => {
      const matchesSearch = product.name
        .toLowerCase()
        .includes(this.searchText.toLowerCase().trim());
  
      const matchesStatus =
        this.statusFilter === 'All' ||
        (this.statusFilter === 'Active' && product.isActive) ||
        (this.statusFilter === 'Inactive' && !product.isActive);
  
      const matchesCategory =
        this.categoryFilter === 'All' ||
        product.category === this.categoryFilter;
  
      return matchesSearch && matchesStatus && matchesCategory;
    });
  }
  // Edit product
  
editProduct(product: Product): void {
  this.selectedProduct = product;
  this.editName = product.name;
  this.editPrice = product.price;
  this.editStockQuantity = product.stockQuantity;

  const category = this.categories.find(
    (c) => c.name === product.category
  );

  this.editCategoryId = category?.categoryId ?? 0;
}

  // Deactivate product 

  deactivateProduct(productId: number): void {
    const product = this.products.find((p) => p.id === productId);
  
    if (!product || !product.isActive) {
      return;
    }
  
    const confirmed = confirm(
      `Are you sure you want to deactivate "${product.name}"?`
    );
  
    if (!confirmed) {
      return;
    }
  
    this.productService.deactivateProduct(productId).subscribe({
      next: () => {
        this.toastTitle = 'Success!';
        this.toastMessage = 'Product deactivated successfully.';
        this.toastType = 'success';
  
        this.loadProducts();
      },
      error: (error) => {
        console.error('Deactivate product error:', error);
  
        this.toastTitle = 'Operation failed';
        this.toastMessage =
          error.status === 401 || error.status === 403
            ? 'Your session has expired or you are not authorized.'
            : 'Unable to deactivate this product. Please try again.';
  
        this.toastType = 'error';
      },
    });
  }
  

  // Save changes
  saveProductChanges(): void {
    if (!this.selectedProduct) return;
  
    const name = this.editName.trim();
  
    if (
      name === '' ||
      !Number.isFinite(this.editPrice) ||
      this.editPrice <= 0 ||
      !Number.isInteger(this.editStockQuantity) ||
      this.editStockQuantity < 0 ||
      this.editCategoryId <= 0
    ) {
      alert('Please enter valid product information.');
      return;
    }
  
    this.productService.updateProduct(
      this.selectedProduct.id,
      name,
      this.editPrice,
      this.editStockQuantity,
      this.editCategoryId,
      this.editProductImage

    ).subscribe({
      next: () => {
        this.toastTitle = 'Success';
        this.toastMessage = 'Product updated successfully.';
        this.toastType = 'success';
  
        this.cancelEdit();
        this.loadProducts();
      },
      error: (error) => {
        console.error('Update product error:', error);
  
        this.toastTitle = 'Error';
        this.toastMessage =
          error.error?.message ??
          'Unable to update product. Please try again.';
        this.toastType = 'error';
      }
    });
  }
  // Cancel editing
  cancelEdit(): void {
    this.selectedProduct = null;
    this.editName = '';
    this.editPrice = 0;
    this.editStockQuantity = 0;
    this.editCategoryId = 0;
  }

  activateProduct(productId: number): void {
    const product = this.products.find((p) => p.id === productId);
  
    if (!product || product.isActive) {
      return;
    }
  
    const confirmed = confirm(
      `Are you sure you want to activate "${product.name}"?`
    );
  
    if (!confirmed) {
      return;
    }
  
    this.productService.reactivateProduct(productId).subscribe({
      next: () => {
        this.toastTitle = 'Success!';
        this.toastMessage = 'Product activated successfully.';
        this.toastType = 'success';
  
        this.loadProducts();
      },
      error: (error) => {
        console.error('Reactivate product error:', error);
  
        this.toastTitle = 'Operation failed';
        this.toastMessage =
          error.status === 401 || error.status === 403
            ? 'Your session has expired or you are not authorized.'
            : 'Unable to activate this product. Please try again.';
  
        this.toastType = 'error';
      },
    });
  }

  openAddProduct(): void {
    this.cancelEdit();
    this.isAddingProduct = true;
  }

  // save New Product
  
saveNewProduct(): void {
  const name = this.newProductName.trim();

  if (
    name === '' ||
    name.length > 100 ||
    !Number.isFinite(this.newProductPrice) ||
    this.newProductPrice <= 0 ||
    !Number.isInteger(this.newProductStockQuantity) ||
    this.newProductStockQuantity < 0 ||
    this.newProductCategoryId <= 0
  ) {
    this.toastTitle = 'Invalid product';
    this.toastMessage = 'Please enter valid product information.';
    this.toastType = 'error';
    return;
  }

  this.productService.addProduct(
    name,
    this.newProductPrice,
    this.newProductStockQuantity,
    this.newProductCategoryId,
    this.newProductImage

  ).subscribe({
    next: () => {
      this.toastTitle = 'Success!';
      this.toastMessage = 'Product added successfully.';
      this.toastType = 'success';

      this.cancelAddProduct();
      this.loadProducts();
    },
    error: (error) => {
      console.error('Add product error:', error);

      this.toastTitle = 'Operation failed';
      this.toastMessage =
        error.error?.message ??
        'Unable to add product. Please try again.';
      this.toastType = 'error';
    }
  });
}

// cancel Add New Product Methode
cancelAddProduct(): void {
  this.isAddingProduct = false;

  this.newProductName = '';
  this.newProductPrice = 0;
  this.newProductStockQuantity = 0;
  this.newProductCategoryId = 0;
  this.removeNewProductImage();
}

// read image methode



onNewProductImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) {
    return;
  }

  this.newProductImage = file;

  const reader = new FileReader();

  reader.onload = () => {
    this.newProductImagePreview = reader.result as string;
    this.cdr.detectChanges();

    console.log(
      'Add image preview updated:',
      !!this.newProductImagePreview
    );
  };

  reader.onerror = () => {
    console.error('FileReader error:', reader.error);
  };

  reader.readAsDataURL(file);
}


// edit image

onEditProductImageSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) {
    return;
  }

  this.editProductImage = file;
  console.log('Edit image selected:', file.name);

  const reader = new FileReader();

  reader.onload = () => {
    this.editProductImagePreview = reader.result as string;
    this.cdr.detectChanges();
  
    console.log(
      'Edit image preview updated:',
      !!this.editProductImagePreview
    );
  };
  
  reader.onerror = () => {
    console.error('FileReader error:', reader.error);
  };

  reader.readAsDataURL(file);
}

//

cancelEditImageChange(input: HTMLInputElement): void {
  this.editProductImage = null;
  this.editProductImagePreview = null;

  // نفرّغ اسم التصويرة الجديدة من الـ input
  input.value = '';
}

// remove image from form 

removeNewProductImage(): void {
  this.newProductImage = null;
  this.newProductImagePreview = null;

  const input = document.getElementById(
    'addImage'
  ) as HTMLInputElement | null;

  if (input) {
    input.value = '';
  }
}


}
