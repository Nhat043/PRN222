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

        [BindProperty]
        public ProductItem ProductItem { get; set; } = default!;

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
