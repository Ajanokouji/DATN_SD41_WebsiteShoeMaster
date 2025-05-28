using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Business.Model
{
    public class CheckoutViewModel
    {
        public Guid? BillId { get; set; }
        public CustomerInfoModel? CustomerInfo { get; set; }
        public List<CartItemModel>? CartItems { get; set; }
        public Guid? VoucherId { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        public decimal FinalAmount { get; set; } = 0;

        //public PaymentViewModel?  PaymentViewModel { get; set; }

        public PaymentMethodModel paymentMethodModel { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string? Description { get; set; }
        
        public CheckoutViewModel()
        {
            CustomerInfo = new CustomerInfoModel();
            CartItems = new List<CartItemModel>();

        }
    }
} 