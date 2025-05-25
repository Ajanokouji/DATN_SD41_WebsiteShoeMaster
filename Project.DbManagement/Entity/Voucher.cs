using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Project.DbManagement.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using Project.DbManagement.Enum;

namespace Project.DbManagement
{
    public class Voucher: BaseEntity
    {
        [Key]
        public Guid Id { get; set; } 

        [Column(TypeName = "nvarchar(50)")]
        public string? Code { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string VoucherName { get; set; }

        public string? ImageUrl { get; set; }

        public VocherTypeEnum? VoucherType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Description { get; set; }

        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }

        //Tổng lượt sử dụng tối đa
        public int? TotalMaxUsage { get; set; }
        //Số lượt sử dụng tối đa trên mỗi khách hàng
        public int? MaxUsagePerCustomer { get; set; }
        //Số tiền giảm giá tối đa đối với voucher giảm theo phần trăm
        public decimal? MaxDiscountAmount { get; set; }
        //Tổng Số lượt đã sử dụng voucher
        public int? RedeemCount { get; set; }
    }
}
