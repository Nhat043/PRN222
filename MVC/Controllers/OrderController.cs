using BLL.Service.Interface;
using DAL.Models; // Cho Order, Account
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers;
using MVC.Models;

using MVC.Models.Orders;
using Org.BouncyCastle.Utilities;
using System.Linq;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using Microsoft.Extensions.Configuration;
using PayPalOrder = PayPalCheckoutSdk.Orders.Order;

public class OrderController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IOrderService _orderService;
    private readonly IProductItemService _productItemService;
    private readonly IConfiguration _configuration;

    public OrderController(IAccountService accountService, IOrderService orderService, IProductItemService productItemService, IConfiguration configuration)
    {
        _accountService = accountService;
        _orderService = orderService;
        _productItemService = productItemService;
        _configuration = configuration;
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
        var order = new DAL.Models.Order
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



        if (await _orderService.CheckQuantity(orderItems) == false)
        {
            TempData["Error"] = "Some products in the cart are out of stock or insufficient in quantity. Please check again";
            return RedirectToAction("Checkout", "Order");
        }
        await _orderService.AddOrderItemsAsync(orderItems);

        // 3. Trừ tồn kho ProductItem
        var notifiedProductIds = new HashSet<int>();
        foreach (var item in cart)
        {
            var productItem = await _productItemService.GetProductItemByIdAsync(item.ProductItemId);
            if (productItem != null)
            {
                productItem.Quantity -= item.Quantity;
                if (productItem.Quantity < 0) productItem.Quantity = 0; // Không để số âm
                await _productItemService.UpdateProductItemAsync(productItem);
                if (productItem.ProductId.HasValue && !notifiedProductIds.Contains(productItem.ProductId.Value))
                {
                    await _orderService.NotifyProductQuantityChanged(productItem.ProductId.Value);
                    notifiedProductIds.Add(productItem.ProductId.Value);
                }
            }
        }

        // 4. Xoá giỏ hàng và thông báo
        HttpContext.Session.Remove("Cart");
        TempData["Message"] = "Order successfully!";
        await _orderService.NotifyAdminNewOrder();
        return RedirectToAction("Index", "Home");
    }

    // Helper to get PayPal environment
    private PayPalEnvironment GetPayPalEnvironment()
    {
        var clientId = _configuration["PayPal:ClientId"];
        var secret = _configuration["PayPal:Secret"];
        return new SandboxEnvironment(clientId, secret);
    }

    private PayPalHttpClient GetPayPalClient()
    {
        return new PayPalHttpClient(GetPayPalEnvironment());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePaypalOrder()
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

        // Create PayPal order
        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD", // Change to your currency if needed
                        Value = (total / 24000.0m).ToString("F2") // Example: convert VND to USD (1 USD = 24,000 VND)
                    }
                }
            },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = Url.Action("PaypalSuccess", "Order", null, Request.Scheme),
                CancelUrl = Url.Action("PaypalCancel", "Order", null, Request.Scheme)
            }
        });

        var response = await GetPayPalClient().Execute(request);
        var statusCode = response.StatusCode;
        var result = response.Result<PayPalOrder>();
        var approvalLink = result.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;
        if (approvalLink != null)
        {
            // Store cart in session for later use
            HttpContext.Session.SetObject("PendingPaypalCart", cart);
            HttpContext.Session.SetInt32("PendingPaypalTotal", total);
            HttpContext.Session.SetInt32("PendingPaypalAccountId", accountId.Value);
            return Redirect(approvalLink);
        }
        TempData["Error"] = "Không thể tạo thanh toán PayPal.";
        return RedirectToAction("Checkout");
    }

    public async Task<IActionResult> PaypalSuccess(string token)
    {
        // Capture the PayPal order
        var request = new OrdersCaptureRequest(token);
        request.RequestBody(new OrderActionRequest());
        var response = await GetPayPalClient().Execute(request);
        var result = response.Result<PayPalOrder>();
        if (result.Status == "COMPLETED")
        {
            // Retrieve cart and account info from session
            var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("PendingPaypalCart") ?? new List<CartItemViewModel>();
            var total = HttpContext.Session.GetInt32("PendingPaypalTotal") ?? 0;
            var accountId = HttpContext.Session.GetInt32("PendingPaypalAccountId");
            if (accountId == null || !cart.Any())
            {
                TempData["Error"] = "Session hết hạn hoặc không hợp lệ.";
                return RedirectToAction("Checkout");
            }

            // 1. Tạo Order và lưu
            var order = new DAL.Models.Order
            {
                UserId = accountId.Value,
                Date = DateTime.Now,
                Price = total,
                StatusId = 2 // Mark as approved/paid
            };
            await _orderService.AddOrderAsync(order);

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
                    if (productItem.Quantity < 0) productItem.Quantity = 0;
                    await _productItemService.UpdateProductItemAsync(productItem);
                }
            }

            // 4. Xoá giỏ hàng và thông báo
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("PendingPaypalCart");
            HttpContext.Session.Remove("PendingPaypalTotal");
            HttpContext.Session.Remove("PendingPaypalAccountId");
            TempData["Message"] = "Thanh toán PayPal thành công! Đơn hàng đã được ghi nhận.";
            return RedirectToAction("Index", "Home");
        }
        TempData["Error"] = "Thanh toán PayPal thất bại.";
        return RedirectToAction("Checkout");
    }

    public IActionResult PaypalCancel()
    {
        TempData["Error"] = "Bạn đã huỷ thanh toán PayPal.";
        return RedirectToAction("Checkout");
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
