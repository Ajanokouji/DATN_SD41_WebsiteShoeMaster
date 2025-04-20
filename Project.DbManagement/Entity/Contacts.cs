using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class Contacts : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? FullName { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }
    }
}
