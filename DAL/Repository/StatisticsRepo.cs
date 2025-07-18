using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class StatisticsRepo : IStatisticsRepo
    {
        private readonly DemoContext _context;

        public StatisticsRepo(DemoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsWithRelatedDataAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.ProductItem)
                    .ThenInclude(pi => pi.Product)
                .ToListAsync();
        }
    }
} 