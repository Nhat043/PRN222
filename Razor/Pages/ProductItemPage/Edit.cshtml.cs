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
    public class EditModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public EditModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductItem ProductItem { get; set; } = default!;

        // Key: VariationName, Value: Selected Option Id
        [BindProperty]
        public Dictionary<string, int> SelectedVariationOptionIds { get; set; } = new Dictionary<string, int>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productitem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (productitem == null)
            {
                return NotFound();
            }
            
            ProductItem = productitem;
            
            // Set selected variation option IDs grouped by variation name
            var groupedOptions = ProductItem.VariationOptions
                .GroupBy(vo => vo.Variation?.Name ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.First().Id);
            
            SelectedVariationOptionIds = groupedOptions;
            
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.ProductItemStatuses, "Id", "Name");
            
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate ViewData for validation errors
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
                ViewData["StatusId"] = new SelectList(_context.ProductItemStatuses, "Id", "Name");
                
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

            // Get the existing ProductItem with its VariationOptions
            var existingProductItem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                .FirstOrDefaultAsync(pi => pi.Id == ProductItem.Id);

            if (existingProductItem == null)
            {
                return NotFound();
            }

            // Update basic properties
            existingProductItem.ProductId = ProductItem.ProductId;
            existingProductItem.Quantity = ProductItem.Quantity;
            existingProductItem.ImportPrice = ProductItem.ImportPrice;
            existingProductItem.SellingPrice = ProductItem.SellingPrice;
            existingProductItem.Discount = ProductItem.Discount;
            existingProductItem.StatusId = ProductItem.StatusId;

            // Update VariationOptions - clear existing and add the selected ones
            existingProductItem.VariationOptions.Clear();
            
            if (SelectedVariationOptionIds != null && SelectedVariationOptionIds.Any())
            {
                var selectedIds = SelectedVariationOptionIds.Values.ToList();
                var variationOptions = await _context.VariationOptions
                    .Where(vo => selectedIds.Contains(vo.Id))
                    .ToListAsync();

                foreach (var option in variationOptions)
                {
                    existingProductItem.VariationOptions.Add(option);
                }
            }

            _context.Attach(existingProductItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductItemExists(ProductItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductItemExists(int id)
        {
            return _context.ProductItems.Any(e => e.Id == id);
        }
    }
}
