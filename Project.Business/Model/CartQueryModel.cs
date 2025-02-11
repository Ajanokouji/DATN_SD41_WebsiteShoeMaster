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
    public class CartQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? IdTaiKhoan { get; set; }
        public Guid? IdThongTinLienHe { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
    }
}
