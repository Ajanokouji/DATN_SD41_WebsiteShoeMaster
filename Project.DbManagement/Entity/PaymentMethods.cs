using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using Project.DbManagement.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement
{
    public class PaymentMethods: BaseEntity
    {
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string PaymentMethodCode { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string PaymentMethodName { get; set; }

        public int Status { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string UpdatedBy { get; set; }
    }
}
