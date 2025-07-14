using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();

        Task AddOrderAsync(Order order);

    }
}
