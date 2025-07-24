using BLL.Service.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Product;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IRatingService _ratingService;
        private readonly IComService _comService;
        public ProductController(IProductService productService, IRatingService ratingService, IComService comService)
        {
            _productService = productService;
            _ratingService = ratingService;
            _comService = comService;
        }

        // /Product/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductByIdWithAvailableItemsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("AccountIdSession");

            var commentVM = new CommentViewModel
            {
                Comments = _comService.GetProductComments(id),
                CurrentUserId = userId
            };

            ViewBag.AvgRating = _ratingService.GetAverageRating(id);
            ViewBag.ReviewCount = _ratingService.GetReviewCount(id);
            ViewBag.UserRating = userId.HasValue ? _ratingService.GetUserRating(userId.Value, id) : 0;
            ViewBag.CommentVM = commentVM;

            return View(product);
        }


        [HttpPost]
        public IActionResult Rate(int productId, int ratingValue)
        {
            // Try to get user ID from session
            int? userId = HttpContext.Session.GetInt32("AccountIdSession");

            if (userId == null)
            {
                TempData["Error"] = "You must be logged in to rate a product.";
                return RedirectToAction("Detail", new { id = productId });
            }

            _ratingService.RateProduct(userId.Value, productId, ratingValue);
            return RedirectToAction("Detail", new { id = productId });
        }

        [HttpPost]
        public IActionResult PostComment(int productId, string content, int? parentId)
        {
            int? userId = HttpContext.Session.GetInt32("AccountIdSession");
            if (userId == null)
            {
                TempData["Error"] = "Login required to comment.";
                return RedirectToAction("Detail", new { id = productId });
            }

            var comment = new Comment
            {
                ProductId = productId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now,
                ParentId = parentId
            };

            _comService.CreateComment(comment);
            return RedirectToAction("Detail", new { id = productId });
        }


        [HttpPost]
        public IActionResult HideComment(int commentId)
        {
            _comService.HideComment(commentId);
            // Optionally redirect to where the comment was
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public IActionResult EditComment(int commentId, string newContent)
        {
            var userId = HttpContext.Session.GetInt32("AccountIdSession");
            var roleId = HttpContext.Session.GetString("RoleIdSession");

            var comment = _comService.GetById(commentId);
            if (comment == null || (comment.UserId != userId && roleId != "1")) // 1 = admin
            {
                return Forbid(); // prevent unauthorized edit
            }

            comment.Content = newContent;
            _comService.UpdateComment(comment);

            return RedirectToAction("Detail", new { id = comment.ProductId });
        }
        [HttpPost]
        public IActionResult EditTrigger(int commentId)
        {
            TempData["EditCommentId"] = commentId;
            var comment = _comService.GetById(commentId);
            return RedirectToAction("Detail", new { id = comment?.ProductId ?? 0 });
        }

        public async Task<IActionResult> Index(string search, string ram, string rom, string price, int? category)
        {
            var ramOptions = await _productService.GetAllRamOptionsAsync();
            var romOptions = await _productService.GetAllRomOptionsAsync();
            var categories = await _productService.GetAllCategoriesAsync();

            // Nếu search, filter theo search + 4 trường còn lại (có thể là all hoặc chọn)
            var products = await _productService.GetFilteredProductsAsync(search, ram, rom, price, category);

            var productVms = products.Select(p =>
            {
                ProductItem item = null;
                if (!string.IsNullOrEmpty(ram) && !string.IsNullOrEmpty(rom))
                    item = p.ProductItems.FirstOrDefault(i =>
                        i.VariationOptions.Any(v => v.Variation.Name == "RAM" && v.Value == ram) &&
                        i.VariationOptions.Any(v => v.Variation.Name == "STORAGE" && v.Value == rom));
                else if (!string.IsNullOrEmpty(ram))
                    item = p.ProductItems.FirstOrDefault(i =>
                        i.VariationOptions.Any(v => v.Variation.Name == "RAM" && v.Value == ram));
                else if (!string.IsNullOrEmpty(rom))
                    item = p.ProductItems.FirstOrDefault(i =>
                        i.VariationOptions.Any(v => v.Variation.Name == "STORAGE" && v.Value == rom));
                if (item == null)
                    item = p.ProductItems.FirstOrDefault();

                var ramOpt = item?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "RAM")?.Value;
                var romOpt = item?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "STORAGE")?.Value;
                var ratings = p.Ratings?.ToList() ?? new List<DAL.Models.Rating>();
                double? avgRating = ratings.Any() ? (double?)ratings.Average(r => r.RatingValue) : null;
                int? reviewCount = ratings.Any() ? ratings.Count : null;

                return new ProductListVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Picture = p.Picture,
                    CategoryName = p.Category?.Name,
                    AvgRating = avgRating,
                    ReviewCount = reviewCount,
                    Price = item?.SellingPrice,
                    Ram = ramOpt,
                    Rom = romOpt,
                    ProductItemId = item?.Id
                };
            }).ToList();

            var model = new ProductIndexViewModel
            {
                Products = productVms,
                Categories = categories.Select(c => new CategoryVm { Id = c.Id, Name = c.Name }).ToList(),
                RamOptions = ramOptions,
                RomOptions = romOptions,
                SelectedRam = ram,
                SelectedRom = rom,
                SelectedPrice = price,
                SelectedCategory = category,
                SearchText = search
            };

            return View(model);
        }

    }
}
