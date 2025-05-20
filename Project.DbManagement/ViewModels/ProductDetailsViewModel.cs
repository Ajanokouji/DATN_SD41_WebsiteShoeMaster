using Project.DbManagement.Entity;

namespace Project.DbManagement.ViewModels;

public class ProductDetailsViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> lstColor { get; set; }
    public List<string> lstSize { get; set; }
}