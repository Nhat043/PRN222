using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class ProductItemStatusService : IProductItemStatusService
    {
        private readonly IProductItemStatusRepo _statusRepo;
        public ProductItemStatusService(IProductItemStatusRepo statusRepo)
        {
            _statusRepo = statusRepo;
        }
        public async Task<IList<ProductItemStatus>> GetAllProductItemStatusesAsync()
        {
            return await _statusRepo.GetAllProductItemStatusesAsync();
        }
    }
} 