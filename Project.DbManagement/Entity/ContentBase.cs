using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement.Entity
{
    public class ContentBase : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string Title { get; set; }  // Tiêu đề nội dung

        [Column(TypeName = "nvarchar(256)")]
        public string SeoUri { get; set; }  // Đường dẫn SEO của nội dung

        [Column(TypeName = "nvarchar(256)")]
        public string SeoTitle { get; set; }  // Tiêu đề SEO

        [Column(TypeName = "nvarchar(512)")]
        public string SeoDescription { get; set; }  // Mô tả SEO

        [Column(TypeName = "nvarchar(256)")]
        public string SeoKeywords { get; set; }  // Từ khóa SEO

        public DateTime PublishStartDate { get; set; }  // Ngày bắt đầu xuất bản
        public DateTime PublishEndDate { get; set; }  // Ngày kết thúc xuất bản

        public bool IsPublish { get; set; }  // Trạng thái xuất bản (true/false)

        public DateTime CreatedOnDate { get; set; }  // Ngày tạo
        public DateTime LastModifiedOnDate { get; set; }  // Ngày sửa đổi lần cuối

        [Column(TypeName = "nvarchar(128)")]
        public string CreatedBy { get; set; }  // Người tạo

        [Column(TypeName = "nvarchar(128)")]
        public string LastModifiedBy { get; set; }  // Người sửa đổi lần cuối
    }
}
