using DAL.Models;

namespace MVC.Models;

public class CartItemViewModel
{
    public int ProductItemId { get; set; }
    public string ProductName { get; set; }
    public string? Picture { get; set; }
    public int Quantity { get; set; }
    public int? SellingPrice { get; set; }
    public decimal? Discount { get; set; }
    public int TotalPrice => Math.Max(0, ((SellingPrice ?? 0) - (int)(Discount ?? 0)) * Quantity);
}