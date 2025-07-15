using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<ProductStatus>> GetAllProductStatusAsync();

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<List<Product>> GetFeaturedProductsAsync(int take);
        Task<Product?> GetNewestProductAsync();

    }
}