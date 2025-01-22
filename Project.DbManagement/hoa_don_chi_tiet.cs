using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class hoa_don_chi_tiet
    {
        [Key]
        public Guid id_hoa_don_chi_tiet { get; set; }
        public string ma_hoa_don_chi_tiet { get; set; }
        public Guid id_hoa_don { get; set; }
        public Guid id_san_pham_chi_tiet { get; set; }
        public int trang_thai { get; set; }
        public int so_luong { get; set; }
        public double don_gia { get; set; }
        public string create_by { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }
        public string update_by { get; set; }
        public string? ghi_chu { get; set; }

        [ForeignKey("id_hoa_don")]
        public virtual hoa_don Hoa_Don { get; set; }
    }
}
