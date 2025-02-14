using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class CategoriesQueryModel: BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public int SortOrder { get; set; }
        public string Type { get; set; }
        public string ParentPath { get; set; }
        public string CompleteCode { get; set; }
        public string CompleteName { get; set; }
        public string CompletePath { get; set; }
    }
}
