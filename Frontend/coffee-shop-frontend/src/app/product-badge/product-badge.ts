import { Component , Input} from '@angular/core';

@Component({
  selector: 'app-product-badge',
  imports: [],
  templateUrl: './product-badge.html',
  styleUrl: './product-badge.css',
})
export class ProductBadge {
  @Input() stockQuantity:number=0;

}
