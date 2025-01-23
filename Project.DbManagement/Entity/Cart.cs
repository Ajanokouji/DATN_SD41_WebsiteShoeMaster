using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{   public class  Cart:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdTaiKhoan { get; set; }
       public Guid IdThongTinLienHe { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
    }
}
