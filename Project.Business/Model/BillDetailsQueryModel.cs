using Project.Common;
using SERP.Metadata.Models.Query;

namespace Project.Business.Model;

public class BillDetailsQueryModel : BaseRequestModel, IListMetadataFilterQuery
{
    public Guid? id_hoa_don_chi_tiet { get; set; }
    public string? ma_hoa_don_chi_tiet { get; set; }
    public Guid? BillId { get; set; }
    public List<Guid>? BillIds { get; set; }
    public Guid? id_san_pham_chi_tiet { get; set; }
    public int? trang_thai { get; set; }
    public int? so_luong { get; set; }
    public decimal? don_gia { get; set; }
    public string? create_by { get; set; }
    public DateTime? create_on_date { get; set; }
    public DateTime? last_modifi_on_date { get; set; }
    public string? update_by { get; set; }
    public string? ghi_chu { get; set; }
    public DateTime? StartDate {  get; set; }
    public DateTime? EndDate { get; set; }
}
