using CoffeSystem_DataAccess.Entities;

namespace CoffeSystem_Business
{
    public class ProductValidator
    {
        public List<string> Validate(Product product)
        {
            List<string> errors = new List<string>();
            if(string.IsNullOrWhiteSpace(product.Name))
            {
                errors.Add("Product name is required");
            }

            if (product.Name?.Trim().Length > 100)
            {
                errors.Add("Product name cannot exceed 100 characters.");
            }

            if (product.Price <= 0)
            {
                errors.Add("Price cannot be negative.");

            }

            if (product.StockQuantity < 0)
            {
                errors.Add("Stock quantity cannot be negative.");
            }

            if (product.CategoryId <= 0)
            {
                errors.Add("Please select a category.");
            }

            return errors;
        }
        
    }
}
