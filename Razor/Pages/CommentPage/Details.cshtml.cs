using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;
using System.Threading.Tasks;

namespace Razor.Pages.CommentPage
{
    public class DetailsModel : PageModel
    {
        private readonly IComService _commentService;
        private readonly IHubContext<DataSignalR> _hubContext;


        public DetailsModel(IComService commentService, IHubContext<DataSignalR> hubContext)
        {
            _commentService = commentService;
            _hubContext = hubContext;
        }

        public Comment Comment { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
                return NotFound();

            var comment = _commentService.GetById(id.Value);
            if (comment == null)
                return NotFound();

            Comment = comment;
            return Page();
        }
    }
}
