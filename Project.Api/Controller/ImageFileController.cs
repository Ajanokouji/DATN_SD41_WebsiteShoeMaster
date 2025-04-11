using Microsoft.AspNetCore.Mvc;
using Nest;
using Project.Business.Interface.Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class ImageFileController : ControllerBase
    {
        private readonly IImageFileBusiness _imageFileBusiness;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageFileController(IConfiguration configuration, IImageFileBusiness imageFileBusiness, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration ;
            _imageFileBusiness = imageFileBusiness ?? throw new ArgumentNullException(nameof(imageFileBusiness));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] FileUploadRequestModel  fileUploadRequestModel)
        {
            var storageUrl = _configuration["FileSettings:StorageUrl"];
            Uri baseUri = new Uri(storageUrl.EndsWith("/") ? storageUrl : storageUrl + "/");
            if (fileUploadRequestModel.File == null || fileUploadRequestModel.File.Length == 0)
            {
                return BadRequest("No file uploaded."); 
            }

            try
            {             
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "user-blob");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName =(fileUploadRequestModel.FileName+ Path.GetExtension(fileUploadRequestModel.File.FileName))??fileUploadRequestModel.File.FileName+DateTimeOffset.UtcNow;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUploadRequestModel.File.CopyToAsync(stream);
                }

                var imageFile = new ImageFile
                {
                    Id = Guid.NewGuid(),
                    FileName =fileName,
                    FilePath = filePath,
                    CompleteFilePath = (new Uri(baseUri, fileName)).ToString(),
                    ContentType = fileUploadRequestModel.File.ContentType,
                    FileSize = fileUploadRequestModel.File.Length,
                    UploadedBy = User?.Identity?.Name ?? "Anonymous",
                    CreatedOnDate = DateTime.UtcNow
                };

                var savedImageFile = await _imageFileBusiness.SaveAsync(imageFile);

                return Ok(new { Message = "File uploaded successfully.", ImageFile = savedImageFile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }
            var completePath =$"{Request.Scheme}://{Request.Host}{Request.Path}";
            try
            {
                // Ensure the fileName is sanitized to prevent directory traversal attacks
                fileName = Path.GetFileName(fileName);

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "user-blob", fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }


                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = "application/octet-stream"; // Default MIME type
                var imageFile = await _imageFileBusiness.FindByCompletePathAsync(completePath); // Replace with actual logic to fetch the file record
                if (imageFile != null)
                {
                    contentType = imageFile.ContentType ?? contentType;
                }

                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetImageFileById(Guid id)
        {
            try
            {
                var imageFile = await _imageFileBusiness.FindAsync(id);
                if (imageFile == null)
                {
                    return NotFound("File not found.");
                }

                return Ok(imageFile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAllImageFiles()
        {
            try
            {
                var imageFiles = await _imageFileBusiness.ListAllAsync();
                return Ok(imageFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteImageFile(Guid id)
        {
            try
            {
                var deletedImageFile = await _imageFileBusiness.DeleteAsync(id);
                if (deletedImageFile == null)
                {
                    return NotFound("File not found.");
                }

                // Optionally, delete the physical file from the server
                var filePath = deletedImageFile.FilePath;
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Ok(new { Message = "File deleted successfully.", ImageFile = deletedImageFile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
