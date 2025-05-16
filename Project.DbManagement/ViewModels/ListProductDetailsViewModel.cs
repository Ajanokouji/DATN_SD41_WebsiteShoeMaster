using Project.DbManagement.Entity;

namespace Project.DbManagement.ViewModels;

public class ListProductDetailsViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Size { get; set; }
    public string Quantity { get; set; }
    public string Image { get; set; }
    public string Price { get; set; }
}