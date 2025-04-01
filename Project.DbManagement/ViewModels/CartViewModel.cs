namespace Project.DbManagement.ViewModels;

public class CartViewModel
{
    public Guid Id { get; set; }
    public string MaHD { get; set; }
    public Guid? IdKhachHang { get; set; }
    public string? TenKhachHang { get; set; }
    public string? GhiChu { get; set; } 
    public List<BillDetailsViewModel> lstBDs { get; set; }
}