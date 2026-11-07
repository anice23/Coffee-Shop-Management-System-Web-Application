
export interface Product{
    id:number;
    name:string;
    price:number;
    stockQuantity:number;
}


export interface CartItem {
    product: Product;
    quantity: number;
  }
  