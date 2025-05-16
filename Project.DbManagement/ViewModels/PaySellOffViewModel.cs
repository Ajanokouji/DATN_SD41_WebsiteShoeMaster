namespace Project.DbManagement.ViewModels;

public class PaySellOffViewModel
{
    public Guid? Id { get; set; }
    public string BillCode { get; set; }
    public string Client { get; set; }
    public string Employee { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int TotalQuantity { get; set; }
    public decimal? TotalPrice { get; set; }
    public List<BillDetailsViewModel> BillDetails { get; set; }
}