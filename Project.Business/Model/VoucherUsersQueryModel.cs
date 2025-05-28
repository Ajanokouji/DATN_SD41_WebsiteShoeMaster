using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class VoucherUsersQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? VoucherId { get; set; }
        public Guid? UserId { get; set; }
        public Boolean? IsUsed { get; set; }
    }
}
