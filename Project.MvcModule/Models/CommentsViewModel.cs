using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Models
{
    public class CommentsViewModel
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? Username { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string? Ref { get; set; } 
        public string? SellerReply { get; set; } 
        public string FormattedDate => CreatedOnDate?.ToString("yyyy-MM-dd HH:mm") ?? "";
    }
}
