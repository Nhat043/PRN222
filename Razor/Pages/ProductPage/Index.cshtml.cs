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
using Razor.Models;

namespace Razor.Pages.ProductPage
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public IndexModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public PaginatedList<Product> Product { get; set; } = default!;
        public IList<Category> Categories { get; set; } = default!;
        public IList<ProductStatus> ProductStatuses { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? StatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10; // Default page size

        public int[] PageSizeOptions { get; set; } = { 5, 10, 20, 50 };

        public async Task OnGetAsync()
        {
            // Get all products with filtering
            var allProducts = await _productService.GetAllProductsAsync();
            
            // Apply filters
            var filteredProducts = allProducts.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (CategoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == CategoryId.Value);
            }
            
            if (StatusId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.StatusId == StatusId.Value);
            }
            
            // Apply pagination
            Product = PaginatedList<Product>.Create(filteredProducts, PageIndex, PageSize);
            
            // Get categories and statuses for filter dropdowns
            Categories = (IList<Category>)await _categoryService.GetAllCategoriesAsync();
            ProductStatuses = (IList<ProductStatus>)await _productService.GetAllProductStatusAsync();
        }
    }
}
