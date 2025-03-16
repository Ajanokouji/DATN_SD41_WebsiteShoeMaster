using System;
using System.Collections.Generic;

namespace Project.Business.Model
{
    public class BillModel
    {
        public long Id { get; set; }
        public string BillCode { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string VoucherCode { get; set; }
        public string Note { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Confirmed, 2: Shipping, 3: Completed, 4: Cancelled
        public string PaymentMethod { get; set; } // COD, Banking, VNPay
        public int PaymentStatus { get; set; } // 0: Unpaid, 1: Paid
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<BillDetailModel> BillDetails { get; set; } = new List<BillDetailModel>();
    }

    public class BillDetailModel
    {
        public long Id { get; set; }
        public long BillId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 