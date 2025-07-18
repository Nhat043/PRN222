using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly DemoContext _demoContext;

        public CategoryRepo(DemoContext demoContext)
        {
            _demoContext = demoContext;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _demoContext.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _demoContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var query = _demoContext.Categories.Where(c => c.Name.Trim().ToLower() == name.Trim().ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _demoContext.Categories.AddAsync(category);
            await _demoContext.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = await _demoContext.Categories.FindAsync(category.Id);
            if (existingCategory == null)
                throw new InvalidOperationException($"Category with ID {category.Id} not found.");

            // Update properties
            existingCategory.Name = category.Name;
            _demoContext.Categories.Update(existingCategory);
            await _demoContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _demoContext.Categories.FindAsync(id);
            if (category != null)
            {
                _demoContext.Categories.Remove(category);
                await _demoContext.SaveChangesAsync();
            }
        }

        public async Task<bool> HasForeignKeyDependenciesAsync(int id)
        {
            return await _demoContext.Products.AnyAsync(p => p.CategoryId == id);
        }
    }
}