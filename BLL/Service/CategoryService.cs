using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository;
using DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryService(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        private void ValidateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
        }
        public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _categoryRepo.IsCategoryNameExistsAsync(name, excludeId);
        }

        public async Task AddCategoryAsync(Category category)
        {
            // Business logic validation
            ValidateCategory(category);

            // Check for duplicate category name
            if (await IsCategoryNameExistsAsync(category.Name))
            {
                throw new InvalidOperationException($"A category with the name '{category.Name}' already exists.");
            }

            await _categoryRepo.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            // Business logic validation
            ValidateCategory(category);

            if (category.Id <= 0)
                throw new ArgumentException("Category ID must be greater than zero.", nameof(category.Id));

            // Check for duplicate category name (excluding current category)
            if (await IsCategoryNameExistsAsync(category.Name, category.Id))
            {
                throw new InvalidOperationException($"A category with the name '{category.Name}' already exists.");
            }

            await _categoryRepo.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Category ID must be greater than zero.", nameof(id));

            // Additional business logic can be added here
            // For example: Check if product is referenced in orders before deletion

            await _categoryRepo.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(id));

            var user = await _categoryRepo.GetCategoryByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException($"User with ID {id} not found.");

            return user;
        }
    }
}