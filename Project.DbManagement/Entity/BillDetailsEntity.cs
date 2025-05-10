using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class BillDetailsEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string BillDetailCode { get; set; }

        [ForeignKey("BillEntity")]
        public Guid? BillId { get; set; }
        public virtual BillEntity? Bill { get; set; }

        [ForeignKey("ProductEntity")]
        public Guid? ProductId { get; set; }
        public virtual ProductEntity? ProductEntity { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? ProductName { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? ProductImage { get; set; }
        public string ?SKU { get; set; }
        public string Size { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Color { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? Notes { get; set; }
    }
}
