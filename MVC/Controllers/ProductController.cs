using BLL.Service.Interface;
using DAL.Models;
using MVC.Models.Product;
using MVC.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var products = await _productService.SearchProductsByNameAsync(searchString);
            return View(products);
        }


        // /Product/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) {
                return NotFound();
            } 

            var userId = HttpContext.Session.GetInt32("AccountIdSession");

            // Comments section only
            var commentVM = new CommentViewModel
            {
                Comments = _comService.GetProductComments(id),
                CurrentUserId = userId
            };

            ViewBag.AvgRating = _ratingService.GetAverageRating(id); // For rating
            
            ViewBag.CommentVM = commentVM;

            return View(product); // Still using Product as main model
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
        public IActionResult PostComment(int productId, string content)
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
                CreatedAt = DateTime.Now
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

    }
}
