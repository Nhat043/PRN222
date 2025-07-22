using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Pages.CommentPage
{
    public class EditModel : PageModel
    {
        private readonly IComService _commentService;
        private readonly IProductService _productService;
        private readonly ICommentStatusService _statusService;
        private readonly IAccountService _accountService;
        private readonly IHubContext<DataSignalR> _hubContext;


        public EditModel(
            IComService commentService,
            IProductService productService,
            ICommentStatusService statusService,
            IAccountService accountService, 
            IHubContext<DataSignalR> hubContext)
        {
            _commentService = commentService;
            _productService = productService;
            _statusService = statusService;
            _accountService = accountService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();

            var comment = _commentService.GetById(id.Value);
            if (comment == null) return NotFound();

            Comment = comment;

            LoadSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

           
            var existingComment =  _commentService.GetById(Comment.Id);
            if (existingComment == null)
            {
                return NotFound();
            }

           
            existingComment.Content = Comment.Content;

           
            _commentService.UpdateComment(existingComment);
            await _hubContext.Clients.All.SendAsync("load");
            return RedirectToPage("./Index");
        }


        private void LoadSelectLists()
        {
            ViewData["ParentId"] = new SelectList(
    _commentService.GetProductComments(Comment.ProductId ?? 0), "Id", "Content");

            ViewData["ProductId"] = new SelectList(_productService.GetAllProductsAsync().Result, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_statusService.GetAll(), "Id", "Name");
            ViewData["UserId"] = new SelectList(_accountService.GetAllAccountsAsync().Result, "Id", "Email");
        }
    }
}
