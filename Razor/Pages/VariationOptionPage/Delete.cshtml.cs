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

using Microsoft.AspNetCore.SignalR;
using Razor.Hubs;


namespace Razor.Pages.VariationOptionPage
{
    public class DeleteModel : PageModel
    {
        private readonly IVariationOptionService _variationOptionService;

        private readonly IHubContext<DataSignalR> _hubContext;
        public DeleteModel(IVariationOptionService variationOptionService, IHubContext<DataSignalR> hubContext)
        {
            _variationOptionService = variationOptionService;
            _hubContext = hubContext; }


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

                await _hubContext.Clients.All.SendAsync("load");

            }

            return RedirectToPage("./Index");
        }
    }
}