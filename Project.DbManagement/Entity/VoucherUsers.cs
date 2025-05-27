using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    //VoucherUsers là bảng lưu thông tin những user được sử dụng Voucher không công khai nào đó
    public class VoucherUsers : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid VoucherId { get; set; }
        public Boolean? IsUsed { get; set; } 
        public Guid UserId { get; set; }
    }
}
