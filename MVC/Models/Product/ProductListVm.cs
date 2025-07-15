namespace MVC.Models.Product
{
    public class ProductListVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string CategoryName { get; set; }
        public double? AvgRating { get; set; }
        public int? ReviewCount { get; set; }
        public int? Price { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public int? ProductItemId { get; set; }
    }

    public class CategoryVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductIndexViewModel
    {
        public List<ProductListVm> Products { get; set; }
        public List<CategoryVm> Categories { get; set; }
        public List<string> RamOptions { get; set; }
        public List<string> RomOptions { get; set; }
        public string SelectedRam { get; set; }
        public string SelectedRom { get; set; }
        public string SelectedPrice { get; set; }
        public int? SelectedCategory { get; set; }
    }

}
