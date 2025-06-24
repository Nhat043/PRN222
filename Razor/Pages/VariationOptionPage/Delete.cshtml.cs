using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.VariationOptionPage
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public DeleteModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VariationOption VariationOption { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variationoption = await _context.VariationOptions.FirstOrDefaultAsync(m => m.Id == id);

            if (variationoption == null)
            {
                return NotFound();
            }
            else
            {
                VariationOption = variationoption;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variationoption = await _context.VariationOptions.FindAsync(id);
            if (variationoption != null)
            {
                VariationOption = variationoption;
                _context.VariationOptions.Remove(VariationOption);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
