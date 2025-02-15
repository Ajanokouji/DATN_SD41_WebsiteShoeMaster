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
    public class BillEntity: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? VoucherId { get; set; }
        public string BillCode { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }
        public double TotalAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double AmountAfterDiscount { get; set; }
        public double AmountToPay { get; set; }
        public int Status { get; set; }
        public int PaymentStatus { get; set; }
        public string UpdateBy { get; set; }
        public string? Notes { get; set; }
    }
}
