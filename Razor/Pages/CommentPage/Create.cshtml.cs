using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Pages.CommentPage
{
    public class CreateModel : PageModel
    {
        private readonly ICommentStatusService _statusService;
        private readonly IComService _commentService;
        private readonly IProductService _productService;
        private readonly IAccountService _accountService;
        public CreateModel(
            ICommentStatusService statusService,
            IComService commentService,
            IProductService productService, 
            IAccountService accountService)
        {
            _statusService = statusService;
            _commentService = commentService;
            _productService = productService;
            _accountService = accountService;
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? ParentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ProductId { get; set; }

        public string? ReplyingToUserName { get; set; }
        public bool IsReplyMode { get; set; } = false;

        public IActionResult OnGet()
        {
            if (ParentId.HasValue)
            {
                var parent = _commentService.GetById(ParentId.Value);
                if (parent == null)
                    return NotFound();

                Comment = new Comment
                {
                    ParentId = parent.Id,
                    ProductId = parent.ProductId,
                    StatusId = _statusService.GetIdByName("Visible") ?? 1
                };

                ReplyingToUserName = parent.User?.Name;
                IsReplyMode = true;
            }
            else
            {
                Comment = new Comment
                {
                    StatusId = _statusService.GetIdByName("Visible") ?? 1
                };
            }

            LoadSelectLists();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                return Page();
            }

           

            Comment.CreatedAt = DateTime.Now;
           
            Comment.UserId = 1;


            _commentService.CreateComment(Comment);
            return RedirectToPage("./Index");
        }

        private void LoadSelectLists()
        {
            ViewData["ParentId"] = new SelectList(_commentService.GetProductComments(ProductId ?? 0), "Id", "Content");
            ViewData["ProductId"] = new SelectList(_productService.GetAllProductsAsync().Result, "Id", "Name");
            
            ViewData["StatusId"] = new SelectList(_statusService.GetAll(), "Id", "Name");
            ViewData["UserId"] = new SelectList(_accountService.GetAllAccountsAsync().Result, "Id", "Email");
        }
    }
}
