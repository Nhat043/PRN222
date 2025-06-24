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
    public class IndexModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public IndexModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IList<ProductItem> ProductItem { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ProductItem = await _context.ProductItems
                .Include(p => p.Product)
                .Include(p => p.Status)
                .Include(p => p.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .ToListAsync();
        }
    }
}
