using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement.Entity
{
    [Table("ContentBase")]
    public class ContentBase : BaseEntity
    {

        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Title { get; set; }  // Tiêu đề nội dung
     

        [Column(TypeName = "nvarchar(256)")]
        public string? SeoUri { get; set; }  // Đường dẫn SEO của nội dung

        [Column(TypeName = "nvarchar(256)")]
        public string? SeoTitle { get; set; }  // Tiêu đề SEO

        [Column(TypeName = "nvarchar(512)")]
        public string? SeoDescription { get; set; }  // Mô tả SEO

        [Column(TypeName = "nvarchar(256)")]
        public string? SeoKeywords { get; set; }  // Từ khóa SEO
        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }  // Tiêu đề nội dung

        public DateTime? PublishStartDate { get; set; }  // Ngày bắt đầu xuất bản
        public DateTime? PublishEndDate { get; set; }  // Ngày kết thúc xuất bản

        public bool? IsPublish { get; set; }  // Trạng thái xuất bản (true/false)
    }
}
