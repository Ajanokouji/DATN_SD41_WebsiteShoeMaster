using Project.Project.Business.Model;
using System.ComponentModel.DataAnnotations;

namespace Project.Business.Model
{
    public class PaymentViewModel
    {
        public Guid BillId { get; set; }
        public decimal TotalAmount { get; set; }
        public string VoucherCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; }

        public string SelectedPaymentMethod { get; set; }
        public List<PaymentMethodModel> PaymentMethods { get; set; }
        public CustomerInfoModel CustomerInfo { get; set; }
    }
} 