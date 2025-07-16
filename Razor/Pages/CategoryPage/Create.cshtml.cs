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

namespace Razor.Pages.CategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
