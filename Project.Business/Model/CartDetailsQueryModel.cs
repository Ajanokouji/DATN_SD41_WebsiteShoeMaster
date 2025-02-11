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
    public class CartDetailsQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? IdGioHang { get; set; }
        public Guid? IdSanPham { get; set; }
        public int? Quantity { get; set; }
        public bool? IsOnSale { get; set; } = false;
        public string? Code { get; set; }
    }
}
