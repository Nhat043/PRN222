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

namespace Razor.Pages.VariationOptionPage
{
    public class EditModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public EditModel(DAL.Datas.DemoContext context)
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

            var variationoption =  await _context.VariationOptions.FirstOrDefaultAsync(m => m.Id == id);
            if (variationoption == null)
            {
                return NotFound();
            }
            VariationOption = variationoption;
           ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
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

            _context.Attach(VariationOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariationOptionExists(VariationOption.Id))
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

        private bool VariationOptionExists(int id)
        {
            return _context.VariationOptions.Any(e => e.Id == id);
        }
    }
}
