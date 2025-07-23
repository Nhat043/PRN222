using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;
using System.Threading.Tasks;

namespace Razor.Pages.CommentPage
{
    public class DeleteModel : PageModel
    {
        private readonly IComService _commentService;
        private readonly IHubContext<CommentSignalR> _hubContext;

        public DeleteModel(IComService commentService, IHubContext<CommentSignalR> hubContext)
        {
            _commentService = commentService;
            _hubContext = hubContext;
        }

        [BindProperty]
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

        public IActionResult OnPost(int? id)
        {
            if (id == null)
                return NotFound();


            _commentService.HideComment(id.Value); 
            _hubContext.Clients.All.SendAsync("load");


            return RedirectToPage("./Index");
        }
    }
}
