
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { switchMap } from 'rxjs';
export interface ProductDto {
  productId: number;
  name: string;
  price: number;
  imagePath: string | null;
  stockQuantity: number;
  categoryName: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);

  // Methode N°1 : get All Products
  getProducts() {
    return this.http.get<ProductDto[]>(
      'https://localhost:7073/api/Products',
      {
        withCredentials: true,
      }
    );
  }

  // Methode N°2 : Desactivate Product
  deactivateProduct(productId: number) {
    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        {
          withCredentials: true,
        }
      )
      .pipe(
        switchMap((csrf) =>
          this.http.delete<{ message: string }>(
            `https://localhost:7073/api/Products/${productId}`,
            {
              withCredentials: true,
              headers: new HttpHeaders({
                'X-CSRF-TOKEN': csrf.requestToken,
              }),
            }
          )
        )
      );
  }

  // Methode N°3 : Reactivate Products
  reactivateProduct(productId: number) {
    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        { withCredentials: true }
      )
      .pipe(
        switchMap((csrf) =>
          this.http.patch<{ message: string }>(
            `https://localhost:7073/api/Products/${productId}/reactivate`,
            {},
            {
              withCredentials: true,
              headers: new HttpHeaders({
                'X-CSRF-TOKEN': csrf.requestToken,
              }),
            }
          )
        )
      );
  }

  // Methode N°4 : get Inactive Product
  getInactiveProducts() {
    return this.http.get<ProductDto[]>(
      'https://localhost:7073/api/Products/inactive',
      { withCredentials: true }
    );
  }

  // Methode N°5 : Update Product
  
updateProduct(
  productId: number,
  name: string,
  price: number,
  stockQuantity: number,
  categoryId: number,
  image?: File | null
) {
  const formData = new FormData();

  formData.append('Name', name);
  formData.append('Price', price.toString());
  formData.append('StockQuantity', stockQuantity.toString());
  formData.append('CategoryId', categoryId.toString());

  if (image) {
    formData.append('Image', image);
  }

  return this.http
    .get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    )
    .pipe(
      switchMap((csrf) =>
        this.http.put<{ message: string }>(
          `https://localhost:7073/api/Products/${productId}`,
          formData,
          {
            withCredentials: true,
            headers: new HttpHeaders({
              'X-CSRF-TOKEN': csrf.requestToken,
            }),
          }
        )
      )
    );

    
}
// Methode N°5 :Add New Product

  addProduct(name: string,price: number,stockQuantity: number,categoryId: number, image?: File | null ) {
    const formData = new FormData();

    formData.append('Name', name);
    formData.append('Price', price.toString());
    formData.append('StockQuantity', stockQuantity.toString());
    formData.append('CategoryId', categoryId.toString());

    if (image) {
      formData.append('Image', image);
    }

    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        { withCredentials: true }
      )
      .pipe(
        switchMap((csrf) =>
          this.http.post<{ message: string; productId: number }>(
            'https://localhost:7073/api/Products',
            formData,
            {
              withCredentials: true,
              headers: new HttpHeaders({
                'X-CSRF-TOKEN': csrf.requestToken,
              }),
            }
          )
        )
      );
  }



  

}
