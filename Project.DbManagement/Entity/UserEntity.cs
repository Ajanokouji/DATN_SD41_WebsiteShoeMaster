using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class UserEntity:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public UserTypeEnum Type { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Username { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? AvartarUrl { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Password { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? UserDetailJson { get; set; }

        public bool? IsActive { get; set; } = true;

    }
}
