using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Models;
using BLL.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;

namespace Razor.Pages.VariationOptionPage
{
    public class CreateModel : PageModel
    {
        private readonly IVariationOptionService _variationOptionService;
        private readonly IVariationService _variationService;
        private readonly IHubContext<VarianSignalR> _hubContext;

        public CreateModel(IVariationOptionService variationOptionService, IVariationService variationService, IHubContext<VarianSignalR> hubContext)
        {
            _variationOptionService = variationOptionService;
            _variationService = variationService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> OnGet()
        {
            var variations = await _variationService.GetAllVariationsWithOptionsAsync();
            ViewData["VariationId"] = new SelectList(variations, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public VariationOption VariationOption { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var variations = await _variationService.GetAllVariationsWithOptionsAsync();
                ViewData["VariationId"] = new SelectList(variations, "Id", "Name");
                return Page();
            }

            try
            {
                await _variationOptionService.AddVariationOptionAsync(VariationOption);
                await _hubContext.Clients.All.SendAsync("load");
                return RedirectToPage("./Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("VariationOption.Value", ex.Message);
                var variations = await _variationService.GetAllVariationsWithOptionsAsync();
                ViewData["VariationId"] = new SelectList(variations, "Id", "Name");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the variation option. Please try again.");
                var variations = await _variationService.GetAllVariationsWithOptionsAsync();
                ViewData["VariationId"] = new SelectList(variations, "Id", "Name");
                return Page();
            }
        }
    }
}
