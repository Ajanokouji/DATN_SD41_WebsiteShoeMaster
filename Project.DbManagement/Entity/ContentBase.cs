using System;
using System.ComponentModel.DataAnnotations;

namespace Project.DbManagement.Entity
{
    public class ContentBase : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }  // Tiêu đề nội dung
        public string SeoUri { get; set; }  // Đường dẫn SEO của nội dung
        public string SeoTitle { get; set; }  // Tiêu đề SEO
        public string SeoDescription { get; set; }  // Mô tả SEO
        public string SeoKeywords { get; set; }  // Từ khóa SEO

        public DateTime PublishStartDate { get; set; }  // Ngày bắt đầu xuất bản
        public DateTime PublishEndDate { get; set; }  // Ngày kết thúc xuất bản

        public bool IsPublish { get; set; }  // Trạng thái xuất bản (true/false)
        public bool IsDeleted { get; set; }  // Trạng thái đã xóa hay chưa (logical delete)

        public DateTime CreatedOnDate { get; set; }  // Ngày tạo
        public DateTime LastModifiedOnDate { get; set; }  // Ngày sửa đổi lần cuối

        public string CreatedBy { get; set; }  // Người tạo
        public string LastModifiedBy { get; set; }  // Người sửa đổi lần cuối
    }
}
