using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task AddOrderAsync(Order order);

    }
}
