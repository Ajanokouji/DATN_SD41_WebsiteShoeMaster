using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Project.Business.Interface.Project.Business.Interface.Repositories;
using Project.Business.Interface.Project.Business.Interface;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using Project.Business.Model;

namespace Project.Business.Implement
{
    public class ImageFileBusiness : IImageFileBusiness
    {
        private readonly IImageFileRepository _imageFileRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private const string ImageFileListCacheKey = "ImageFileList";
        private readonly MemoryCacheEntryOptions _cacheOptions;
        private readonly bool _useCache;

        public ImageFileBusiness(IImageFileRepository imageFileRepository, IMemoryCache cache, IConfiguration configuration)
        {
            _imageFileRepository = imageFileRepository ?? throw new ArgumentNullException(nameof(imageFileRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = Log.ForContext<ImageFileBusiness>();
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            _useCache = _configuration.GetValue<bool>("CacheSettings:UseCache");
        }

        public async Task<ImageFileEntity> FindAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid image file ID", nameof(id));
                }

                var imageFile = await _imageFileRepository.FindAsync(id);
                if (imageFile == null)
                {
                    _logger.Warning("ImageFile {ImageFileId} not found", id);
                }
                return imageFile;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding image file {ImageFileId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ImageFileEntity>> ListAllAsync(ImageFileQueryModel imageFileQueryModel)
        {
            try
            {
                if (_useCache && _cache.TryGetValue(ImageFileListCacheKey, out IEnumerable<ImageFileEntity> cachedImageFiles))
                {
                    _logger.Debug("Retrieved image files from cache");
                    return cachedImageFiles;
                }

                var imageFiles = await _imageFileRepository.ListAllAsync(imageFileQueryModel);
                if (_useCache)
                {
                    _cache.Set(ImageFileListCacheKey, imageFiles, _cacheOptions);
                    _logger.Debug("Image files cached successfully");
                }
                return imageFiles;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all image files");
                throw;
            }
        }

        public async Task<ImageFileEntity> SaveAsync(ImageFileEntity imageFile)
        {
            try
            {
                if (imageFile == null)
                {
                    throw new ArgumentNullException(nameof(imageFile));
                }

                var result = await _imageFileRepository.SaveAsync(imageFile);
                if (_useCache)
                {
                    _cache.Remove(ImageFileListCacheKey);
                }
                _logger.Information("ImageFile {ImageFileId} saved successfully", imageFile.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving image file {ImageFileId}", imageFile?.Id);
                throw;
            }
        }

        public async Task<ImageFileEntity> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid image file ID", nameof(id));
                }

                var result = await _imageFileRepository.DeleteAsync(id);
                if (result != null)
                {
                    if (_useCache)
                    {
                        _cache.Remove(ImageFileListCacheKey);
                    }
                    _logger.Information("ImageFile {ImageFileId} deleted successfully", id);
                }
                else
                {
                    _logger.Warning("ImageFile {ImageFileId} not found for deletion", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting image file {ImageFileId}", id);
                throw;
            }
        }

        public async Task<ImageFileEntity> FindByCompletePathAsync(string completePath)
        {
            var res = await  _imageFileRepository.FindByCompletePathAsync(completePath);
            return res;
        }

        public async Task<Pagination<ImageFileEntity>> GetAllAsync(ImageFileQueryModel imageFileQueryModel)
        {
            var res = await _imageFileRepository.GetAllAsync(imageFileQueryModel);
            return res;
        }
    }

}
