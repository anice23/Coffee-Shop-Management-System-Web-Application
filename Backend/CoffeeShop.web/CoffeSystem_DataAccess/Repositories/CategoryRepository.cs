
using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeSystem_DataAccess.Repositories
{
    public class CategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1 : Get All Categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // : Get All Active&Inactive Categories
        public async Task<List<Category>> GetAllCategoriesIncludingInactiveAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Methode N°2 : Get Category By Id
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(
                    c => c.CategoryId == categoryId
                );
        }

        // Methode N°3 : Add Category
        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();
        }

        // Methode N°4 : Update Category
        public async Task UpdateCategoryAsync(
            int categoryId,
            string name,
            string description)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(
                    c => c.CategoryId == categoryId
                );

            if (category == null)
                return;

            category.Name = name;
            category.Description = description;

            await _context.SaveChangesAsync();
        }

        // Methode N°5 : Delete Category
        public async Task DeactivateCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(
                    c => c.CategoryId == categoryId
                );

            if (category == null)
                return;

            category.IsActive = false;

            await _context.SaveChangesAsync();
        }

        // Methode N°6 : Restore Category
        public async Task RestoreCategoryAsync(int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(
                    c => c.CategoryId == categoryId
                );

            if (category == null)
                return;

            category.IsActive = true;

            await _context.SaveChangesAsync();
        }

        // Methode N°7: Get All Inactive Categories
        public async Task<List<Category>> GetInactiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Methode N°8 : Check if category by name exists
        public async Task<bool> CategoryNameExistsAsync(string name)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name == name);
        }

        // Methode N°9 : Search Categories by name
        public async Task<List<Category>> SearchCategoriesAsync(
            string searchText)
        {
            return await _context.Categories
                .Where(c =>
                    c.IsActive &&
                    c.Name.Contains(searchText))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // Methode N°10 : Search Inactive Categories by name
        public async Task<List<Category>> SearchInactiveCategoriesAsync(
            string searchText)
        {
            return await _context.Categories
                .Where(c =>
                    !c.IsActive &&
                    c.Name.Contains(searchText))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
