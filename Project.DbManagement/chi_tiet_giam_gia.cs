using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class chi_tiet_giam_gia
    {
        public Guid id_giam_gia { get; set; }
        public Guid id_hoa_don { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }

        [ForeignKey("id_giam_gia")]
        public virtual giam_gia Giam_Gia { get; set; }

        [ForeignKey("id_hoa_don")]
        public virtual hoa_don Hoa_Don { get; set; }
    }
}
