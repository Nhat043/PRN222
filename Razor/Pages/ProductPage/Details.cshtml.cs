using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using BLL.Service.Interface;

namespace Razor.Pages.ProductPage
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IProductItemService _productItemService;
        private readonly IVariationService _variationService;
        private readonly IProductItemStatusService _statusService;

        public DetailsModel(IProductService productService, IProductItemService productItemService, IVariationService variationService, IProductItemStatusService statusService)
        {
            _productService = productService;
            _productItemService = productItemService;
            _variationService = variationService;
            _statusService = statusService;
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

            var product = await _productService.GetProductByIdWithCategoryAndStatusAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
                ProductItems = await _productItemService.GetProductItemsByProductIdAsync(id.Value);
                Variations = await _variationService.GetAllVariationsWithOptionsAsync();
                Statuses = await _statusService.GetAllProductItemStatusesAsync();
            }
            return Page();
        }
        
        public async Task<IActionResult> OnGetEditProductItemAsync(int productId, int productItemId)
        {
            var productItem = await _productItemService.GetProductItemByIdAsync(productItemId);
            if (productItem == null || productItem.ProductId != productId)
            {
                return NotFound();
            }
            EditingProductItem = productItem;
            EditingProductItemId = productItemId;
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
            await _productItemService.AddProductItemWithVariationsAsync(NewProductItem, variationOptionIds);
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
            var existingProductItem = await _productItemService.GetProductItemByIdAsync(productItemId);
            if (existingProductItem == null || existingProductItem.ProductId != productId)
            {
                return NotFound();
            }
            // Update the basic properties
            existingProductItem.Quantity = EditingProductItem.Quantity;
            existingProductItem.ImportPrice = EditingProductItem.ImportPrice;
            existingProductItem.SellingPrice = EditingProductItem.SellingPrice;
            existingProductItem.Discount = EditingProductItem.Discount;
            existingProductItem.StatusId = EditingProductItem.StatusId;
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
            await _productItemService.UpdateProductItemWithVariationsAsync(existingProductItem, variationOptionIds);
            return RedirectToPage(new { id = productId });
        }
        
        public async Task<IActionResult> OnPostDeleteProductItemAsync(int productItemId, int productId)
        {
            await _productItemService.DeleteProductItemAsync(productItemId);
            return RedirectToPage(new { id = productId });
        }
    }
}
