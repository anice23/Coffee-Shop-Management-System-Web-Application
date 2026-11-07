
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeSystem_Business
{
    public class CategoryService
    {
        private readonly CategoryRepository _repository;

        public CategoryService(CategoryRepository repository)
        {
            _repository = repository;
        }

        // Methode N°1
        public Task<List<Category>> GetAllCategoriesAsync()
        {
            return _repository.GetAllCategoriesAsync();
        }
        // Get All Categories Including Inactive
        public async Task<List<Category>> GetAllCategoriesIncludingInactiveAsync()
        {
            return await _repository.GetAllCategoriesIncludingInactiveAsync();
        }

        // Methode N°2
        public Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return _repository.GetCategoryByIdAsync(categoryId);
        }

        // Methode N°3
        public Task AddCategoryAsync(Category category)
        {
            return _repository.AddCategoryAsync(category);
        }

        // Methode N°4
        public Task UpdateCategoryAsync(
            int categoryId,
            string name,
            string description)
        {
            return _repository.UpdateCategoryAsync(
                categoryId,
                name,
                description
            );
        }

        // Methode N°5
        public Task DeactivateCategoryAsync(int categoryId)
        {
            return _repository.DeactivateCategoryAsync(categoryId);
        }

        // Methode N°6
        public Task RestoreCategoryAsync(int categoryId)
        {
            return _repository.RestoreCategoryAsync(categoryId);
        }

        // Methode N°7
        public Task<List<Category>> GetInactiveCategoriesAsync()
        {
            return _repository.GetInactiveCategoriesAsync();
        }

        // Methode N°8
        public Task<bool> CategoryNameExistsAsync(string name)
        {
            return _repository.CategoryNameExistsAsync(name);
        }

        // Methode N°9
        public Task<List<Category>> SearchCategoriesAsync(
            string searchText)
        {
            return _repository.SearchCategoriesAsync(searchText);
        }

        // Methode N°10
        public Task<List<Category>> SearchInactiveCategoriesAsync(
            string searchText)
        {
            return _repository.SearchInactiveCategoriesAsync(
                searchText
            );
        }
    }
}
