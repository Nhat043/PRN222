using BLL.Service.Interface;
using DAL.Datas;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Razor.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Pages_AccountPage
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IList<Account> Account { get;set; } = default!;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            Account = (await _accountService.GetAllAccountsAsync()).ToList();
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

            // Notify clients about the ban action
            await _accountService.NotifyBanAccountAsync();

            return RedirectToPage();
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
            return RedirectToPage();
        }
    }
}
