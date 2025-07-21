using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IStatisticsService
    {
        Task<string> GetRevenueChartJsonAsync(DateTime startDate, DateTime endDate);
        Task<string> GetOrdersChartJsonAsync(DateTime startDate, DateTime endDate);
        Task<string> GetBestProductsChartJsonAsync(DateTime startDate, DateTime endDate);
        Task<string> GetAovChartJsonAsync(DateTime startDate, DateTime endDate);
    }
} 