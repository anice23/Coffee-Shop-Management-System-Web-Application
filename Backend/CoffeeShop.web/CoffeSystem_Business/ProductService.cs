
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Repositories;

namespace CoffeSystem_Business
{
    public class ProductService
    {
        private readonly ProductRepository _repository;
        private readonly ProductValidator _validator;
        private readonly CategoryService _categoryService;
        public ProductService(ProductRepository repository, CategoryService categoryService)
        {
            _repository = repository;
            _validator = new ProductValidator();
            _categoryService = categoryService;

        }

        // Method 1: Get All Active Products
        public Task<List<Product>> GettAllProductsAsync()
        {
            return _repository.GettAllProductsAsync();
        }

        // Method 2: Get Product By Id
        public Task<Product?> GetProductByIdAsync(int productId)
        {
            return _repository.GetProductByIdAsync(productId);
        }

        // Method 3: Update Product
        public Task UpdateProductAsync(
            int productId,
            string name,
            decimal price,
            int stockQuantity,
            int categoryId,
            string? imagePath)
        {
            return _repository.UpdateProductAsync(
                productId,
                name,
                price,
                stockQuantity,
                categoryId,
                imagePath
            );
        }

        // Method 4: Add Product
        public async Task AddProductAsync(Product product)
        {
            // Validate product in memory
            List<string> errors = _validator.Validate(product);

            if (errors.Count > 0)
            {
                throw new Exception(
                    string.Join(Environment.NewLine, errors)
                );
            }

            product.Name = product.Name.Trim();
            // Check if category exists and is active
            var category = await _categoryService
                .GetCategoryByIdAsync(product.CategoryId);

            if (category == null || !category.IsActive)
            {
                throw new Exception(
                    "The selected category does not exist or is inactive."
                );
            }

            // Check duplicate name
            bool isExists =
                await _repository.ProductNameExistsAsync(product.Name);

            if (isExists)
            {
                throw new Exception(
                    "A product with this name already exists."
                );
            }

            // Save product
            await _repository.AddProductAsync(product);
        }

        // Method 5: Deactivate Product
        public Task DeactivateProductAsync(int productId)
        {
            return _repository.DeactivateProductAsync(productId);
        }

        // Method 6: Activate Product
        public Task ActivateProductAsync(int productId)
        {
            return _repository.ActivateProductAsync(productId);
        }

        // Method 7: Get Inactive Products
        public Task<List<Product>> GetInactiveProductsAsync()
        {
            return _repository.GetInactiveProductsAsync();
        }

        // Method 8: Search Products
        public Task<List<Product>> SearchProductsAsync(string searchText)
        {
            return _repository.SearchProductsAsync(searchText);
        }

        // Method 9: Filter And Search Products
        public Task<List<Product>> FilterProductsAsync(
            string? searchText,
            int? categoryId)
        {
            return _repository.FilterProductsAsync(
                searchText,
                categoryId
            );
        }
    }
}
