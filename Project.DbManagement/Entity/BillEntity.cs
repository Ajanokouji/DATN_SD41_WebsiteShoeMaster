using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class BillEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? PaymentMethodId { get; set; }

        public Guid? VoucherId { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string? BillCode { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? RecipientName { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? RecipientEmail { get; set; }

        [Column(TypeName = "nvarchar(15)")]
        public string? RecipientPhone { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? RecipientAddress { get; set; }

        public decimal? TotalAmount { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? Note { get; set; }

        public decimal? DiscountAmount { get; set; }
        public Source  Source{ get;set; }

        public decimal? AmountAfterDiscount { get; set; }

        public decimal? AmountToPay { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string? VoucherCode { get; set; }

        public decimal? FinalAmount { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? Status { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? PaymentStatus { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string? PaymentMethod { get; set; }
    }
}
