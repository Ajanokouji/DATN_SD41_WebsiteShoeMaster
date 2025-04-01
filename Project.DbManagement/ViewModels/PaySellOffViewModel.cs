namespace Project.DbManagement.ViewModels;

public class PaySellOffViewModel
{
    public Guid Id { get; set; }
    public string MaHD { get; set; }
    public string KhachHang { get; set; }
    public string NhanVien { get; set; }
    public DateTime? NgayThanhToan { get; set; }
    public int TongSL { get; set; }
    public int? TongTien { get; set; }
}