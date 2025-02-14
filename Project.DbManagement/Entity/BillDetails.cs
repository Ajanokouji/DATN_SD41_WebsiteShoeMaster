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
    public class BillDetails: BaseEntity
    {
        [Key]
        public Guid id_hoa_don_chi_tiet { get; set; }
        public string ma_hoa_don_chi_tiet { get; set; }
        public Guid? id_hoa_don { get; set; }
        public Guid? id_san_pham_chi_tiet { get; set; }
        public int trang_thai { get; set; }
        public int so_luong { get; set; }
        public double don_gia { get; set; }
        public string? ghi_chu { get; set; }

        [ForeignKey("id_hoa_don")]
        public virtual BillEntity Hoa_Don { get; set; }
    }
}
