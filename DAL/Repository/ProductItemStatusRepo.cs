using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class ProductItemStatusRepo : IProductItemStatusRepo
    {
        private readonly DemoContext _context;
        public ProductItemStatusRepo(DemoContext context)
        {
            _context = context;
        }
        public async Task<IList<ProductItemStatus>> GetAllProductItemStatusesAsync()
        {
            return await _context.ProductItemStatuses.ToListAsync();
        }
    }
} 