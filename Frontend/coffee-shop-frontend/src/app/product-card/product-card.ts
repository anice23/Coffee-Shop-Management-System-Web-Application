
import { Component, Input, output } from '@angular/core';
import { ProductBadge } from '../product-badge/product-badge';
import { Product } from '../product';

@Component({
  selector: 'app-product-card',
  imports: [ProductBadge],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {
  @Input() product!: Product;

  addToCart = output<Product>();
}
