namespace Project.DbManagement.ViewModels;

public class BillDetailsViewModel
{
    public Guid Id { get; set; }
    public Guid IdHoaDon { get; set; }
    public Guid? IdSP { get; set; }
    public string? Ten { get; set; }
    public string? PhanLoai { get; set; }
    public int SoLuong { get; set; }
    public int GiaKM { get; set; } // Cho hóa đơn chưa thanh toán
    public int? GiaGoc { get; set; }// Cho hóa đơn chưa thanh toán
    public int? GiaLuu { get; set; } // Hóa đơn thanh toán rồi
}