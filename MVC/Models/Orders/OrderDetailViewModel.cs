using DAL.Models;
using System.Collections.Generic;

namespace MVC.Models.Orders
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string? StatusName { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
