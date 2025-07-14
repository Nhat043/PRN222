namespace MVC.Models.Order
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
    }
}
