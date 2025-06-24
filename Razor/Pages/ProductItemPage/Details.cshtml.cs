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
    public class DetailsModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public DetailsModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public ProductItem ProductItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productitem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .Include(pi => pi.Product)
                .Include(pi => pi.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
                
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
    }
}
