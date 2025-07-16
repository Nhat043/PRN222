using DAL.Models;

namespace BLL.Service.Interface
{
    public interface IProductItemService
    {
        Task<ProductItem?> GetProductItemByIdAsync(int id);
        Task<IEnumerable<ProductItem>> GetAllProductItemsAsync();

        Task UpdateProductItemAsync(ProductItem productItem);
    }
}
