import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryService, CategoryDto } from '../services/category';
import { Toast } from '../shared/toast/toast';
import { AuthService } from '../services/auth';
interface Category {
  id: number;
  name: string;
  description:string;
  isActive: boolean;
}

@Component({
  selector: 'app-categories',
  imports: [FormsModule,Toast],
  templateUrl: './categories.html',
  styleUrl: './categories.css',
})
export class Categories implements OnInit {

  private categoryService = inject(CategoryService);
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthService);

  get isAdmin(): boolean {
    return this.authService.currentUser()?.role === 'Admin';
  }

  categories: Category[] = [];
  ngOnInit(): void {
    this.loadCategories();
  }

  // filter data + filter Methode ()

  statusFilter: 'All' | 'Active' | 'Inactive' = 'Active';
  get filteredCategories(): Category[] {
    return this.categories.filter(category => {
      const matchesSearch = category.name
        .toLowerCase()
        .includes(this.searchText.toLowerCase().trim());

      const matchesStatus =
        this.statusFilter === 'All' ||
        (this.statusFilter === 'Active' && category.isActive) ||
        (this.statusFilter === 'Inactive' && !category.isActive);

      return matchesSearch && matchesStatus;
    })
  }
  

  // Search Data
  searchText: string = '';
  // Declaration
  showAddForm: boolean = false;
  newCategoryName: string = '';
  newCategoryDescription = '';

  // Toast Data 
  
    toastTitle = '';
    toastMessage = '';
    toastType: 'success' | 'error' = 'success';


  
  // Opening Add Form
  openAddForm(): void {
    this.newCategoryName = '';
    this.newCategoryDescription='';
    this.showAddForm = true;
  }

  // Canceling Add Form

  cancelAddForm(): void {
    this.newCategoryName = '';
    this.newCategoryDescription='';
    this.showAddForm = false;
  }




  // Methode N°1 : Load Categories Data
  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (data: CategoryDto[]) => {
        this.categories = data.map(category => ({
          id: category.categoryId,
          name: category.name,
          description: category.description,

          isActive: category.isActive
        }));
  
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        alert('Failed to load categories from the server.');
      }
    });
  }


  // Methode N°2 : Add New Category
    addCategory(): void {
    const name = this.newCategoryName.trim();
  
    if (name === '') {
      this.toastTitle = 'Invalid Category';
      this.toastMessage = 'Please enter a category name.';
      this.toastType = 'error';
      return;
    }
  
    const alreadyExists = this.categories.some(
      category => category.name.toLowerCase() === name.toLowerCase()
    );
  
    if (alreadyExists) {
      this.toastTitle = 'Category Already Exists';
      this.toastMessage = 'This category already exists.';
      this.toastType = 'error';
      return;
    }
  
    this.categoryService.addCategory(name,this.newCategoryDescription.trim()).subscribe({
      next: () => {
        this.cancelAddForm();
        this.toastTitle = 'Category Added!';
        this.toastMessage = 'The category was added successfully.';
        this.toastType = 'success';
        this.loadCategories();
      },
      error: (error) => {
        if (error.status === 409) {
          alert('This category already exists.');
        } else if (error.status === 400) {
          alert(error.error?.message ?? 'Invalid category data or CSRF validation failed.');
        } else {
          console.error('Error adding category:', error);
          alert('Failed to add category. Please try again.');
        }
      }
    });
  }
  // Editing categorie
  
  selectedCategory: Category | null = null;
  editCategoryName: string = '';
  editCategoryDescription = '';

  editCategory(category: Category): void {
    this.selectedCategory = category;
    this.editCategoryName = category.name;
    this.editCategoryDescription = category.description;

  }


  saveCategoryChanges(): void {
    if (!this.selectedCategory) return;
  
    const name = this.editCategoryName.trim();
  
    if (name === '') {
      this.toastTitle = 'Invalid Category';
      this.toastMessage = 'Please enter a category name.';
      this.toastType = 'error';
      return;
    }
  
    const alreadyExists = this.categories.some(
      category =>
        category.id !== this.selectedCategory!.id &&
        category.name.toLowerCase() === name.toLowerCase()
    );
  
    if (alreadyExists) {
      this.toastTitle = 'Category Already Exists';
      this.toastMessage = 'This category already exists.';
      this.toastType = 'error';
      return;
    }
  
    this.categoryService.updateCategory(
      this.selectedCategory.id,
      name,
      this.editCategoryDescription.trim()

    ).subscribe({
      next: () => {
        this.cancelEditCategory();
  
        this.toastTitle = 'Category Updated!';
        this.toastMessage = 'The category was updated successfully.';
        this.toastType = 'success';
  
        this.loadCategories();
      },
      error: (error) => {
        if (error.status === 409) {
          this.toastTitle = 'Category Already Exists';
          this.toastMessage = 'This category already exists.';
          this.toastType = 'error';
        } else if (error.status === 400) {
          this.toastTitle = 'Invalid Category';
          this.toastMessage =
            error.error?.message ?? 'Invalid category data or CSRF validation failed.';
          this.toastType = 'error';
        } else {
          console.error('Error updating category:', error);
          this.toastTitle = 'Update Failed';
          this.toastMessage = 'Failed to update category. Please try again.';
          this.toastType = 'error';
        }
      }
    });
  }
  

  cancelEditCategory(): void {
    this.selectedCategory = null;
    this.editCategoryName = '';
  }

  // desactivate methode 
  

  deactivateCategory(categoryId: number): void {
    const confirmed = confirm(
      'Are You Sure ?'
    );
  
    if (!confirmed) {
      return;
    }
    const category = this.categories.find(
      category => category.id === categoryId
    );
  
    if (!category || !category.isActive) {
      return;
    }
  
    this.categoryService.deactivateCategory(categoryId).subscribe({
      next: () => {
        this.toastTitle = 'Category Deactivated!';
        this.toastMessage = 'The category was deactivated successfully.';
        this.toastType = 'success';
  
        this.loadCategories();
      },
      error: (error) => {
        console.error('Error deactivating category:', error);
  
        this.toastTitle = 'Deactivation Failed!';
        this.toastMessage =
          error.error?.message ?? 'Failed to deactivate the category.';
        this.toastType = 'error';
      }
    });
  }
  
  // reactivate methode
  
reactivateCategory(categoryId: number): void {
  const confirmed = confirm(
    'هل حقًا تحب ترجّع التصنيف هذا Active؟'
  );

  if (!confirmed) {
    return;
  }

  const category = this.categories.find(
    category => category.id === categoryId
  );

  if (!category || category.isActive) {
    return;
  }

  this.categoryService.reactivateCategory(categoryId).subscribe({
    next: () => {
      this.toastTitle = 'Category Reactivated!';
      this.toastMessage = 'The category was reactivated successfully.';
      this.toastType = 'success';

      this.loadCategories();
    },
    error: (error) => {
      console.error('Error reactivating category:', error);

      this.toastTitle = 'Reactivation Failed!';
      this.toastMessage =
        error.error?.message ?? 'Failed to reactivate the category.';
      this.toastType = 'error';
    }
  });
}

  // close Toast
  onToastClosed():void{
    this.toastMessage="";
  }


  showPermissionModal = false;

openPermissionModal(): void {
  this.showPermissionModal = true;
}

closePermissionModal(): void {
  this.showPermissionModal = false;
}





}
