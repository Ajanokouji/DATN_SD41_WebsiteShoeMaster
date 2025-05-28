namespace Project.DbManagement.ViewModels;

public class BillDetailsViewModel
{
    public Guid Id { get; set; }
    public Guid? IdBill { get; set; }
    public Guid? IdProduct { get; set; }
    public string? Image { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Sku { get; set; }
    public string? Size { get; set; }
    public int Quantity { get; set; }
    public decimal? Price { get; set; }
}