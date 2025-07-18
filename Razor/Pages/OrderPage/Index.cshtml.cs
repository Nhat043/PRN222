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

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
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
                order.StatusId = statusId;
                await _orderService.UpdateOrderAsync(order);
            }
            return RedirectToPage();
        }
    }
}
