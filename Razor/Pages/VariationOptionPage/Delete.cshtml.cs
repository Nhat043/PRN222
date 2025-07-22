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

namespace Razor.Pages.VariationOptionPage
{
    public class DeleteModel : PageModel
    {
        private readonly IVariationOptionService _variationOptionService;

        public DeleteModel(IVariationOptionService variationOptionService)
        {
            _variationOptionService = variationOptionService;
        }

        [BindProperty]
        public VariationOption VariationOption { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variationoption = await _variationOptionService.GetVariationOptionByIdAsync(id.Value);

            if (variationoption == null)
            {
                return NotFound();
            }
            else
            {
                VariationOption = variationoption;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variationoption = await _variationOptionService.GetVariationOptionByIdAsync(id.Value);
            if (variationoption != null)
            {
                VariationOption = variationoption;
                await _variationOptionService.DeleteVariationOptionAsync(VariationOption.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}
