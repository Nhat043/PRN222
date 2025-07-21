using DAL.Models;

namespace DAL.Repository.Interface
{
    public interface IProductItemRepo
    {
        Task<IEnumerable<ProductItem>> GetAllProductItemsAsync();
        Task<ProductItem?> GetProductItemByIdAsync(int id);
        Task AddProductItemAsync(ProductItem item);
        Task UpdateProductItemAsync(ProductItem item);
        Task DeleteProductItemAsync(int id);
        Task<IList<ProductItem>> GetProductItemsByProductIdAsync(int productId);
        Task SetVariationOptionsAsync(int productItemId, List<int> variationOptionIds);
        Task<bool> HasForeignKeyDependenciesAsync(int id);
    }
}
