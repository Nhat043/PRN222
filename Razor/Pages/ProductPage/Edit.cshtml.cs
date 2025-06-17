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

namespace Razor.Pages.ProductPage
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;

        public EditModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
            var categories = await _productService.GetAllCategoriesAsync();
            var statuses = await _productService.GetAllProductStatusAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            ViewData["StatusId"] = new SelectList(statuses, "Id", "Name");
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
                await _productService.UpdateProductAsync(Product);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> ProductExists(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null;
        }
    }
}
