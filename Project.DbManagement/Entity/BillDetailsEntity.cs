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
    public class BillDetailsEntity: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string BillDetailCode { get; set; }
        [ForeignKey("BillEntity")]
        public Guid? BillId { get; set; }
        public virtual BillEntity? Bill { get; set; }
        [ForeignKey("ProductEntity")]
        public Guid? ProductId { get; set; }
        public virtual ProductEntity?  ProductEntity { get; set; }
        public int Status { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? Notes { get; set; }
    }
}
