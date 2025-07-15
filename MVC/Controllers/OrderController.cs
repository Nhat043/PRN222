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

    public OrderController(IAccountService accountService, IOrderService orderService)
    {
        _accountService = accountService;
        _orderService = orderService;
    }

    // GET: /Order/Checkout
    [HttpGet]
    public IActionResult Checkout()
    {
        var accountId = HttpContext.Session.GetInt32("AccountIdSession");
        if (accountId == null)
            return RedirectToAction("Login", "Auth");

        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

        // Ép kiểu SellingPrice & Discount chuẩn decimal (tránh lỗi nullable hoặc kiểu int)
        decimal total = cart.Sum(item =>
            (Convert.ToDecimal(item.SellingPrice) * item.Quantity) -
            ((item.Discount ?? 0) * item.Quantity)
        );

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

        int total = cart.Sum(item =>
            item.SellingPrice * item.Quantity - ((int)(item.Discount ?? 0) * item.Quantity)
        ) ?? 0;

        var order = new Order
        {
            UserId = accountId.Value,
            Date = DateTime.Now,
            Price = total,
            StatusId = 1
        };

        await _orderService.AddOrderAsync(order); 

        var orderItems = cart.Select(item => new OrderItem
        {
            OrderId = order.Id, 
            ProductItemId = item.ProductItemId,
            Quantity = item.Quantity,
            Price = item.SellingPrice ?? 0
        }).ToList();

        await _orderService.AddOrderItemsAsync(orderItems);

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
