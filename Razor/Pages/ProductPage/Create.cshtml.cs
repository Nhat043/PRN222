using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using BLL.Service.Interface;
using BLL.Util;

namespace Razor.Pages.ProductPage
{
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHost;

        public CreateModel(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHost)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHost = webHost;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            var statuses = await _productService.GetAllProductStatusAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
        ViewData["StatusId"] = new SelectList(statuses, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("On POST");
            if (!ModelState.IsValid)
            {
                var categories = await _productService.GetAllCategoriesAsync();
                var statuses = await _productService.GetAllProductStatusAsync();
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
                ViewData["StatusId"] = new SelectList(statuses, "Id", "Name");
                return Page();
            }
            Console.WriteLine("Start to process image");
            try
            {
                // Xử lý lưu hình ảnh
                if (UploadFile != null)
                {
                    var fileName = await ImageHelper.UploadImageAsync(UploadFile, Path.Combine(_webHost.WebRootPath, "images"));
                    Product.Picture = fileName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error process image: " + ex.Message);
            }
            

            await _productService.AddProductAsync(Product);

            return RedirectToPage("./Index");
        }
    }
}
