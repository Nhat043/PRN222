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

        public IList<VariationOption> RamOptions { get; set; } = new List<VariationOption>();
        public IList<VariationOption> StorageOptions { get; set; } = new List<VariationOption>();

        public async Task OnGetAsync()
        {
            var allOptions = await _context.VariationOptions.Include(v => v.Variation).ToListAsync();
            RamOptions = allOptions.Where(vo => vo.Variation != null && vo.Variation.Name.ToLower() == "ram").ToList();
            StorageOptions = allOptions.Where(vo => vo.Variation != null && vo.Variation.Name.ToLower() == "storage").ToList();
        }
    }
}
