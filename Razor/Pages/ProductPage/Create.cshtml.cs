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
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
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
                ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
                return Page();
            }
            Console.WriteLine("Start to process image");
            try
            {
                // Xử lý lưu hình ảnh
                if (UploadFile != null)
                {
                    const long maxFileSize = 4 * 1024 * 1024; // 2MB
                    if (UploadFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("UploadFile", "Ảnh phải nhỏ hơn 2MB.");
                        return await ReloadFormAsync();
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(UploadFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("UploadFile", "Chỉ cho phép ảnh .jpg, .jpeg, .png, .gif.");
                        return await ReloadFormAsync();
                    }

                    var safeFileName = CleanFileName(Product.Name ?? "image", extension);

                    var sharedImagePath = Path.Combine(_webHost.ContentRootPath, "..", "SharedImages");

                    if (!Directory.Exists(sharedImagePath))
                    {
                        Directory.CreateDirectory(sharedImagePath);
                    }

                    var filePath = Path.Combine(sharedImagePath, safeFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await UploadFile.CopyToAsync(stream);
                    }

                    Product.Picture = "/SharedImages/" + safeFileName;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("error process image: " + ex.Message);
            }
            
            // Set default status to "available" (id=1)
            Product.StatusId = 1;

            await _productService.AddProductAsync(Product);

            return RedirectToPage("./Index");
        }
        private string CleanFileName(string name, string extension)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

            if (cleaned.Length > 100)
                cleaned = cleaned.Substring(0, 100);

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{cleaned}_{timestamp}{extension}";
        }
        private async Task<IActionResult> ReloadFormAsync()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return Page();
        }

    }
}
