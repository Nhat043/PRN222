using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IStatisticsRepo
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsWithRelatedDataAsync();
    }
} 