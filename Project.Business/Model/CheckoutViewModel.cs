using System;
using System.Collections.Generic;

namespace Project.Business.Model
{
    public class CheckoutViewModel
    {
        public CustomerInfoModel CustomerInfo { get; set; }
        public List<CartItemModel> CartItems { get; set; }
        public string PaymentMethod { get; set; }
        public decimal SubTotal => CartItems?.Sum(x => x.Total) ?? 0;
        public decimal Total => SubTotal;
    }
} 