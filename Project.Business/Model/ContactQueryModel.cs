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
    public class ContactQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? Id { get; set; }
        public string Ten { get; set; }
        public string? TenDayDu { get; set; }
        public string? DiaChi { get; set; }
        public string? NgaySinh { get; set; }
        public string? ImageUrl { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string NoiDung { get; set; }
    }
}
