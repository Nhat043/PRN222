using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers;
using MVC.Models;

namespace MVC.Controllers;

public class CartController : Controller
{
    private readonly IProductItemService _productItemService;

    public CartController(IProductItemService productItemService)
    {
        _productItemService = productItemService;
    }

    // GET: /Cart
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        return View(cart);
    }

    // POST: /Cart/AddToCart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productItemId)
    {
        var productItem = await _productItemService.GetProductItemByIdAsync(productItemId);
        if (productItem == null) return NotFound();

        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

        var existingItem = cart.FirstOrDefault(x => x.ProductItemId == productItemId);
        int cartQuantity = existingItem?.Quantity ?? 0;

        // ❌ Nếu tổng số lượng vượt quá hàng tồn kho
        if (cartQuantity + 1 > productItem.Quantity)
        {
            TempData["Error"] = $" Sản phẩm trong kho không còn. Không thể thêm nữa!";
            return RedirectToAction("Detail", "Product", new { id = productItem.ProductId });
        }

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                ProductItemId = productItemId,
                ProductName = productItem.Product?.Name ?? "",
                Picture = productItem.Product?.Picture,
                SellingPrice = productItem.SellingPrice,
                Discount = productItem.Discount,
                Quantity = 1
            });
        }

        HttpContext.Session.SetObject("Cart", cart);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int productItemId, int change)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        var item = cart.FirstOrDefault(c => c.ProductItemId == productItemId);
        if (item == null) return RedirectToAction("Index");

        // Lấy số lượng tồn kho
        var productItem = await _productItemService.GetProductItemByIdAsync(productItemId);
        if (productItem == null) return RedirectToAction("Index");

        var newQuantity = item.Quantity + change;

        if (newQuantity > productItem.Quantity)
        {
            TempData["Error"] = $"Vượt quá số lượng sản phẩm trong kho.";
        }
        else if (newQuantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = newQuantity;
        }

        HttpContext.Session.SetObject("Cart", cart);
        return RedirectToAction("Index");
    }


    // GET: /Cart/Remove/5
    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        var item = cart.FirstOrDefault(c => c.ProductItemId == id);
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObject("Cart", cart);
        }
        return RedirectToAction("Index");
    }

    // GET: /Cart/Clear
    public IActionResult Clear()
    {
        HttpContext.Session.Remove("Cart");
        return RedirectToAction("Index");
    }
}
