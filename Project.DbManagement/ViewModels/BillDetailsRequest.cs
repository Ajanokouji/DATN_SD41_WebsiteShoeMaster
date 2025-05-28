namespace Project.DbManagement.ViewModels;

public class BillDetailsRequest
{
    public Guid Id { get; set; }    
    public Guid IdBill { get; set; }
    public Guid IdProduct { get; set; }
    public Guid IdEmployee { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int Quantity { get; set; }
    //public int DonGia { get; set; } thanh toán r mới lưu
    public int Status { get; set; }
}