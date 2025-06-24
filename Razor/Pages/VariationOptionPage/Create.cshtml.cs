using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.VariationOptionPage
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
        ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public VariationOption VariationOption { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.VariationOptions.Add(VariationOption);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
