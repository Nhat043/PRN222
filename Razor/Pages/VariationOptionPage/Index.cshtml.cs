using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using BLL.Service.Interface;

namespace Razor.Pages.VariationOptionPage
{
    public class IndexModel : PageModel
    {
        private readonly IVariationOptionService _variationOptionService;

        public IndexModel(IVariationOptionService variationOptionService)
        {
            _variationOptionService = variationOptionService;
        }

        public IList<VariationOption> RamOptions { get; set; } = new List<VariationOption>();
        public IList<VariationOption> StorageOptions { get; set; } = new List<VariationOption>();

        public async Task OnGetAsync()
        {
            var allOptions = (await _variationOptionService.GetAllVariationOptionsAsync()).ToList();
            RamOptions = allOptions
                .Where(vo => vo.Variation != null && vo.Variation.Name.ToLower() == "ram")
                .OrderBy(vo => ExtractNumber(vo.Value))
                .ToList();
            StorageOptions = allOptions
                .Where(vo => vo.Variation != null && vo.Variation.Name.ToLower() == "storage")
                .OrderBy(vo => ExtractNumber(vo.Value))
                .ToList();
        }

        private int ExtractNumber(string value)
        {
            // Extract the first number found in the string (e.g., "4GB" -> 4)
            var match = Regex.Match(value, "\\d+");
            return match.Success ? int.Parse(match.Value) : int.MaxValue;
        }
    }
}
