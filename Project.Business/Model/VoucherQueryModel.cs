using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class VoucherQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? id_giam_gia { get; set; }
        public string ten_giam_gia { get; set; }
        public int loai_giam_gia { get; set; }
        public DateTime thoi_gian_bat_dau { get; set; }
        public DateTime thoi_gian_ket_thuc { get; set; }
        public int trang_thai { get; set; }
        public DateTime create_on_date { get; set; }
        public DateTime last_modifi_on_date { get; set; }
    }
}
