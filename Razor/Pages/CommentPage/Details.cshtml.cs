using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Service.Interface;
using DAL.Models;

namespace Razor.Pages.CommentPage
{
    public class DetailsModel : PageModel
    {
        private readonly IComService _commentService;

        public DetailsModel(IComService commentService)
        {
            _commentService = commentService;
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
