
using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeSystem_DataAccess.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Method 1: Get All Active Products
        public async Task<List<Product>> GettAllProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Method 2: Get Product By Id
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(
                    p => p.ProductId == productId
                );
        }

        // Method 3: Update Product
        public async Task UpdateProductAsync(
            int productId,
            string name,
            decimal price,
            int stockQuantity,
            int categoryId,
            string? imagePath)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found."
                );
            }

            product.Name = name;
            product.Price = price;
            product.StockQuantity = stockQuantity;
            product.CategoryId = categoryId;
            product.ImagePath = imagePath;

            await _context.SaveChangesAsync();
        }

        // Method 4: Add Product
        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();
        }

        // Method 5: Check If Product Name Exists
        public async Task<bool> ProductNameExistsAsync(string name)
        {
            return await _context.Products
                .AnyAsync(p => p.Name == name);
        }

        // Method 6: Deactivate Product
        public async Task DeactivateProductAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found."
                );
            }

            product.IsActive = false;

            await _context.SaveChangesAsync();
        }

        // Method 7: Activate Product
        public async Task ActivateProductAsync(int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found."
                );
            }

            if (product.IsActive)
            {
                throw new InvalidOperationException(
                    "Product is already active."
                );
            }

            product.IsActive = true;

            await _context.SaveChangesAsync();
        }

        // Method 8: Get Inactive Products
        public async Task<List<Product>> GetInactiveProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => !p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Method 9: Search Products By Name
        public async Task<List<Product>> SearchProductsAsync(
            string searchText)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p =>
                    p.IsActive &&
                    p.Name.Contains(searchText))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Method 10: Filter And Search Products
        public async Task<List<Product>> FilterProductsAsync(
            string? searchText,
            int? categoryId)
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(p =>
                    p.Name.Contains(searchText));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
