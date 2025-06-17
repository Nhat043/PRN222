using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class RevenueLog
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int TotalRevenue { get; set; }

    public int TotalCost { get; set; }

    public int TotalProfit { get; set; }

    public DateTime Date { get; set; }

    public virtual Order? Order { get; set; }
}
