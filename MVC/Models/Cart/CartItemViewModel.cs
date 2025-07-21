using BLL.Service.Interface;
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
    public decimal TotalPrice {
        get {
            IPriceCalculator calculator = new BasePriceCalculator();
            if (Discount.HasValue && Discount.Value > 0)
            {
                calculator = new DiscountDecorator(calculator, Discount.Value);
            }
            return calculator.CalculatePrice(SellingPrice ?? 0, Quantity);
        }
    }
}