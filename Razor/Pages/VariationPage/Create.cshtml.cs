using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.VariationPage
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
            return Page();
        }

        [BindProperty]
        public Variation Variation { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Variations.Add(Variation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
