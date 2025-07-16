using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Datas;
using DAL.Models;

namespace Razor.Pages.ProductItemPage
{
    public class CleanupModel : PageModel
    {
        private readonly DAL.Datas.DemoContext _context;

        public CleanupModel(DAL.Datas.DemoContext context)
        {
            _context = context;
        }

        public List<List<ProductItem>> DuplicateGroups { get; set; } = new List<List<ProductItem>>();

        [BindProperty]
        public List<int> KeepItemIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Load all product items with their variation options
            var allProductItems = await _context.ProductItems
                .Include(pi => pi.Product)
                .Include(pi => pi.Status)
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .ToListAsync();

            // Group by product and variation options
            var groupedByProduct = allProductItems
                .GroupBy(pi => pi.ProductId)
                .ToList();

            foreach (var productGroup in groupedByProduct)
            {
                // Group by variation options within each product
                var groupedByVariations = productGroup
                    .GroupBy(pi => string.Join("|", 
                        pi.VariationOptions
                            .OrderBy(vo => vo.Variation?.Name)
                            .ThenBy(vo => vo.Value)
                            .Select(vo => $"{vo.Variation?.Name}:{vo.Value}")))
                    .Where(g => g.Count() > 1) // Only groups with duplicates
                    .ToList();

                foreach (var variationGroup in groupedByVariations)
                {
                    DuplicateGroups.Add(variationGroup.ToList());
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (KeepItemIds == null || !KeepItemIds.Any())
            {
                return RedirectToPage("./Index");
            }

            // Load all product items with their variation options
            var allProductItems = await _context.ProductItems
                .Include(pi => pi.Product)
                .Include(pi => pi.Status)
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .ToListAsync();

            // Group by product and variation options
            var groupedByProduct = allProductItems
                .GroupBy(pi => pi.ProductId)
                .ToList();

            var itemsToRemove = new List<ProductItem>();

            foreach (var productGroup in groupedByProduct)
            {
                // Group by variation options within each product
                var groupedByVariations = productGroup
                    .GroupBy(pi => string.Join("|", 
                        pi.VariationOptions
                            .OrderBy(vo => vo.Variation?.Name)
                            .ThenBy(vo => vo.Value)
                            .Select(vo => $"{vo.Variation?.Name}:{vo.Value}")))
                    .Where(g => g.Count() > 1) // Only groups with duplicates
                    .ToList();

                foreach (var variationGroup in groupedByVariations)
                {
                    var groupItems = variationGroup.ToList();
                    var keepId = KeepItemIds.FirstOrDefault(id => groupItems.Any(item => item.Id == id));
                    
                    if (keepId > 0)
                    {
                        // Add all items except the one to keep
                        itemsToRemove.AddRange(groupItems.Where(item => item.Id != keepId));
                    }
                }
            }

            // Remove the duplicate items
            if (itemsToRemove.Any())
            {
                _context.ProductItems.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
} 