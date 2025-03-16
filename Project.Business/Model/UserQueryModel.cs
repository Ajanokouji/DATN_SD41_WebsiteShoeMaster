using Project.Common;
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
        public string? Type { get; set; }
    }
}
