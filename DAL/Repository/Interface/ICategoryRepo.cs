using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);

        Task AddCategoryAsync(Category category);

        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<bool> HasForeignKeyDependenciesAsync(int id);
    }
}