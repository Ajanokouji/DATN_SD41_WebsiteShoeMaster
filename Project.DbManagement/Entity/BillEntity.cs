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
    public class BillEntity: BaseEntity
    {
        [Key]
        public Guid id_hoa_don { get; set; }
        public Guid? id_nhan_vien { get; set; }
        public Guid? id_khach_hang { get; set; }
        public Guid? id_don_hang { get; set; }
        public Guid? id_phuong_thuc_thanh_toan { get; set; }
        public string ma_hoa_don { get; set; }
        public string ten_khach_nhan { get; set; }
        public string email_khach_nhan { get; set; }
        public string so_dien_thoai_khach_nhan { get; set; }
        public string dia_chi_nhan { get; set; }
        public double tong_tien { get; set; }
        public double tong_tien_khuyen_mai { get; set; }
        public double tong_tien_sau_khuyen_mai { get; set; }
        public double tong_tien_phai_thanh_toan { get; set; }
        public int trang_thai { get; set; }
        public int trang_thai_thanh_toan { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }
        public string update_by { get; set; }
        public string? ghi_chu { get; set; }

        public virtual ICollection<VoucherDetails> Chi_Tiet_Giam_Gias { get; set; }
        public virtual ICollection<BillDetails> Hoa_Don_Chi_Tiets { get; set; }

        //[ForeignKey("id_nhan_vien")]
        //public virtual nhan_vien Nhan_Vien { get; set; }

        //[ForeignKey("id_khach_hang")]
        //public virtual Customers Khach_Hang { get; set; }

        //[ForeignKey("id_don_hang")]
        //public virtual don_hang Don_Hang { get; set; }

        //[ForeignKey("id_phuong_thuc_thanh_toan")]
        //public virtual PaymentMethods Phuong_Thuc_Thanh_Toan { get; set; }
    }
}
