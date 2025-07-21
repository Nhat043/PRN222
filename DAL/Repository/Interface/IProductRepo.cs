using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> GetProductByIdWithCategoryAndStatusAsync(int id);

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<ProductStatus>> GetAllProductStatusAsync();

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<List<Product>> GetFeaturedProductsAsync(int take);
        Task<Product?> GetNewestProductInStockAsync();

        Task<Product?> GetProductByIdWithAvailableItemsAsync(int id);

        Task<List<Product>> GetAllProductsFullAsync();
        Task<List<string>> GetAllRamOptionsAsync();
        Task<List<string>> GetAllRomOptionsAsync();
        Task<List<Product>> GetFilteredProductsAsync(string search, string ram, string rom, string price, int? categoryId);
        Task<bool> HasForeignKeyDependenciesAsync(int id);
    }
}