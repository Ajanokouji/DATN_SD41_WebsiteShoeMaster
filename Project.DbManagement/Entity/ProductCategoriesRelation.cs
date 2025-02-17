using Newtonsoft.Json;
using Project.DbManagement.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class ProductCategoriesRelation : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid IdProduct { get; set; }
        public Guid IdCategory { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string RelationType { get; set; }
        public int? Order { get; set; } = 9999;

        public bool? IsPublish { get; set; }
        public DateTime? PublishStartDate { get; set; }
        public DateTime? PublishEndDate { get; set; }
        public DateTime? PublishOnDate { get; set; }
        public string? Status { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public virtual RelatedObj RelatedObj { get; set; }

        public virtual string RelatedObjJson
        {
            get
            {
                return RelatedObj == null ? null : JsonConvert.SerializeObject(RelatedObj);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    RelatedObj = null;
                else
                {
                    try
                    {
                        RelatedObj = JsonConvert.DeserializeObject<RelatedObj>(value);
                    }
                    catch
                    {
                        RelatedObj = null;
                    }
                }
            }
        }

        [NotMapped]
        public virtual RelatedObj RelatedObj2 { get; set; }

        public virtual string RelatedObj2Json
        {
            get
            {
                return RelatedObj2 == null ? null : JsonConvert.SerializeObject(RelatedObj2);
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    RelatedObj2 = null;
                else
                {
                    try
                    {
                        RelatedObj2 = JsonConvert.DeserializeObject<RelatedObj>(value);
                    }
                    catch
                    {
                        RelatedObj2 = null;
                    }
                }
            }
        }

    }
}
