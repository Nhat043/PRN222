using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages_AccountPage
{
    public class CreateModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public CreateModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["RoleId"] = new SelectList(_context.RoleNames, "Id", "Name");
        ViewData["StatusId"] = new SelectList(_context.AccountStatuses, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Accounts.Add(Account);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
