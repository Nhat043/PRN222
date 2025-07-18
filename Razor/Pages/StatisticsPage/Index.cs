using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Service.Interface;

namespace Razor.Pages
{
    public class StatisticsPageModel : PageModel
    {
        public string RevenueChartJson { get; set; } = "{}";
        public string OrdersChartJson { get; set; } = "{}";
        public string BestProductsChartJson { get; set; } = "{}";
        public string AovChartJson { get; set; } = "{}";

        private readonly IStatisticsService _statisticsService;

        public StatisticsPageModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task OnGetAsync()
        {
            RevenueChartJson = await _statisticsService.GetRevenueChartJsonAsync();
            OrdersChartJson = await _statisticsService.GetOrdersChartJsonAsync();
            BestProductsChartJson = await _statisticsService.GetBestProductsChartJsonAsync();
            AovChartJson = await _statisticsService.GetAovChartJsonAsync();
        }
    }
} 