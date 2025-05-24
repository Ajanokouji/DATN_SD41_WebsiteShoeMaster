using Newtonsoft.Json;
using Project.DbManagement.Entity;
using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class ProductPatchModel 
    {
        public Guid Id { get; set; }
      
        public string? Code { get; set; }

        public string? Name { get; set; }
  
        public string? Status { get; set; }

        public string? ImageUrl { get; set; }


        public string? SortOrder { get; set; }


        public string? Description { get; set; }

        public Guid? MainCategoryId { get; set; }

        [NotMapped]
        public List<Variant>? VariantObjs { get; set; }

    
        public string? VariantJson {get;set;}

        [NotMapped]
        public List<string>? MediaObjs { get; set; }

        public string? MediasJson { get; set; }

        [NotMapped]
        public List<Guid>? RelatedObjectIds { get; set; }

        public string? RelatedIds
        {
            get;set;
        }

        public string? WorkFlowStates { get; set; }

        public DateTime? PublicOnDate { get; set; }

        [NotMapped]
        public virtual List<MetaField>? MetadataObj { get; set; }


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

       
        public string? CompleteName { get; set; }


        public string? CompletePath { get; set; }

     
        public string? CompleteCode { get; set; }

    }
}
