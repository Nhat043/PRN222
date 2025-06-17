using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime Date { get; set; }

    public int Price { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<RevenueLog> RevenueLogs { get; set; } = new List<RevenueLog>();

    public virtual OrderStatus? Status { get; set; }

    public virtual Account? User { get; set; }
}
