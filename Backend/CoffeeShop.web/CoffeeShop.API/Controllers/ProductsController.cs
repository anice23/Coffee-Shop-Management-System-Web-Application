
using CoffeSystem_Business;
using Microsoft.AspNetCore.Mvc;
using CoffeeShop.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(ProductService productService, IWebHostEnvironment environment)
    {
        _productService = productService;
        _environment = environment;

    }

    // endpoint N°1 : Get All Active Products

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GettAllProductsAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            ImagePath = p.ImagePath,
            StockQuantity = p.StockQuantity,
            CategoryName = p.Category.Name
        }).ToList();

        return Ok(productDtos);
    }

    // endpoint N°2 : Add New Product

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddProduct(
        [FromForm] CreateProductDto dto)
    {
        // Check DTO validation
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Prepare product image path
        string? imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            const long maxImageSize = 5 * 1024 * 1024; // 5 MB

            string[] allowedExtensions =   {  ".jpg", ".jpeg", ".png", ".webp"   };

            string extension =  Path.GetExtension(dto.Image.FileName).ToLowerInvariant();

            if (dto.Image.Length > maxImageSize)
            {
                return BadRequest("Image size must not exceed 5 MB.");
            }

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    "Only JPG, JPEG, PNG and WEBP images are allowed."
                );
            }
            // Create the upload directory if it doesn't exist
            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "Images",
                "Products"
            );

            Directory.CreateDirectory(uploadsFolder);

            // Generate a unique file name
            string fileName = $"{Guid.NewGuid()}{extension}";

            string filePath = Path.Combine(
                uploadsFolder,
                fileName
            );

            // Save the uploaded image
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            // Store the relative path in the database
            imagePath = $"Images/Products/{fileName}";
        }

        try
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                ImagePath = imagePath,
                IsActive = true
            };

            await _productService.AddProductAsync(product);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Product added successfully.",
                product.ProductId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }


    }


    // endpoint N°3 : Upadate Product


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateProduct(int id,[FromForm] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingProduct =
            await _productService.GetProductByIdAsync(id);

        if (existingProduct == null || !existingProduct.IsActive)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        string? imagePath = existingProduct.ImagePath;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            const long maxImageSize = 5 * 1024 * 1024;

            string[] allowedExtensions =
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            string extension =
                Path.GetExtension(dto.Image.FileName).ToLowerInvariant();

            if (dto.Image.Length > maxImageSize)
            {
                return BadRequest("Image size must not exceed 5 MB.");
            }

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    "Only JPG, JPEG, PNG and WEBP images are allowed."
                );
            }

            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "Images",
                "Products"
            );

            Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(
                filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = $"Images/Products/{fileName}";
        }

        try
        {
            await _productService.UpdateProductAsync(
                id,
                dto.Name,
                dto.Price,
                dto.StockQuantity,
                dto.CategoryId,
                imagePath
            );

            return Ok(new
            {
                message = "Product updated successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // endpoint N°4 : Soft Delete Product => desactivate
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null || !product.IsActive)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        try
        {
            await _productService.DeactivateProductAsync(id);

            return Ok(new
            {
                message = "Product deactivated successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred."
            });
        }
    }



    // Endpoint N°5: Get Inactive Products
    [HttpGet("inactive")]
    public async Task<ActionResult<List<ProductDto>>> GetInactiveProducts()
    {
        var products = await _productService.GetInactiveProductsAsync();

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            ImagePath = p.ImagePath,
            StockQuantity = p.StockQuantity,
            CategoryName = p.Category.Name
        }).ToList();

        return Ok(productDtos);
    }


    // Endpoint N°6: Reactivate Product
    [HttpPatch("{id:int}/reactivate")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> ReactivateProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        if (product.IsActive)
        {
            return BadRequest(new
            {
                message = "Product is already active."
            });
        }

        try
        {
            await _productService.ActivateProductAsync(id);

            return Ok(new
            {
                message = "Product reactivated successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred."
            });
        }
    }


}
