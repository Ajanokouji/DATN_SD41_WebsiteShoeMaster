using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class VoucherProductsQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? VoucherId { get; set; }
        public Guid? ProductId { get; set; }
        public string? VarientProductId { get; set; }
    }
}
