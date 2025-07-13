using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages_AccountPage
{
    public class IndexModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public IndexModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IList<Account> Account { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Status).ToListAsync();
        }

        public async Task<IActionResult> OnPostBanAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.StatusId = 2; // 2 = banned
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.StatusId = 1; // 1 = active
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
