using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            await _orderRepository.AddOrderAsync(order);
        }

        public async Task AddOrderItemsAsync(List<OrderItem> orderItems) 
        {
            await _orderRepository.AddOrderItemsAsync(orderItems);
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }
        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateOrderAsync(order);
        }
        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

    }

}
