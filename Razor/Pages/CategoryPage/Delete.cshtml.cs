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

namespace Razor.Pages.CategoryPage
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IHubContext<DataSignalR> _hubContext;

        public DeleteModel(ICategoryService categoryService, IHubContext<DataSignalR> hubContext)
        {
            _categoryService = categoryService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                await _categoryService.DeleteCategoryAsync(id.Value);
                await _hubContext.Clients.All.SendAsync("load");
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                // Handle foreign key constraint error
                ModelState.AddModelError("", ex.Message);
                // Reload the category for display
                var category = await _categoryService.GetCategoryByIdAsync(id.Value);
                if (category != null)
                {
                    Category = category;
                }
                return Page();
            }
        }
    }
}
