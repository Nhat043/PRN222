using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task AddOrderAsync(Order order);

        Task AddOrderItemsAsync(List<OrderItem> orderItems);
        Task<Order> GetOrderByIdAsync(int id);
        Task UpdateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<OrderStatus>> GetAllOrderStatusAsync();
    }
}
