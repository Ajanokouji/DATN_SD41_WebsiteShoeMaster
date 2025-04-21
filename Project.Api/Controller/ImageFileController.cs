using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Interface.Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;


namespace Project.Api.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class ImageFileController : BaseControllerApi
    {
        private readonly IImageFileBusiness _imageFileBusiness;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageFileController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IImageFileBusiness imageFileBusiness, IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger) : base(httpRequestHelper, logger)
        {
            _configuration = configuration;
            _imageFileBusiness = imageFileBusiness ?? throw new ArgumentNullException(nameof(imageFileBusiness));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(ResponseObject<ImageFileEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadImage([FromForm] FileUploadRequestModel fileUploadRequestModel)
        {
            return await ExecuteFunction(async () =>
            {
                var storageUrl = _configuration["FileSettings:StorageUrl"];
                Uri baseUri = new Uri(storageUrl.EndsWith("/") ? storageUrl : storageUrl + "/");
                if (fileUploadRequestModel.File == null || fileUploadRequestModel.File.Length == 0)
                {
                   throw new Exception("File is required.");
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "user-blob");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(fileUploadRequestModel.File.FileName);
                string fileName;
                if (!string.IsNullOrEmpty(fileUploadRequestModel.FileName))
                {
                    fileName = fileUploadRequestModel.FileName + extension;
                }
                else
                {
                    fileName = Path.GetFileNameWithoutExtension(fileUploadRequestModel.File.FileName)
                        + "_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + extension;
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUploadRequestModel.File.CopyToAsync(stream);
                }

                ImageFileEntity imageFile = new ImageFileEntity
                {
                    Id = Guid.NewGuid(),
                    FileName = fileName,
                    FilePath = filePath,
                    CompleteFilePath = (new Uri(baseUri, fileName)).ToString(),
                    ContentType = fileUploadRequestModel.File.ContentType,
                    FileSize = fileUploadRequestModel.File.Length,
                    UploadedBy = User?.Identity?.Name ?? "Anonymous",
                    CreatedOnDate = DateTime.UtcNow
                };
                var res=  await _imageFileBusiness.SaveAsync(imageFile);
                return res;
            });
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

        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponsePagination<ImageFileEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImageFiles([FromBody] ImageFileQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                try
                {
                    var imageFiles = await _imageFileBusiness.GetAllAsync(queryModel);
                    return imageFiles;
                }
                catch (Exception ex)
                {
                    throw new Exception ($"Internal server error: {ex.Message}");
                }
            });
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteImageFile(Guid id)
        {
            return await ExecuteFunction(async () =>
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
            });
        }
    }
}
