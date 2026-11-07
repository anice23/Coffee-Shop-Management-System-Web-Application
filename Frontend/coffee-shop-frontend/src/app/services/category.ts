
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { switchMap } from 'rxjs';

export interface CategoryDto {
  categoryId: number;
  name: string;
  description: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private http = inject(HttpClient);

  // methode N°1
  getCategories() {
    return this.http.get<CategoryDto[]>(
      'https://localhost:7073/api/Categories',
      { withCredentials: true }
    );
  }

  // methode N°2
  getAllCategories() {
    return this.http.get<CategoryDto[]>(
      'https://localhost:7073/api/Categories/all',
      { withCredentials: true }
    );
  }

  // methode N°3
  addCategory(name: string, description: string = '') {
    return this.http
      .get<{ requestToken: string }>(
        'https://localhost:7073/api/Auth/csrf',
        { withCredentials: true }
      )
      .pipe(
        switchMap(csrf =>
          this.http.post<CategoryDto>(
            'https://localhost:7073/api/Categories',
            {
              name: name.trim(),
              description: description.trim()
            },
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

  // methode N°4
  
updateCategory(id: number,name: string,description: string = '')
 {
  return this.http
    .get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    )
    .pipe(
      switchMap(csrf =>
        this.http.put<CategoryDto>(
          `https://localhost:7073/api/Categories/${id}`,
          {
            name: name.trim(),
            description: description.trim()
          },
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

// methode N°5 

deactivateCategory(id: number) {
  return this.http
    .get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    )
    .pipe(
      switchMap(csrf =>
        this.http.patch(
          `https://localhost:7073/api/Categories/${id}/deactivate`,
          {},
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

// methode N°6 : reactivate category

reactivateCategory(id: number) {
  return this.http
    .get<{ requestToken: string }>(
      'https://localhost:7073/api/Auth/csrf',
      { withCredentials: true }
    )
    .pipe(
      switchMap(csrf =>
        this.http.patch(
          `https://localhost:7073/api/Categories/${id}/reactivate`,
          {},
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
