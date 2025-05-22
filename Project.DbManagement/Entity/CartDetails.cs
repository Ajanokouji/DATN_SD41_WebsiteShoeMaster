using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Nest;

namespace Project.DbManagement.Entity
{
    public class CartDetails : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid IdCart { get; set; }

        public Guid IdProduct { get; set; }


        public string? SKU { get; set; }
        public string? Size { get; set; }

        public string? Color { get; set; }


        public int? Quantity { get; set; }

        public bool? IsOnSale { get; set; } = false;

        [Column(TypeName = "nvarchar(128)")]
        public string? Code { get; set; }
    }
}
