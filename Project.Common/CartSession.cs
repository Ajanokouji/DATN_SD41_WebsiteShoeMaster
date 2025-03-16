using System;
using System.Collections.Generic;

namespace Project.Common
{
    public class CartSession
    {
        public const string CartKey = "CartSession";
        
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public decimal Total { get; set; }
    }
} 