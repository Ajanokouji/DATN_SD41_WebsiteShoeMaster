using SERP.Framework.Common;

namespace Project.Business.Interface.Repositories
{
    public class ProductCategoriesRelationQueryModel : PaginationRequest
    {
        public Guid? Id { get; set; }

        public Guid? IdSanPham { get; set; }
        public Guid? IdDanhMuc { get; set; }
        public string? TenSanPham { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? RelationType { get; set; }
        public int? Order { get; set; } = 9999;

        public bool? IsPublish { get; set; }
        public DateTime? PublishStartDate { get; set; }
        public DateTime? PublishEndDate { get; set; }
        public DateTime? PublishOnDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
    }
}