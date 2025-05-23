using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Model
{
    public class CommentsModel  : PaginationRequest
    {
        public Guid? Id { get; set; }
        public Guid? ObjectId { get; set; }
        public string? Ref { get; set; }
        public string? Message { get; set; }
        public int? Status { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public bool? IsPublish { get; set; }
        public Guid? ParentId { get; set; }
        public int? TolalReply { get; set; }
    }
 }
