namespace MVC.Models.Product
{
    public class LaptopFeatureVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Picture { get; set; }
        public int? Price { get; set; }
        public string? Ram { get; set; }
        public string? Rom { get; set; }
        public int? ProductItemId { get; set; }
    }

    public class HomeIndexViewModel
    {
        public LaptopFeatureVm NewestLaptop { get; set; }
        public List<LaptopFeatureVm> FeaturedLaptops { get; set; }
    }
}
