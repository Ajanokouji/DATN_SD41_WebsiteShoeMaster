using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model.PatchModel
{
    public class BillPatchModel : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? VoucherId { get; set; }
        public string? BillCode { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Note { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? AmountAfterDiscount { get; set; }
        public decimal? AmountToPay { get; set; }
        public string? VoucherCode { get; set; }

        public decimal? FinalAmount { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? UpdateBy { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
