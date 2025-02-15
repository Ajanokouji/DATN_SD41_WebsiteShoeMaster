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
    public class BillDetails: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string BillDetailCode { get; set; }
        public Guid? BillId { get; set; }
        public Guid? ProductDetailId { get; set; }
        public int Status { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? Notes { get; set; }
    }
}
