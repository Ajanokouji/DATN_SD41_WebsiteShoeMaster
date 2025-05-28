namespace Project.MVC.Models
{
    public class AppliedVoucher
    {
        public string VoucherCode { get; set; } 
        public Guid VoucherId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAfterDiscount { get; set; }
    }
}
