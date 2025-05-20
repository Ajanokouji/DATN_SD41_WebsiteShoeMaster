using Project.Business.Model.VnPayments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Business.Model
{
    public class PaymentViewModel
    {
        public Guid BillId { get; set; }
        
        [Display(Name = "Số tiền thanh toán")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Phương thức thanh toán")]
        public string SelectedPaymentMethod { get; set; }
        
        public PaymentInformationModel PaymentInformationModel { get; set; }

    }

    public class PaymentMethodModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
} 