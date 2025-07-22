using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();

        Task AddOrderAsync(Order order);

        Task AddOrderItemsAsync(List<OrderItem> orderItems);
        Task<Order> GetOrderByIdAsync(int id);
        Task UpdateOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task NotifyAdminNewOrder();
        Task<List<OrderStatus>> GetAllOrderStatusAsync();
        Task NotifyProductQuantityChanged(int productId);
    }
}
