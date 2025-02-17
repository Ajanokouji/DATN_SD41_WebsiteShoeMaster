using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class CartDetails:BaseEntity
    {
        public Guid Id { get; set; }
        public Guid IdCart { get; set; }
        public Guid IdProduct { get; set; }
        public int? Quantity { get; set; }
        public bool? IsOnSale { get; set; } = false;
        public string? Code { get; set; }
    }
}
