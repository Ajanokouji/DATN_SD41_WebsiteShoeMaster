using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class thong_tin_lien_he
    {
        [Key]
        public Guid id { get; set; }
        public string sdt { get; set; }
        public string ho { get; set; }
        public string ten_dem { get; set; }
    }
}
