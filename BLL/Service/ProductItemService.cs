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
    }
}
