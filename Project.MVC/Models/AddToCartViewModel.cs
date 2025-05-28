namespace Project.MVC.Models
{
    public class AddToCartViewModel
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public decimal? Price { get; set; }
        public int ?Stock { get; set; }
        public int? Quantity { get; set; }
        public string? SKU { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? Total { get; set; }
    }

}
