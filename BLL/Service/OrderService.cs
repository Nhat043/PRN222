using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace BLL.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductItemRepo _productItemRepo;
        private static HubConnection? _connection;
        private bool _connected = false;
        public OrderService(IOrderRepository orderRepository, IProductItemRepo productItemRepo)
        {
            _orderRepository = orderRepository;
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7082/DataSignalRChanel") // SignalR Host
                .WithAutomaticReconnect()
                .Build();
            _productItemRepo = productItemRepo;
        }

        public async Task<Boolean> CheckQuantity(List<OrderItem> listOrderItem)
        {
         
            foreach(var productItem in listOrderItem)
            {
                var checkProQuantity = await _productItemRepo.GetProductItemByIdAsync((int)productItem.ProductItemId);
                if (productItem.Quantity > checkProQuantity.Quantity)
                    return false;
            }
            return true;
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
        public async Task NotifyAdminNewOrder()
        {
            if (!_connected || _connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
                _connected = true;
            }
            await _connection.InvokeAsync("NotifyAdminNewOrder");
        }

        public async Task NotifyProductQuantityChanged(int productId)
        {
            if (!_connected || _connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
                _connected = true;
            }
            await _connection.InvokeAsync("NotifyProductQuantityChanged", productId);
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetPaginatedOrdersByUserIdAsync(int userId, int pageIndex, int pageSize)
        {
            var allOrders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var totalCount = allOrders.Count;
            var pagedOrders = allOrders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return (pagedOrders, totalCount);
        }

        public async Task<List<OrderStatus>> GetAllOrderStatusAsync()
        {
            return await _orderRepository.GetAllOrderStatusAsync();
        }
    }
}
