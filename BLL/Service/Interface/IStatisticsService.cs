using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IStatisticsService
    {
        Task<string> GetRevenueChartJsonAsync();
        Task<string> GetOrdersChartJsonAsync();
        Task<string> GetBestProductsChartJsonAsync();
        Task<string> GetAovChartJsonAsync();
    }
} 