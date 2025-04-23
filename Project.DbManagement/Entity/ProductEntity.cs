using Project.DbManagement.Extension;
using SERP.Framework.Entities.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Project.Common;

namespace Project.DbManagement.Entity
{
    public class ProductEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Code { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Status { get; set; }

        public string? ImageUrl { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? SortOrder { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        public Guid? MainCategoryId { get; set; }

        [NotMapped]
        public List<Variant>? VariantObjs { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? VariantJson
        {
            get
            {
                if (VariantObjs == null) return null;
                return JsonSerializer.Serialize(VariantObjs);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    VariantObjs = null;
                    return;
                }
                VariantObjs = JsonSerializer.Deserialize<List<Variant>>(value);
            }
        }

        [NotMapped]
        public List<string>? MediaObjs { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? MediasJson
        {
            get
            {
                if (MediaObjs == null) return null;
                return JsonSerializer.Serialize(MediaObjs);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MediaObjs = null;
                    return;
                }
                MediaObjs = JsonSerializer.Deserialize<List<string>>(value);
            }
        }

        [NotMapped]
        public List<Guid>? RelatedObjectIds { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? RelatedIds
        {
            get
            {
                if (RelatedObjectIds == null) return null;
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
            }
        }

        [Column(TypeName = "nvarchar(max)")]
        public string? WorkFlowStates { get; set; }

        public DateTime? PublicOnDate { get; set; }

        [NotMapped]
        public virtual List<MetaField>? MetadataObj { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public virtual string? MetadataJson
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

        [Column(TypeName = "nvarchar(256)")]
        public string? CompleteName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? CompletePath { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? CompleteCode { get; set; }

        [NotMapped]
        public List<LabelsObj>? LabelsObjs { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public virtual string? LabelsJson
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

    public class Variant
    {
        [Column(TypeName = "nvarchar(50)")]
        public string Id { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string? ProductId { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string? Size { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(50)")]
        public string? SizeType { get; set; } = string.Empty;

        public decimal? LowestAsk { get; set; }
    }
}
