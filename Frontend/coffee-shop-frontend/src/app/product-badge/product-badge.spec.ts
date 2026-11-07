import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductBadge } from './product-badge';

describe('ProductBadge', () => {
  let component: ProductBadge;
  let fixture: ComponentFixture<ProductBadge>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductBadge],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductBadge);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
