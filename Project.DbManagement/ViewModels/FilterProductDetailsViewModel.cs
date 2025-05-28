namespace Project.DbManagement.ViewModels;

public class FilterProductDetailsViewModel
{
    public Guid ProductId { get; set; }
    public List<string>? lstColor { get; set; }
    public List<string>? lstSize { get; set; }
}