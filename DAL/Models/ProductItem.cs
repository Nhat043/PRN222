using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ProductItem
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public int? ImportPrice { get; set; }

    public int? SellingPrice { get; set; }

    public decimal? Discount { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product? Product { get; set; }

    public virtual ProductItemStatus? Status { get; set; }

    public virtual ICollection<VariationOption> VariationOptions { get; set; } = new List<VariationOption>();
}
