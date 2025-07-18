namespace BLL.Service.Interface
{
    public interface IPriceCalculator
    {
        decimal CalculatePrice(int sellingPrice, int quantity, decimal discount = 0);
    }

    public class BasePriceCalculator : IPriceCalculator
    {
        public decimal CalculatePrice(int sellingPrice, int quantity, decimal discount = 0)
        {
            return sellingPrice * quantity;
        }
    }

    public class DiscountDecorator : IPriceCalculator
    {
        private readonly IPriceCalculator _calculator;
        private readonly decimal _discountPercentage;

        public DiscountDecorator(IPriceCalculator calculator, decimal discountPercentage)
        {
            _calculator = calculator;
            _discountPercentage = discountPercentage;
        }

        public decimal CalculatePrice(int sellingPrice, int quantity, decimal discount = 0)
        {
            var basePrice = _calculator.CalculatePrice(sellingPrice, quantity, discount);
            return basePrice * (1 - _discountPercentage / 100);
        }
    }
} 