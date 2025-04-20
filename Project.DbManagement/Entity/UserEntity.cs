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
        [Column(TypeName = "nvarchar(50)")]
        public string Type { get; set; }

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

        [NotMapped]
        public virtual List<MetaField> MetadataObj { get; set; }

        public virtual string MetadataJson
        {
            get
            {
                if (MetadataObj != null)
                {
                    return JsonSerializer.Serialize(MetadataObj, JsonSerializerOptionConstants.JavaScriptEncoderOption);
                }

                return null;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MetadataObj = null;
                    return;
                }

                try
                {
                    MetadataObj = JsonSerializer.Deserialize<List<MetaField>>(value, JsonSerializerOptionConstants.DefaultOption);
                }
                catch (Exception value2)
                {
                    Console.WriteLine(value2);
                }
            }
        }
    }
}
