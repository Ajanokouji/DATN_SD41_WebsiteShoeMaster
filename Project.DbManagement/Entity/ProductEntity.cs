using Project.DbManagement.Extension;
using SERP.Framework.Entities.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Project.Common;
using Newtonsoft.Json;

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
                return JsonConvert.SerializeObject(VariantObjs);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    VariantObjs = null;
                    return;
                }
                VariantObjs = JsonConvert.DeserializeObject<List<Variant>>(value);
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
                return JsonConvert.SerializeObject(MediaObjs);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MediaObjs = null;
                    return;
                }
                MediaObjs = JsonConvert.DeserializeObject<List<string>>(value);
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
                return JsonConvert.SerializeObject(RelatedObjectIds);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    RelatedObjectIds = null;
                    return;
                }
                RelatedObjectIds = JsonConvert.DeserializeObject<List<Guid>>(value);
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
                    return JsonConvert.SerializeObject(MetadataObj);
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
                    MetadataObj = JsonConvert.DeserializeObject<List<MetaField>>(value);
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

                return JsonConvert.SerializeObject(LabelsObjs.Where((LabelsObj x) => x != null).ToList());
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
                    LabelsObjs = JsonConvert.DeserializeObject<List<LabelsObj>>(value);
                }
                catch (Exception)
                {
                    try
                    {
                        List<string> source = JsonConvert.DeserializeObject<List<string>>(value);
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
        public string? Sku { get; set; } = string.Empty;
                                                                                                                                                                                                                                                                                       
        public string? ImgUrl { get; set; } = string.Empty;

        public string? Group1 { get; set; } = string.Empty;

        public string? Group2 { get; set; } = string.Empty;

        public string? Size
        {get;set;
        }


        public string? Price { get; set; } = string.Empty;
        public int? Stock { get; set; }

    }
}
