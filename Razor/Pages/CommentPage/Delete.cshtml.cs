using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using BLL.Service.Interface;

namespace Razor.Pages.CommentPage
{
    public class DeleteModel : PageModel
    {
        private readonly IComService _commentService;

        public DeleteModel(IComService commentService)
        {
            _commentService = commentService;
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

            _commentService.HideComment(id.Value); // ❗ Soft delete (statusId = 2)
            return RedirectToPage("./Index");
        }
    }
}
