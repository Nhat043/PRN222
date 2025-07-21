using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;
using BLL.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;

namespace Razor.Pages.CommentPage
{
    public class IndexModel : PageModel
    {
        private readonly IComService _context;
        private readonly IHubContext<DataSignalR> _hubContext;
        public IndexModel(IComService context, IHubContext<DataSignalR> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IList<Comment> Comment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Comment = await _context.GetAllCommentsAsync();
        }
    }
}
