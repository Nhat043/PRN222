using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.ProductItemPage
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public DeleteModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductItem ProductItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productitem = await _context.ProductItems.FirstOrDefaultAsync(m => m.Id == id);

            if (productitem == null)
            {
                return NotFound();
            }
            else
            {
                ProductItem = productitem;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productitem = await _context.ProductItems.FindAsync(id);
            if (productitem != null)
            {
                ProductItem = productitem;
                _context.ProductItems.Remove(ProductItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
