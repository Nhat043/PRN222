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

namespace Razor.Pages.ProductPage
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly DemoContext _context;

        public DetailsModel(IProductService productService, DemoContext context)
        {
            _productService = productService;
            _context = context;
        }

        public Product Product { get; set; } = default!;
        public IList<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
        
        [BindProperty]
        public ProductItem NewProductItem { get; set; } = new ProductItem();
        
        [BindProperty]
        public ProductItem EditingProductItem { get; set; } = new ProductItem();
        
        public int? EditingProductItemId { get; set; }
        
        public IList<Variation> Variations { get; set; } = new List<Variation>();
        public IList<ProductItemStatus> Statuses { get; set; } = new List<ProductItemStatus>();

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
            else
            {
                Product = product;
                
                // Load product items for this product
                ProductItems = await _context.ProductItems
                    .Include(pi => pi.Status)
                    .Include(pi => pi.VariationOptions)
                    .Where(pi => pi.ProductId == id.Value)
                    .ToListAsync();
                
                // Load variations and statuses for the form
                Variations = await _context.Variations
                    .Include(v => v.VariationOptions)
                    .ToListAsync();
                    
                Statuses = await _context.ProductItemStatuses.ToListAsync();
            }
            return Page();
        }
        
        public async Task<IActionResult> OnGetEditProductItemAsync(int productId, int productItemId)
        {
            var productItem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                .FirstOrDefaultAsync(pi => pi.Id == productItemId && pi.ProductId == productId);
                
            if (productItem == null)
            {
                return NotFound();
            }
            
            EditingProductItem = productItem;
            EditingProductItemId = productItemId;
            
            // Load the product and other data
            await OnGetAsync(productId);
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostCreateProductItemAsync(int productId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage(new { id = productId });
            }

            NewProductItem.ProductId = productId;
            
            // Add the product item first
            _context.ProductItems.Add(NewProductItem);
            await _context.SaveChangesAsync();
            
            // Handle variation options if any were selected
            var form = await Request.ReadFormAsync();
            var variationOptionIds = new List<int>();
            
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("SelectedVariationOptionIds["))
                {
                    var value = form[key].ToString();
                    if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int optionId))
                    {
                        variationOptionIds.Add(optionId);
                    }
                }
            }
            
            // Add variation options to the product item
            if (variationOptionIds.Any())
            {
                var variationOptions = await _context.VariationOptions
                    .Where(vo => variationOptionIds.Contains(vo.Id))
                    .ToListAsync();
                
                // Add each variation option individually
                foreach (var option in variationOptions)
                {
                    NewProductItem.VariationOptions.Add(option);
                }
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage(new { id = productId });
        }
        
        public async Task<IActionResult> OnPostUpdateProductItemAsync(int productId, int productItemId)
        {
            if (!ModelState.IsValid)
            {
                EditingProductItemId = productItemId;
                await OnGetAsync(productId);
                return Page();
            }

            var existingProductItem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                .FirstOrDefaultAsync(pi => pi.Id == productItemId && pi.ProductId == productId);
                
            if (existingProductItem == null)
            {
                return NotFound();
            }
            
            // Update the basic properties
            existingProductItem.Quantity = EditingProductItem.Quantity;
            existingProductItem.ImportPrice = EditingProductItem.ImportPrice;
            existingProductItem.SellingPrice = EditingProductItem.SellingPrice;
            existingProductItem.Discount = EditingProductItem.Discount;
            existingProductItem.StatusId = EditingProductItem.StatusId;
            
            // Clear existing variation options
            existingProductItem.VariationOptions.Clear();
            
            // Handle new variation options
            var form = await Request.ReadFormAsync();
            var variationOptionIds = new List<int>();
            
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("EditingVariationOptionIds["))
                {
                    var value = form[key].ToString();
                    if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int optionId))
                    {
                        variationOptionIds.Add(optionId);
                    }
                }
            }
            
            // Add new variation options
            if (variationOptionIds.Any())
            {
                var variationOptions = await _context.VariationOptions
                    .Where(vo => variationOptionIds.Contains(vo.Id))
                    .ToListAsync();
                
                foreach (var option in variationOptions)
                {
                    existingProductItem.VariationOptions.Add(option);
                }
            }
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage(new { id = productId });
        }
        
        public async Task<IActionResult> OnPostDeleteProductItemAsync(int productItemId, int productId)
        {
            var productItem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                .FirstOrDefaultAsync(pi => pi.Id == productItemId);
                
            if (productItem != null)
            {
                // Remove variation options first
                productItem.VariationOptions.Clear();
                await _context.SaveChangesAsync();
                
                // Then remove the product item
                _context.ProductItems.Remove(productItem);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToPage(new { id = productId });
        }
    }
}
