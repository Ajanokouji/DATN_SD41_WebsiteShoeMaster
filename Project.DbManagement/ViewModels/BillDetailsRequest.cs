namespace Project.DbManagement.ViewModels;

public class BillDetailsRequest
{
    public Guid Id { get; set; }    
    public Guid IdBill { get; set; }
    public Guid IdProduct { get; set; }
    public int Quantity { get; set; }
    //public int DonGia { get; set; } thanh toán r mới lưu
    public int Status { get; set; }
}