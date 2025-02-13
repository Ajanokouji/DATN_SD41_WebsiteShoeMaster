using Project.Common;
using SERP.Metadata.Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
   public class UserQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public string? Type { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
        public string? AvartarUrl { get; set; }
        public string? Password { get; set; }
        public string? UserDetailJson { get; set; }
    }
}
