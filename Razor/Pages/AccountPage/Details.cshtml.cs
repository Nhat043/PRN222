using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;
using BLL.Service.Interface;
using Razor.Models;

namespace Razor.Pages_AccountPage
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IOrderService _orderService;

        public DetailsModel(IAccountService accountService, IOrderService orderService)
        {
            _accountService = accountService;
            _orderService = orderService;
        }

        public Account Account { get; set; } = default!;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public PaginatedList<Order> OrderHistory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, int pageIndex = 1, int pageSize = 5)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                Account = await _accountService.GetAccountByIdAsync(id.Value);
                var (orders, totalCount) = await _orderService.GetPaginatedOrdersByUserIdAsync(id.Value, pageIndex, pageSize);
                OrderHistory = new PaginatedList<Order>(orders, totalCount, pageIndex, pageSize);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostBanAsync(int id)
        {
            var success = await _accountService.BanAccountAsync(id);
            if (!success)
            {
                ErrorMessage = "Cannot ban admin accounts or account not found.";
            }
            else
            {
                SuccessMessage = "Account banned successfully.";
            }
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostUnbanAsync(int id)
        {
            var success = await _accountService.UnbanAccountAsync(id);
            if (!success)
            {
                ErrorMessage = "Account not found.";
            }
            else
            {
                SuccessMessage = "Account unbanned successfully.";
            }
            return RedirectToPage(new { id });
        }
    }
}
