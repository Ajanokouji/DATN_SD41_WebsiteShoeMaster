using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using Project.DbManagement.Entity;

namespace Project.DbManagement
{
    public class PaymentMethods: BaseEntity
    {
        [Key]
        public Guid id_phuong_thuc_thanh_toan { get; set; }
        public string ma_phuong_thuc_thanh_toan { get; set; }
        public string ten_phuong_thuc_thanh_toan { get; set; }
        public int trang_thai { get; set; }
        public string create_by { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }
        public string update_by { get; set; }

        public virtual ICollection<BillEntity> Hoa_Dons { get; set; }
        //public virtual ICollection<don_hang> Don_Hangs { get; set; }
    }
}
