using SERP.Framework.Common;

namespace Project.Business.Model
{
    public class ImageFileQueryModel : PaginationRequest
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; } = null!;           // Tên file gốc
        public string? FilePath { get; set; } = null!;           // Đường dẫn lưu trên server hoặc cloud
        public string? CompleteFilePath { get; set; } = null!; // Đường dẫn đầy đủ (bao gồm tên file)
        public string? ContentType { get; set; } = null!;        // Kiểu MIME (ví dụ: image/jpeg, image/png)
        public long? FileSize { get; set; }                      // Kích thước file (bytes)
        public string? UploadedBy { get; set; }
    }
}
