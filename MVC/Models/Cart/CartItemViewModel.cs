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
    public string? Ram { get; set; }   
    public string? Rom { get; set; }
    public decimal TotalPrice =>((SellingPrice ?? 0) - (SellingPrice ?? 0) * (Discount ?? 0) / 100) * Quantity;
}