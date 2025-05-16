using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Model
{
    public class UserQueryModel: BaseRequestModel
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AvartarUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public UserTypeEnum? Type { get; set; }
    }
}
