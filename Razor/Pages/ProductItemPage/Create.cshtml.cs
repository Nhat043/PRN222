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

namespace Razor.Pages.ProductItemPage
{
    public class CreateModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public CreateModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            // Removed StatusId dropdown
            
            // Group variation options by variation name for better organization
            var variationsWithOptions = _context.Variations
                .Select(v => new
                {
                    VariationName = v.Name,
                    Options = v.VariationOptions.Select(vo => new
                    {
                        Id = vo.Id,
                        Value = vo.Value,
                        DisplayText = $"{v.Name}: {vo.Value}"
                    }).ToList()
                })
                .ToList();

            ViewData["VariationsWithOptions"] = variationsWithOptions;
            
            return Page();
        }

        [BindProperty]
        public ProductItem ProductItem { get; set; } = new ProductItem
        {
            Quantity = 0,
            ImportPrice = 0,
            SellingPrice = 0,
            Discount = 0
        };

        // Key: VariationName, Value: Selected Option Id
        [BindProperty]
        public Dictionary<string, int> SelectedVariationOptionIds { get; set; } = new Dictionary<string, int>();

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate ViewData for validation errors
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                var variationsWithOptions = _context.Variations
                    .Select(v => new
                    {
                        VariationName = v.Name,
                        Options = v.VariationOptions.Select(vo => new
                        {
                            Id = vo.Id,
                            Value = vo.Value,
                            DisplayText = $"{v.Name}: {vo.Value}"
                        }).ToList()
                    })
                    .ToList();
                ViewData["VariationsWithOptions"] = variationsWithOptions;
                return Page();
            }

            // Check for duplicate Product Item with same variation options (by VariationName+Value)
            if (SelectedVariationOptionIds != null && SelectedVariationOptionIds.Any())
            {
                var selectedIds = SelectedVariationOptionIds.Values.ToList();
                var selectedOptions = await _context.VariationOptions
                    .Include(vo => vo.Variation)
                    .Where(vo => selectedIds.Contains(vo.Id))
                    .ToListAsync();

                var newCombo = string.Join("|", selectedOptions
                    .OrderBy(vo => vo.Variation.Name)
                    .ThenBy(vo => vo.Value)
                    .Select(vo => $"{vo.Variation.Name}:{vo.Value}"));

                // Get all existing product items for this product
                var existingProductItems = await _context.ProductItems
                    .Include(pi => pi.VariationOptions)
                        .ThenInclude(vo => vo.Variation)
                    .Where(pi => pi.ProductId == ProductItem.ProductId)
                    .ToListAsync();

                foreach (var existingItem in existingProductItems)
                {
                    var combo = string.Join("|", existingItem.VariationOptions
                        .OrderBy(vo => vo.Variation.Name)
                        .ThenBy(vo => vo.Value)
                        .Select(vo => $"{vo.Variation.Name}:{vo.Value}"));

                    if (combo == newCombo)
                    {
                        ModelState.AddModelError("", $"A product item with the same configuration ({newCombo}) already exists for this product.");
                        // Repopulate ViewData for validation errors
                        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                        var variationsWithOptions = _context.Variations
                            .Select(v => new
                            {
                                VariationName = v.Name,
                                Options = v.VariationOptions.Select(vo => new
                                {
                                    Id = vo.Id,
                                    Value = vo.Value,
                                    DisplayText = $"{v.Name}: {vo.Value}"
                                }).ToList()
                            })
                            .ToList();
                        ViewData["VariationsWithOptions"] = variationsWithOptions;
                        return Page();
                    }
                }
            }

            // Ensure all numeric fields are at least 0
            ProductItem.Quantity = ProductItem.Quantity ?? 0;
            ProductItem.ImportPrice = ProductItem.ImportPrice ?? 0;
            ProductItem.SellingPrice = ProductItem.SellingPrice ?? 0;
            ProductItem.Discount = ProductItem.Discount ?? 0;
            // Set status to 'active' (id=1)
            ProductItem.StatusId = 1;

            // Create the ProductItem first
            _context.ProductItems.Add(ProductItem);
            await _context.SaveChangesAsync();

            // Handle one option per variation type
            if (SelectedVariationOptionIds != null && SelectedVariationOptionIds.Any())
            {
                var selectedIds = SelectedVariationOptionIds.Values.ToList();
                var variationOptions = await _context.VariationOptions
                    .Where(vo => selectedIds.Contains(vo.Id))
                    .ToListAsync();

                foreach (var option in variationOptions)
                {
                    ProductItem.VariationOptions.Add(option);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
