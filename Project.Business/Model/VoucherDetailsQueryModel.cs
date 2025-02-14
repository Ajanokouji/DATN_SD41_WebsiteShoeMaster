using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class VoucherDetailsQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? id_giam_gia { get; set; }
        public Guid? id_hoa_don { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }
    }
}
