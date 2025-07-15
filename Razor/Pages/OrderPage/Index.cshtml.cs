using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using BLL.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Razor.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<Order> Orders { get; set; }

        public async Task OnGetAsync()
        {
            Orders = await _orderService.GetAllOrdersAsync();
        }
        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, int statusId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                order.StatusId = statusId;
                await _orderService.UpdateOrderAsync(order); // Create this method
            }
            return RedirectToPage();
        }

    }
}
