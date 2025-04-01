using Project.DbManagement.Entity;

namespace Project.DbManagement.ViewModels;

public class ProductDetailsViewModel
{
    public Guid Id { get; set; }
    public string Ten { get; set; }
    public List<ProductEntity> lstMau { get; set; }
    public List<ProductEntity> lstKC { get; set; }
}