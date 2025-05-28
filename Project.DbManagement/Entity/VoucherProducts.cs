using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Project.DbManagement.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement
{
    //Voucher sản phẩm là voucher chỉ áp dụng cho 1 số sản phẩm nhất định và
    //những sản phẩm đó sẽ được lưu ở đây
    public class VoucherProducts : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid VoucherId { get; set; }
        public Guid ProductId { get; set; }

        //public string Sku { get; set; }
        public string VarientProductId { get; set; }
    }
}
