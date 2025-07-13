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
using BLL.Util;

namespace Razor.Pages.ProductPage
{
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHost;

        public EditModel(IProductService productService, IWebHostEnvironment webHost)
        {
            _productService = productService;
            _webHost = webHost;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

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
                var categories = await _productService.GetAllCategoriesAsync();
                var statuses = await _productService.GetAllProductStatusAsync();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
                ViewData["StatusId"] = new SelectList(statuses, "Id", "Name");
                return Page();
            }

            try
            {
                // Handle image upload
                if (UploadFile != null)
                {
                    var productName = Product.Name;
                    var sharedImagePath = Path.Combine(_webHost.ContentRootPath, "..", "SharedImages");
                    var fileName = await ImageHelper.UploadImageAsync(UploadFile, sharedImagePath, productName);
                    Product.Picture = "/SharedImages/" + fileName;
                }
                
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
