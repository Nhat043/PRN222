using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using BLL.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Razor.Models;

namespace Razor.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductItemService _productItemService;

        public IndexModel(IOrderService orderService, IProductItemService productItemService)
        {
            _orderService = orderService;
            _productItemService = productItemService;
        }

        public PaginatedList<Order> Orders { get; set; } = default!;
        public IList<OrderStatus> OrderStatuses { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? StatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10; // Default page size

        public int[] PageSizeOptions { get; set; } = { 5, 10, 20, 50 };

        public async Task OnGetAsync()
        {
            // Get all orders with filtering
            var allOrders = await _orderService.GetAllOrdersAsync();
            
            // Apply filters
            var filteredOrders = allOrders.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filteredOrders = filteredOrders.Where(o => 
                    o.Id.ToString().Contains(SearchTerm) ||
                    (o.User.Name != null && o.User.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (o.User.Email != null && o.User.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }
            
            if (StatusId.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.StatusId == StatusId.Value);
            }
            
            // Sort by status (pending first, then approved, then rejected) and then by date (newest first)
            filteredOrders = filteredOrders.OrderBy(o => o.StatusId == 1 ? 1 : 
                                                       o.StatusId == 2 ? 2 : 
                                                       o.StatusId == 3 ? 3 : 4)
                                         .ThenByDescending(o => o.Date);
            
            // Apply pagination
            Orders = PaginatedList<Order>.Create(filteredOrders, PageIndex, PageSize);
            
            // Get order statuses for filter dropdown
            OrderStatuses = await _orderService.GetAllOrderStatusAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, int statusId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                // Check if the order status is being changed to rejected (status ID 3)
                if (statusId == 3 && order.StatusId != 3)
                {
                    // Restore product item quantities for all order items
                    foreach (var orderItem in order.OrderItems)
                    {
                        var productItem = await _productItemService.GetProductItemByIdAsync(orderItem.ProductItemId.Value);
                        if (productItem != null)
                        {
                            // Restore the quantity that was deducted when the order was created
                            productItem.Quantity += orderItem.Quantity;
                            await _productItemService.UpdateProductItemAsync(productItem);
                        }
                    }
                }
                
                // Update the order status
                order.StatusId = statusId;
                await _orderService.UpdateOrderAsync(order);
            }
            return RedirectToPage();
        }
    }
}
