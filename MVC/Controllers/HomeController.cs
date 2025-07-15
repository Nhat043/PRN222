using BLL.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.Product;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            var newestProduct = await _productService.GetNewestProductAsync();
            LaptopFeatureVm? newestLaptopVm = null;
            if (newestProduct != null)
            {
                var productItem = newestProduct.ProductItems.FirstOrDefault();
                var ram = productItem?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "RAM")?.Value;
                var rom = productItem?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "STORAGE")?.Value;

                newestLaptopVm = new LaptopFeatureVm
                {
                    Id = newestProduct.Id,
                    Name = newestProduct.Name,
                    Picture = newestProduct.Picture,
                    Price = productItem?.SellingPrice,
                    Ram = ram,
                    Rom = rom
                };
            }

            var featuredProducts = await _productService.GetFeaturedProductsAsync(4);
            var featuredLaptops = featuredProducts.Select(p =>
            {
                var productItem = p.ProductItems.FirstOrDefault();
                var ram = productItem?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "RAM")?.Value;
                var rom = productItem?.VariationOptions.FirstOrDefault(x => x.Variation.Name == "STORAGE")?.Value;

                return new LaptopFeatureVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    Picture = p.Picture,
                    Price = productItem?.SellingPrice,
                    Ram = ram,
                    Rom = rom,
                    ProductItemId = productItem?.Id
                };
            }).ToList();

            var model = new HomeIndexViewModel
            {
                NewestLaptop = newestLaptopVm,
                FeaturedLaptops = featuredLaptops
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
