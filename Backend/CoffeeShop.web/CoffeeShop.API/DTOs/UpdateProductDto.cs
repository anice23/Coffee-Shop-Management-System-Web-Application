
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CoffeeShop.API.DTOs;

public class UpdateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    public IFormFile? Image { get; set; }
}
