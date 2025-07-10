using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.CommentPage
{
    public class IndexModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public IndexModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IList<Comment> Comment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Comment = await _context.Comments
                .Include(c => c.Parent)
                .Include(c => c.Product)
                .Include(c => c.Status)
                .Include(c => c.User).ToListAsync();
        }
    }
}
