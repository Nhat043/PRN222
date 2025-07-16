using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Datas;
using DAL.Models;
using BLL.Service.Interface;

namespace Razor.Pages.VariationOptionPage
{
    public class CreateModel : PageModel
    {
        private readonly IVariationOptionService _variationOptionService;
        private readonly DAL.Datas.DemoContext _context;

        public CreateModel(IVariationOptionService variationOptionService, DAL.Datas.DemoContext context)
        {
            _variationOptionService = variationOptionService;
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public VariationOption VariationOption { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
                return Page();
            }

            try
            {
                await _variationOptionService.AddVariationOptionAsync(VariationOption);
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                // Handle duplicate variation option value error
                ModelState.AddModelError("VariationOption.Value", ex.Message);
                ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
                return Page();
            }
            catch (Exception ex)
            {
                // Handle other errors
                ModelState.AddModelError("", "An error occurred while creating the variation option. Please try again.");
                ViewData["VariationId"] = new SelectList(_context.Variations, "Id", "Name");
                return Page();
            }
        }
    }
}
