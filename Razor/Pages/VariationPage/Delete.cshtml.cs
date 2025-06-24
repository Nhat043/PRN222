using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.VariationPage
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public DeleteModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Variation Variation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variation = await _context.Variations.FirstOrDefaultAsync(m => m.Id == id);

            if (variation == null)
            {
                return NotFound();
            }
            else
            {
                Variation = variation;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variation = await _context.Variations.FindAsync(id);
            if (variation != null)
            {
                Variation = variation;
                _context.Variations.Remove(Variation);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
