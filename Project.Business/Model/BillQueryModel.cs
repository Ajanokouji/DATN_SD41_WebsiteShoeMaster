using Project.Common;
using Project.DbManagement.Entity;
using SERP.Metadata.Models.Query;
using SERP.Framework.Common;
using System;
using NetTopologySuite.Geometries;

namespace Project.Business.Model;

public class BillQueryModel : BaseRequestModel, IListMetadataFilterQuery
{
    public Guid? id_hoa_don { get; set; }
    public Guid? id_nhan_vien { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? id_don_hang { get; set; }
    public Guid? id_phuong_thuc_thanh_toan { get; set; }
    public string? BillCode { get; set; }
    public string? ten_khach_nhan { get; set; }
    public string? email_khach_nhan { get; set; }
    public string? so_dien_thoai_khach_nhan { get; set; }
    public string? dia_chi_nhan { get; set; }
    public decimal? tong_tien { get; set; }
    public decimal? tong_tien_khuyen_mai { get; set; }
    public decimal? tong_tien_sau_khuyen_mai { get; set; }
    public decimal? tong_tien_phai_thanh_toan { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? create_on_date { get; set; }
    public DateTime? last_modifi_on_date { get; set; }
    public string? update_by { get; set; }
    public string? ghi_chu { get; set; }
    public DateTime? ngay_tao { get; set; }
    public DateTime? ngay_thanh_toan { get; set; }
    public decimal? tong_tien_decimal { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
   
}