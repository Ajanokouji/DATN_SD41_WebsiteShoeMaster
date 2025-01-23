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
    public class User:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string AvartarUrl { get; set; }
        public string Password { get; set; }    
        public string UserDetailJson { get; set; }

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
