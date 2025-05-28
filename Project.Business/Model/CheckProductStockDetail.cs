using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class CheckProductStockDetail
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}
