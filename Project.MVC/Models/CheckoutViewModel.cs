using Project.Business.Model;
using Project.MVC.Controllers;
using Project.Project.Business.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.MVC.Models
{
    public class CheckoutViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public CustomerInfoModel CustomerInfo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount => TotalAmount - DiscountAmount;
        public string VoucherCode { get; set; }
    }


    
} 