using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Business.Model
{
    public class CheckoutViewModel
    {
        public CustomerInfoModel CustomerInfo { get; set; }
        public List<CartItemModel> CartItems { get; set; }
        public string PaymentMethod { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        
        public CheckoutViewModel()
        {
            CustomerInfo = new CustomerInfoModel();
            CartItems = new List<CartItemModel>();
            PaymentMethod = "COD";
        }
    }
} 