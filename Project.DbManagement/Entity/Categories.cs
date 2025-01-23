using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class Categories:BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public int SortOrder { get; set; }
        public string Type { get; set; }
        public string ParentPath { get; set; }
        public string Code { get; set; }
        public string CompleteCode { get; set; }
        public string CompleteName { get; set; }
        public string CompletePath { get; set; }
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
