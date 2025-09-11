using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;
using BLL.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;

namespace Razor.Pages.CategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IHubContext<VarianSignalR> _hubContext;

        public CreateModel(ICategoryService categoryService, IHubContext<VarianSignalR> hubContext)
        {
            _categoryService = categoryService;
            _hubContext = hubContext;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _categoryService.AddCategoryAsync(Category);
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
                ModelState.AddModelError("", "An error occurred while creating the category. Please try again.");
                return Page();
            }
        }
    }
}
