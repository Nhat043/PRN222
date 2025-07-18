using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;

namespace BLL.Service
{
    public class ProductItemService : IProductItemService
    {
        private readonly IProductItemRepo _productItemRepo;

        public ProductItemService(IProductItemRepo productItemRepo)
        {
            _productItemRepo = productItemRepo;
        }

        public async Task<ProductItem?> GetProductItemByIdAsync(int id)
        {
            return await _productItemRepo.GetProductItemByIdAsync(id);
        }

        public async Task<IEnumerable<ProductItem>> GetAllProductItemsAsync()
        {
            return await _productItemRepo.GetAllProductItemsAsync();
        }

        public async Task UpdateProductItemAsync(ProductItem productItem)
        {
            await _productItemRepo.UpdateProductItemAsync(productItem);
        }

        public async Task<IList<ProductItem>> GetProductItemsByProductIdAsync(int productId)
        {
            return await _productItemRepo.GetProductItemsByProductIdAsync(productId);
        }

        public async Task AddProductItemWithVariationsAsync(ProductItem item, List<int> variationOptionIds)
        {
            await _productItemRepo.AddProductItemAsync(item);
            if (variationOptionIds != null && variationOptionIds.Count > 0)
            {
                await _productItemRepo.SetVariationOptionsAsync(item.Id, variationOptionIds);
            }
        }

        public async Task UpdateProductItemWithVariationsAsync(ProductItem item, List<int> variationOptionIds)
        {
            await _productItemRepo.UpdateProductItemAsync(item);
            await _productItemRepo.SetVariationOptionsAsync(item.Id, variationOptionIds);
        }

        public async Task DeleteProductItemAsync(int productItemId)
        {
            // Check for foreign key dependencies before deletion
            if (await HasForeignKeyDependenciesAsync(productItemId))
            {
                throw new InvalidOperationException("Cannot delete product item. It has related order items. Please remove all related orders first.");
            }

            await _productItemRepo.DeleteProductItemAsync(productItemId);
        }

        public async Task<bool> HasForeignKeyDependenciesAsync(int id)
        {
            return await _productItemRepo.HasForeignKeyDependenciesAsync(id);
        }
    }
}
