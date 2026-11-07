
using CoffeSystem_Business;
using CoffeSystem_DataAccess.Entities;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace CoffeSystem_API.Controllers
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }


    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly IAntiforgery _antiforgery;
        public CategoriesController(CategoryService categoryService, IAntiforgery antiforgery)
        {
            _categoryService = categoryService;
            _antiforgery = antiforgery;
        }

        // Endpoint N°1 : GetAllCategories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories =await _categoryService.GetAllCategoriesAsync();

            return Ok(categories.Select(c => new
            {
                c.CategoryId,
                c.Name,
                c.Description
            }));
        }

        // 
        // GET: api/Categories/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategoriesIncludingInactive()
        {
            var categories = await _categoryService
                .GetAllCategoriesIncludingInactiveAsync();

            return Ok(categories.Select(c => new
            {
                c.CategoryId,
                c.Name,
                c.Description,
                c.IsActive
            }));
        }

        // Endpoint N°2 : Add New Category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new
                {
                    message = "CSRF validation failed."
                });
            }

            var name = request.Name.Trim();
            var description = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    message = "Category name is required."
                });
            }

            if (await _categoryService.CategoryNameExistsAsync(name))
            {
                return Conflict(new
                {
                    message = "This category already exists."
                });
            }

            var category = new Category
            {
                Name = name,
                Description = description,
                IsActive = true
            };

            await _categoryService.AddCategoryAsync(category);

            return Ok(new
            {
                category.CategoryId,
                category.Name,
                category.Description,
                category.IsActive
            });
        }

        // Enpdoint N°3: Update Category

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateCategory(int id,[FromBody] UpdateCategoryRequest request)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new { message = "CSRF validation failed." });
            }

            var name = request.Name?.Trim() ?? string.Empty;
            var description = request.Description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Category name is required." });

            var existingCategory = await _categoryService.GetCategoryByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new { message = "Category not found." });

            // Prevent duplicate names, but allow keeping this category's current name.
            if (!string.Equals(existingCategory.Name, name, StringComparison.OrdinalIgnoreCase)
                && await _categoryService.CategoryNameExistsAsync(name))
            {
                return Conflict(new { message = "This category already exists." });
            }

            await _categoryService.UpdateCategoryAsync(id, name, description);

            return Ok(new
            {
                existingCategory.CategoryId,
                Name = name,
                Description = description,
                existingCategory.IsActive
            });
        }


        // Endpoint N°4 : Desactivate Category
        [Authorize(Roles = "Admin")]


        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new { message = "CSRF validation failed." });
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            if (!category.IsActive)
            {
                return Conflict(new { message = "Category is already inactive." });
            }

            await _categoryService.DeactivateCategoryAsync(id);

            return Ok(new
            {
                categoryId = id,
                isActive = false,
                message = "Category deactivated successfully."
            });
        }

        // Endpoint N°5 : Reactivate Category


        [HttpPatch("{id:int}/reactivate")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> ReactivateCategory(int id)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return BadRequest(new { message = "CSRF validation failed." });
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            if (category.IsActive)
            {
                return Conflict(new { message = "Category is already active." });
            }

            await _categoryService.RestoreCategoryAsync(id);

            return Ok(new
            {
                categoryId = id,
                isActive = true,
                message = "Category reactivated successfully."
            });
        }


    }

}
