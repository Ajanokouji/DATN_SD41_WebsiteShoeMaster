using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Business.Model
{
    public class BillModel: BillEntity

    {
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }


        public List<BillDetailModel>? BillDetails { get; set; } = new List<BillDetailModel>();
    }

    public class BillDetailModel  :BaseEntity
    {
        public Guid Id { get; set; }
        public string BillDetailCode { get; set; }
        public Guid? BillId { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public string? Size { get; set; }
        public string? SKU { get; set; }
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
} 