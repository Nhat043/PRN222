using DAL.Models;

namespace BLL.Service.Interface
{
    public interface IProductItemService
    {
        Task<ProductItem?> GetProductItemByIdAsync(int id);
        Task<IEnumerable<ProductItem>> GetAllProductItemsAsync();

        Task UpdateProductItemAsync(ProductItem productItem);
        Task<IList<ProductItem>> GetProductItemsByProductIdAsync(int productId);
        Task AddProductItemWithVariationsAsync(ProductItem item, List<int> variationOptionIds);
        Task UpdateProductItemWithVariationsAsync(ProductItem item, List<int> variationOptionIds);
        Task DeleteProductItemAsync(int productItemId);
        Task<bool> HasForeignKeyDependenciesAsync(int id);
    }
}
