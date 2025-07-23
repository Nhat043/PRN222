using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;
using BLL.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;

namespace Razor.Pages.CategoryPage
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IHubContext<VarianSignalR> _hubContext;

        public EditModel(ICategoryService categoryService, IHubContext<VarianSignalR> hubContext)
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
            Category = category;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(Category);
                await _hubContext.Clients.All.SendAsync("load");
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                // Handle duplicate category name error
                ModelState.AddModelError("Category.Name", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                // Handle other errors
                ModelState.AddModelError("", "An error occurred while updating the category. Please try again.");
                return Page();
            }
        }

        private async Task<bool> CategoryExists(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category != null;
        }
    }
}
