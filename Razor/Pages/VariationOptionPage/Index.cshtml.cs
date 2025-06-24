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
    public class IndexModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public IndexModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IList<VariationOption> VariationOption { get;set; } = default!;

        public async Task OnGetAsync()
        {
            VariationOption = await _context.VariationOptions
                .Include(v => v.Variation).ToListAsync();
        }
    }
}
