using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Service.Interface;

namespace Razor.Pages.StatisticsPage
{
    public class IndexModel : PageModel
    {
        public string RevenueChartJson { get; set; } = "{}";
        public string OrdersChartJson { get; set; } = "{}";
        public string BestProductsChartJson { get; set; } = "{}";
        public string AovChartJson { get; set; } = "{}";

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        private readonly IStatisticsService _statisticsService;

        public IndexModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task OnGetAsync()
        {
            // Set default date range if not provided (last 12 months)
            if (!StartDate.HasValue)
                StartDate = DateTime.Now.AddMonths(-12);
            if (!EndDate.HasValue)
                EndDate = DateTime.Now;

            RevenueChartJson = await _statisticsService.GetRevenueChartJsonAsync(StartDate.Value, EndDate.Value);
            OrdersChartJson = await _statisticsService.GetOrdersChartJsonAsync(StartDate.Value, EndDate.Value);
            BestProductsChartJson = await _statisticsService.GetBestProductsChartJsonAsync(StartDate.Value, EndDate.Value);
            AovChartJson = await _statisticsService.GetAovChartJsonAsync(StartDate.Value, EndDate.Value);
        }
    }
} 