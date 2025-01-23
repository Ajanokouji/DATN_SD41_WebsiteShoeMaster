using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Project.DbManagement.Entity;

namespace Project.DbManagement
{
    public class Voucher: BaseEntity
    {
        [Key]
        public Guid id_giam_gia { get; set; }
        public string ten_giam_gia { get; set; }
        public int loai_giam_gia { get; set; }
        public DateTime thoi_gian_bat_dau { get; set; }
        public DateTime thoi_gian_ket_thuc { get; set; }
        public int trang_thai { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }

        public virtual ICollection<VoucherDetails> Chi_Tiet_Giam_Gias { get; set; }
    }
}
