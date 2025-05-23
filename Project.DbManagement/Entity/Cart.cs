using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement.Entity
{
    public class Cart : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid IdUser { get; set; }

        public Guid? IdContact { get; set; }

        public int? Status { get; set; }

        [Column(TypeName = "nvarchar(1024)")]
        public string? Description { get; set; }
    }
}
