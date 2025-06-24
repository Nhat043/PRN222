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
    public class DetailsModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public DetailsModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

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
    }
}
