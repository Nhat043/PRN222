using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.VariationPage
{
    public class EditModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public EditModel(DAL.Datas.DemoContext context)
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

            var variation =  await _context.Variations.FirstOrDefaultAsync(m => m.Id == id);
            if (variation == null)
            {
                return NotFound();
            }
            Variation = variation;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Variation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariationExists(Variation.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VariationExists(int id)
        {
            return _context.Variations.Any(e => e.Id == id);
        }
    }
}
