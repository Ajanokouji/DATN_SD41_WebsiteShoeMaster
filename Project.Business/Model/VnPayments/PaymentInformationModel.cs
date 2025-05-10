using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model.VnPayments
{
    public class PaymentInformationModel
    {
        public int? OrderType { get; set; } = 1;
        public double? Amount { get; set; }
        public Guid? BillId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? OrderDescription { get; set; }
        public string? CustomerName { get; set; }
        public string? PayMethod { get; set; }

    }
}
