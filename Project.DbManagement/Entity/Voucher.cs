using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Project.DbManagement.Entity;

namespace Project.DbManagement
{
    public class Voucher: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string VoucherName { get; set; }
        public int? VoucherType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public int? DiscountAmount { get; set; }
        public string? Description { get; set; }

        public int? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get;set; }

    }
}
