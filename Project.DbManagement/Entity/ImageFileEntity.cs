using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class ImageFileEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? FileName { get; set; } = null!;           // Tên file gốc
        public string? FilePath { get; set; } = null!;           // Đường dẫn lưu trên server hoặc cloud
        public string? CompleteFilePath { get; set; } = null!; // Đường dẫn đầy đủ (bao gồm tên file)
        public string? ContentType { get; set; } = null!;        // Kiểu MIME (ví dụ: image/jpeg, image/png)
        public long? FileSize { get; set; }                      // Kích thước file (bytes)
        public string? UploadedBy { get; set; }
    }

}
