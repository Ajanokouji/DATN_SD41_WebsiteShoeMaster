namespace Project.DbManagement.ViewModels;

public class PaymentBillRequest
{
    public Guid Id { get; set; }
    public Guid IdEmployee { get; set; }
    public Guid IdCustomer { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public int TotalPrice { get; set; }
    public string status { get; set; }
}