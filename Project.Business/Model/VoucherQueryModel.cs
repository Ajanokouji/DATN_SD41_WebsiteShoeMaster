using Project.Common;
using Project.DbManagement.Enum;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class VoucherQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? id_giam_gia { get; set; }
        public string? ten_giam_gia { get; set; }
        public VocherTypeEnum? loai_giam_gia { get; set; }
        public DateTime? thoi_gian_bat_dau { get; set; }
        public DateTime? thoi_gian_ket_thuc { get; set; }
        public int? trang_thai { get; set; }
        public DateTime? create_on_date { get; set; }
        public DateTime? last_modifi_on_date { get; set; }
        public decimal? DiscountAmount { get; set; }

        public string? Description { get; set; }

        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }

        //Tổng lượt sử dụng tối đa
        public int? TotalMaxUsage { get; set; }
        //Số lượt sử dụng tối đa trên mỗi khách hàng
        public int? MaxUsagePerCustomer { get; set; }
        //Thiết lập hiển thị: Hiển thị nhiều nơi | Chia sẻ thông qua mã voucher
        public int? DisplaySettings { get; set; }
        //Số tiền giảm giá tối đa đối với voucher giảm theo phần trăm
        public decimal? MaxDiscountAmount { get; set; }
        //Tổng Số lượt đã sử dụng voucher
        public int? RedeemCount { get; set; }
        //Lọc theo trạng thái tổng: Gọi là trạng thái tổng vì nó dựa trên cả startDate, EndDate và Status của voucher
        public int? StatusTotal { get; set; }
    }
}
