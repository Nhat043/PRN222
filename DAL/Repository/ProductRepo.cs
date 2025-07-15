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
    public class ProductRepo : IProductRepo
    {
        private readonly DemoContext _demoContext;

        public ProductRepo(DemoContext demoContext)
        {
            _demoContext = demoContext;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _demoContext.Products
                .Include(p => p.Category)
                .Include(p => p.Status)
                .ToListAsync();
        }


        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _demoContext.Products
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }


        public async Task AddProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            await _demoContext.Products.AddAsync(product);
            await _demoContext.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = await _demoContext.Products.FindAsync(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException($"Product with ID {product.Id} not found.");

            // Update properties
            existingProduct.Name = product.Name;
            existingProduct.Picture = product.Picture;
            existingProduct.Description = product.Description;
            existingProduct.Status = product.Status;
            _demoContext.Products.Update(existingProduct);
            await _demoContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _demoContext.Products.FindAsync(id);
            if (product != null)
            {
                _demoContext.Products.Remove(product);
                await _demoContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductStatus>> GetAllProductStatusAsync()
        {
            var productStatus = await _demoContext.ProductStatuses.ToListAsync();
            return productStatus;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _demoContext.Categories.ToListAsync();
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(int take)
        {
            return await _demoContext.Products
                .OrderByDescending(x => x.Id)
                .Take(take)
                .Include(x => x.ProductItems)
                    .ThenInclude(x => x.VariationOptions)
                        .ThenInclude(x => x.Variation)
                .ToListAsync();
        }

        public async Task<Product?> GetNewestProductAsync()
        {
            return await _demoContext.Products
                .OrderByDescending(x => x.Id)
                .Include(x => x.ProductItems)
                    .ThenInclude(x => x.VariationOptions)
                        .ThenInclude(x => x.Variation)
                .FirstOrDefaultAsync();
        }
    }
}