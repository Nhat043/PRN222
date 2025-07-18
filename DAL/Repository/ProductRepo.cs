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

        public async Task<Product?> GetProductByIdWithAvailableItemsAsync(int id)
        {
            var product = await _demoContext.Products
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.StatusId == 1);

            if (product != null)
            {
                product.ProductItems = product.ProductItems
                    .Where(pi => pi.StatusId == 1 && pi.Quantity > 0)
                    .ToList();
            }
            return product;
        }

        public async Task<Product> GetProductByIdWithCategoryAndStatusAsync(int productId)
        {
            return await _demoContext.Products
                .Include(p => p.Category)
                .Include(p => p.Status)
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

        public async Task<bool> HasForeignKeyDependenciesAsync(int id)
        {
            return await _demoContext.ProductItems.AnyAsync(pi => pi.ProductId == id) ||
                   await _demoContext.Comments.AnyAsync(c => c.ProductId == id) ||
                   await _demoContext.Ratings.AnyAsync(r => r.ProductId == id);
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
                .Where(p => p.StatusId == 1 &&
                    p.ProductItems.Any(pi => pi.StatusId == 1 && pi.Quantity > 0))
                .OrderByDescending(p => p.Id)
                .Take(take)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                .ToListAsync();
        }

        public async Task<Product?> GetNewestProductInStockAsync()
        {
            return await _demoContext.Products
                .Where(p => p.StatusId == 1 &&
                    p.ProductItems.Any(pi => pi.StatusId == 1 && pi.Quantity > 0))
                .OrderByDescending(p => p.Id)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetAllProductsFullAsync()
        {
            return await _demoContext.Products
                .Include(p => p.ProductItems)
                    .ThenInclude(i => i.VariationOptions)
                        .ThenInclude(v => v.Variation)
                .Include(p => p.Category)
                .Include(p => p.Ratings)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllRamOptionsAsync()
        {
            return await _demoContext.VariationOptions
                .Where(x => x.Variation.Name == "RAM")
                .Select(x => x.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllRomOptionsAsync()
        {
            return await _demoContext.VariationOptions
                .Where(x => x.Variation.Name == "STORAGE")
                .Select(x => x.Value)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task<List<Product>> GetFilteredProductsAsync(string search, string ram, string rom, string price, int? categoryId)
        {
            var query = _demoContext.Products
                .Where(p => p.StatusId == 1 &&
                    p.ProductItems.Any(pi => pi.StatusId == 1 && pi.Quantity > 0))
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                .Include(p => p.Category)
                .Include(p => p.Ratings)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            if (!string.IsNullOrEmpty(ram))
                query = query.Where(p => p.ProductItems.Any(i =>
                    i.VariationOptions.Any(v => v.Variation.Name == "RAM" && v.Value == ram) &&
                    i.StatusId == 1 && i.Quantity > 0));

            if (!string.IsNullOrEmpty(rom))
                query = query.Where(p => p.ProductItems.Any(i =>
                    i.VariationOptions.Any(v => v.Variation.Name == "STORAGE" && v.Value == rom) &&
                    i.StatusId == 1 && i.Quantity > 0));

            if (!string.IsNullOrEmpty(price))
            {
                var split = price.Split('-');
                if (split.Length == 2 && int.TryParse(split[0], out var min) && int.TryParse(split[1], out var max))
                {
                    query = query.Where(p => p.ProductItems.Any(i => i.SellingPrice >= min && i.SellingPrice <= max && i.StatusId == 1 && i.Quantity > 0));
                }
            }

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(p => p.CategoryId == categoryId);

            return await query.ToListAsync();
        }
    }
}