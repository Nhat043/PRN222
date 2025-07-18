using BLL.Service.Interface;
using DAL.Models; // Cho Order, Account
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers;
using MVC.Models;

using MVC.Models.Orders;
using Org.BouncyCastle.Utilities;
using System.Linq;
using System.Threading.Tasks;

public class OrderController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IOrderService _orderService;
    private readonly IProductItemService _productItemService;

    public OrderController(IAccountService accountService, IOrderService orderService, IProductItemService productItemService)
    {
        _accountService = accountService;
        _orderService = orderService;
        _productItemService = productItemService;
    }

    // GET: /Order/Checkout
    [HttpGet]
    public IActionResult Checkout()
    {
        var accountId = HttpContext.Session.GetInt32("AccountIdSession");
        if (accountId == null)
            return RedirectToAction("Login", "Auth");

        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

        decimal total = cart.Sum(item => {
            IPriceCalculator calculator = new BasePriceCalculator();
            if (item.Discount.HasValue && item.Discount.Value > 0)
            {
                calculator = new DiscountDecorator(calculator, item.Discount.Value);
            }
            return calculator.CalculatePrice(item.SellingPrice ?? 0, item.Quantity);
        });

        var model = new CheckoutViewModel
        {
            CartItems = cart,
            TotalPrice = total
        };

        return View(model);
    }

    // POST: /Order/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountIdSession");
        if (accountId == null)
            return RedirectToAction("Login", "Auth");

        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        if (!cart.Any())
        {
            TempData["Error"] = "Giỏ hàng rỗng!";
            return RedirectToAction("Index", "Cart");
        }

        int total = (int)cart.Sum(item => {
            IPriceCalculator calculator = new BasePriceCalculator();
            if (item.Discount.HasValue && item.Discount.Value > 0)
            {
                calculator = new DiscountDecorator(calculator, item.Discount.Value);
            }
            return calculator.CalculatePrice(item.SellingPrice ?? 0, item.Quantity);
        });

        // 1. Tạo Order và lưu
        var order = new Order
        {
            UserId = accountId.Value,
            Date = DateTime.Now,
            Price = total,
            StatusId = 1
        };
        await _orderService.AddOrderAsync(order); // order.Id sẽ có giá trị sau khi lưu

        // 2. Tạo OrderItem và lưu
        var orderItems = cart.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ProductItemId = item.ProductItemId,
            Quantity = item.Quantity,
            Price = item.SellingPrice ?? 0
        }).ToList();
        await _orderService.AddOrderItemsAsync(orderItems);

        // 3. Trừ tồn kho ProductItem
        foreach (var item in cart)
        {
            var productItem = await _productItemService.GetProductItemByIdAsync(item.ProductItemId);
            if (productItem != null)
            {
                productItem.Quantity -= item.Quantity;
                if (productItem.Quantity < 0) productItem.Quantity = 0; // Không để số âm
                await _productItemService.UpdateProductItemAsync(productItem);
            }
        }

        // 4. Xoá giỏ hàng và thông báo
        HttpContext.Session.Remove("Cart");
        TempData["Message"] = "Đặt hàng thành công!";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        var model = new OrderDetailViewModel
        {
            Id = order.Id,
            Date = order.Date,
            Price = order.Price,
            StatusName = order.Status?.Name,
            OrderItems = order.OrderItems.ToList()
        };

        return View(model);
    }


}
