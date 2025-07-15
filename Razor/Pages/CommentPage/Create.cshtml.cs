using DAL.Datas;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Pages.CommentPage
{
    public class CreateModel : PageModel
    {
        private readonly DemoContext _context;

        public CreateModel(DemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? ParentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ProductId { get; set; }

        public string? ReplyingToUserName { get; set; }
        public bool IsReplyMode { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            int visibleStatusId = await _context.CommentStatuses
                .Where(s => s.Name == "Visible")
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (ParentId.HasValue)
            {
                var parent = await _context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == ParentId);

                if (parent == null)
                    return NotFound();

                Comment = new Comment
                {
                    ParentId = parent.Id,
                    ProductId = parent.ProductId,
                    StatusId = visibleStatusId
                };

                ReplyingToUserName = parent.User?.Name;
                IsReplyMode = true;
            }
            else
            {
                Comment = new Comment
                {
                    StatusId = visibleStatusId
                };
            }

            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            var accountId = HttpContext.Session.GetInt32("AccountIdSession");
            if (accountId == null)
                return RedirectToPage("/Account/Login"); // đổi đúng route nếu cần

            Comment.CreatedAt = DateTime.Now;
            Comment.UserId = accountId.Value;

            _context.Comments.Add(Comment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
            ViewData["ParentId"] = new SelectList(await _context.Comments.ToListAsync(), "Id", "Content");
            ViewData["ProductId"] = new SelectList(await _context.Products.ToListAsync(), "Id", "Name");
            ViewData["StatusId"] = new SelectList(await _context.CommentStatuses.ToListAsync(), "Id", "Name");
            ViewData["UserId"] = new SelectList(await _context.Accounts.ToListAsync(), "Id", "Email");
        }
    }
}
