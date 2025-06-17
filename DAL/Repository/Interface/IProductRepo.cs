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

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        Task<IEnumerable<ProductStatus>> GetAllProductStatusAsync();

        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}