using Project.Common;
using SERP.Framework.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class ProductQueryModel: BaseRequestModel, IListMetadataFilterQuery
    {
        public string? MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public Guid? DanhMucId { get; set; }
        public string? WorkFlowStates { get; set; }
        public DateTime? PublicOnDate { get; set; }

    }
}
