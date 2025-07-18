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
    public class OrderRepository : IOrderRepository
    {
        private readonly DemoContext _dbContext;

        public OrderRepository(DemoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _dbContext.Orders
       .Include(o => o.Status)
       .Include(o => o.User)
       .ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrderItemsAsync(List<OrderItem> orderItems)  
        {
            _dbContext.OrderItems.AddRange(orderItems);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductItem)
                        .ThenInclude(pi => pi.Product)
                        .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task UpdateOrderAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId) // ✅ CORRECT
                .Include(o => o.User)
                .Include(o => o.Status)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<List<OrderStatus>> GetAllOrderStatusAsync()
        {
            return await _dbContext.OrderStatuses.ToListAsync();
        }
    }
}
