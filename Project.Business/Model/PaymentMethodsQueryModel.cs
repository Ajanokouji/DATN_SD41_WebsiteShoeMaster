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
    public class PaymentMethodsQueryModel: BaseRequestModel, IListMetadataFilterQuery
    {
        public string? ma_phuong_thuc_thanh_toan { get; set; }
        public string? ten_phuong_thuc_thanh_toan { get; set; }
        public int? trang_thai { get; set; }
        public string? create_by { get; set; }
        public DateTime? create_on_date { get; set; }
        public DateTime? last_modifi_on_date { get; set; }
        public string? update_by { get; set; }
    }
}