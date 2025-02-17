using Project.DbManagement.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class ProductEntity:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Status { get; set; }
        public string? ImageUrl { get; set; }
        public string? SortOrder { get; set; }
        public string? Description { get; set; }
        public Guid? MainCategoryId { get; set; }
        [NotMapped]
        public List<Guid>? RelatedObjectIds { get; set; }
        public string? RelatedIds { 
            get 
            { 
                if (RelatedObjectIds==null) return null;
                return JsonSerializer.Serialize(RelatedObjectIds);
            } 
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RelatedObjectIds = null;
                    return;
                }
                RelatedObjectIds = JsonSerializer.Deserialize<List<Guid>>(value);
            } }
        public string? WorkFlowStates { get; set; }
        public DateTime? PublicOnDate { get; set; }
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
        public string CompleteName { get; set; }
        public string CompletePath { get; set; }
        public string CompleteCode { get; set; }
        [NotMapped]
        public List<LabelsObj> LabelsObjs { get; set; }
        public virtual string LabelsJson
        {
            get
            {
                if (LabelsObjs == null)
                {
                    return null;
                }

                return JsonSerializer.Serialize(LabelsObjs.Where((LabelsObj x) => x != null).ToList());
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    LabelsObjs = null;
                    return;
                }

                try
                {
                    LabelsObjs = JsonSerializer.Deserialize<List<LabelsObj>>(value);
                }
                catch (Exception)
                {
                    try
                    {
                        List<string> source = JsonSerializer.Deserialize<List<string>>(value);
                        LabelsObjs = source.Select((string x) => new LabelsObj(x, x, x)).ToList();
                    }
                    catch (Exception)
                    {
                        LabelsObjs = null;
                    }
                }
            }
        }
    }
}
