using System;

namespace Project.Business.Model
{
    public class CartItemModel
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductCode { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int DiscountPercent { get; set; }
        public bool IsInStock { get; set; }
        public bool IsMax { get; set; }
        public string SKU { get; set; }
        public decimal Total => Price * Quantity;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
} 