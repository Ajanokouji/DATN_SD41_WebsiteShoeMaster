using System;
using System.Collections.Generic;
using Project.Common;
using SERP.Framework.Common;
using SERP.Framework.Entities.Enums;
using SERP.Metadata.Models.Query;

namespace SERP.NewsMng.Business.Models.QueryModels
{
    public class ContentBaseQueryModel : BaseRequestModel, IListMetadataFilterQuery
    {
        public Guid? ParentId { get; set; }  // Lọc theo ParentId
        public string? Title { get; set; }  // Lọc theo tiêu đề
        public string? SeoUri { get; set; }  // Lọc theo SEO URI
        public bool? IsPublish { get; set; }  // Lọc theo trạng thái xuất bản
        public bool? IsSearchable { get; set; }  // Lọc theo khả năng tìm kiếm
        public StatusEnum? StatusObj { get; set; }  // Lọc theo trạng thái nội dung
        public bool? IsDeleted { get; set; }  // Lọc theo trạng thái xóa mềm
        public string? Type { get; set; }  // Lọc theo loại nội dung
        public DateTime? PublishStartDate { get; set; }  // Lọc theo ngày bắt đầu xuất bản
        public DateTime? PublishEndDate { get; set; }  // Lọc theo ngày kết thúc xuất bản
        public int? Order { get; set; }  // Lọc theo thứ tự sắp xếp
        public List<Guid>? OwnerIds { get; set; }  // Lọc theo danh sách OwnerId (nếu cần)
    }
}
