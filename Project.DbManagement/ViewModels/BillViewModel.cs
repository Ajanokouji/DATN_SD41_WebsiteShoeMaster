namespace Project.DbManagement.ViewModels;

public class BillViewModel
{
    public Guid? Id { get; set; }
    public string? BillCode { get; set; }
    public Guid? IdClient { get; set; }
    public string? NameClient { get; set; }
    public string? Note { get; set; } 
    public List<BillDetailsViewModel> listBillDetails { get; set; }
}