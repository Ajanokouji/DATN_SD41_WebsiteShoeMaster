using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class ImageFile : BaseEntity
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? FileName { get; set; } = null!;           // Tên file gốc

        [Column(TypeName = "nvarchar(512)")]
        public string? FilePath { get; set; } = null!;           // Đường dẫn lưu trên server hoặc cloud

        [Column(TypeName = "nvarchar(1024)")]
        public string? CompleteFilePath { get; set; } = null!;   // Đường dẫn đầy đủ (bao gồm tên file)

        [Column(TypeName = "nvarchar(128)")]
        public string? ContentType { get; set; } = null!;        // Kiểu MIME (ví dụ: image/jpeg, image/png)

        public long? FileSize { get; set; }                      // Kích thước file (bytes)

        [Column(TypeName = "nvarchar(256)")]
        public string? UploadedBy { get; set; }                  // Người upload (nếu có)

        public bool? Isdeleted { get; set; } = false;
    }
}
