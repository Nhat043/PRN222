using BLL.Service.Interface;
using BLL.Util;
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
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;

        public ProductService(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepo.GetAllProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(id));

            var product = await _productRepo.GetProductByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException($"User with ID {id} not found.");

            return product;
        }
        private void ValidateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
        }
        public async Task AddProductAsync(Product product)
        {
            // Business logic validation
            ValidateProduct(product);

            // Additional business rules can be added here
            // For example: Check for duplicate product names, validate price ranges, etc.
            await _productRepo.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            // Business logic validation
            ValidateProduct(product);

            if (product.Id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(product.Id));

            // Additional business rules for updates can be added here

            await _productRepo.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            // Additional business logic can be added here
            // For example: Check if product is referenced in orders before deletion

            await _productRepo.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<ProductStatus>> GetAllProductStatusAsync()
        {
            return await _productRepo.GetAllProductStatusAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _productRepo.GetAllCategoriesAsync();
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(int take)
        {
            return await _productRepo.GetFeaturedProductsAsync(take);
        }

        public async Task<Product?> GetNewestProductAsync()
        {
            return await _productRepo.GetNewestProductAsync();
        }

    }
}