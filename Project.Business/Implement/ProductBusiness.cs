using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Project.DbManagement.ViewModels;

namespace Project.Business.Implement
{
    public class ProductBusiness : IProductBusiness
    {
        private readonly IProductRepository _productRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private const string ProductListCacheKey = "ProductList";
        private readonly MemoryCacheEntryOptions _cacheOptions;
        private readonly bool _useCache;

        public ProductBusiness(IProductRepository productRepository, IMemoryCache cache, IConfiguration configuration)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = Log.ForContext<ProductBusiness>();
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            _useCache = _configuration.GetValue<bool>("CacheSettings:UseCache");
        }

        public async Task<ProductEntity> DeleteAsync(Guid contentId)
        {
            try
            {
                if (contentId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid product ID", nameof(contentId));
                }

                var result = await _productRepository.DeleteAsync(contentId);
                if (result != null)
                {
                    if (_useCache)
                    {
                        _cache.Remove(ProductListCacheKey);
                    }
                    _logger.Information("Product {ProductId} deleted successfully", contentId);
                }
                else
                {
                    _logger.Warning("Product {ProductId} not found for deletion", contentId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting product {ProductId}", contentId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                if (deleteIds == null || !deleteIds.Any())
                {
                    throw new ArgumentException("Delete IDs cannot be null or empty", nameof(deleteIds));
                }

                var result = await _productRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    if (_useCache)
                    {
                        _cache.Remove(ProductListCacheKey);
                    }
                    _logger.Information("Multiple products deleted successfully: {ProductIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple products: {ProductIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<ProductEntity> FindAsync(Guid contentId)
        {
            try
            {
                if (contentId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid product ID", nameof(contentId));
                }

                var product = await _productRepository.FindAsync(contentId);
                if (product == null)
                {
                    _logger.Warning("Product {ProductId} not found", contentId);
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding product {ProductId}", contentId);
                throw;
            }
        }

        public async Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                if (_useCache && _cache.TryGetValue(ProductListCacheKey + (queryModel.ParentId ?? Guid.Empty).ToString(), out Pagination<ProductEntity> cachedProducts))
                {
                    _logger.Debug("Retrieved products from cache");
                    return cachedProducts;
                }

                var products = await _productRepository.GetAllAsync(queryModel);
                if (_useCache)
                {
                    _cache.Set(ProductListCacheKey, products, _cacheOptions);
                    _logger.Debug("Products cached successfully");
                }
                return products;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<int> GetCountAsync(ProductQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _productRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting product count");
                throw;
            }
        }

        public async Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _productRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all products");
                throw;
            }
        }

        public async Task<IEnumerable<ProductEntity>> ListByIdsAsync(List<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    throw new ArgumentException("IDs cannot be null or empty", nameof(ids));
                }

                return await _productRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing products by ids: {ProductIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<ProductEntity> PatchAsync(ProductPatchModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                var exist = await _productRepository.FindAsync(model.Id);
                if (exist == null)
                {
                    _logger.Warning("Product {ProductId} not found for update", model.Id);
                    throw new ArgumentException(ProductConstant.ProductNotFound);
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var update = new ProductEntity
                    {
                        Id = exist.Id,
                        MainCategoryId = model.MainCategoryId ?? exist.MainCategoryId,
                        CompleteCode = model.CompleteCode ?? exist.CompleteCode,
                        CompleteName = model.CompleteName ?? exist.CompleteName,
                        CompletePath = model.CompletePath ?? exist.CompletePath,
                        CreatedByUserId = exist.CreatedByUserId,
                        CreatedOnDate = exist.CreatedOnDate,
                        Description = model.Description ?? exist.Description,
                        ImageUrl = model.ImageUrl ?? exist.ImageUrl,
                        IsDeleted = exist.IsDeleted,
                        //LabelsJson = model.LabelsJson ?? exist.LabelsJson,
                        //LabelsObjs = model.LabelsObjs ?? exist.LabelsObjs,
                        LastModifiedByUserId = exist.LastModifiedByUserId,
                        LastModifiedOnDate = DateTime.UtcNow,
                        Code = model.Code ?? exist.Code,
                        MetadataObj = model.MetadataObj ?? exist.MetadataObj,
                        PublicOnDate = model.PublicOnDate ?? exist.PublicOnDate,
                        RelatedIds = model.RelatedIds ?? exist.RelatedIds,
                        RelatedObjectIds = model.RelatedObjectIds ?? exist.RelatedObjectIds,
                        SortOrder = model.SortOrder ?? exist.SortOrder,
                        Status = model.Status ?? exist.Status,
                        Name = model.Name ?? exist.Name,
                        VariantObjs = model.VariantObjs ?? exist.VariantObjs,
                        WorkFlowStates = model.WorkFlowStates ?? exist.WorkFlowStates
                    };

                    var result = await SaveAsync(update);
                    if (result != null)
                    {
                        if (_useCache)
                        {
                            _cache.Remove(ProductListCacheKey);
                        }
                        _logger.Information("Product {ProductId} updated successfully", model.Id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating product {ProductId}", model?.Id);
                throw;
            }
        }

        public async Task<ProductEntity> SaveAsync(ProductEntity productEntity)
        {
            try
            {
                if (productEntity == null)
                {
                    throw new ArgumentNullException(nameof(productEntity));
                }

                var result = await SaveAsync(new[] { productEntity });
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving product {ProductId}", productEntity?.Id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntities)
        {
            try
            {
                if (productEntities == null || !productEntities.Any())
                {
                    throw new ArgumentException("Product entities cannot be null or empty", nameof(productEntities));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _productRepository.SaveAsync(productEntities);
                    if (result != null && result.Any())
                    {
                        if (_useCache)
                        {
                            _cache.Remove(ProductListCacheKey);
                        }
                        _logger.Information("Multiple products saved successfully: {ProductIds}",
                            string.Join(", ", result.Select(p => p.Id)));
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving multiple products");
                throw;
            }
        }
        public Task<List<ListProductSellViewModel>> GetAllProduct()
        {
            return _productRepository.GetAllProduct();
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsById(Guid idprd)
        {
            return await _productRepository.GetProductDetailsById(idprd); 
        }

        public async Task<List<ListProductDetailsViewModel>> ListProductDetailsById(Guid idprd)
        {
            return await _productRepository.ListProductDetailsById(idprd);
        }

        public Task<List<ListProductSellViewModel>> SearchProduct(string keyword)
        {
            return _productRepository.SearchProduct(keyword);
        }



        public async Task<ProductEntity> PatchVariantStockBySKUAsync(Guid productId, Variant variant)
        {
            var exist = await _productRepository.FindAsync(productId);

            if (exist == null)
            {
                throw new Exception("Product Not Founded");
            }

            if (exist.VariantObjs == null || !exist.VariantObjs.Any())
            {
                throw new Exception("No variants found for this product.");
            }

            var updateVariant = exist.VariantObjs.FirstOrDefault(x => x.Sku == variant.Sku);
            if (updateVariant == null)
            {
                throw new Exception("Variant Not Found");
            }

            // Cập nhật các thuộc tính của variant tìm được bằng variant truyền vào
            //updateVariant.Price = variant.Price;
            //updateVariant.Size = variant.Size;
            updateVariant.Stock = variant.Stock;
            //updateVariant.Group1 = variant.Group1;
            //updateVariant.Group2 = variant.Group2;
            //updateVariant.ImgUrl = variant.ImgUrl;

            // Lưu lại product đã cập nhật variant
            var existProduct = AutoMapperUtils.AutoMap<ProductEntity, ProductPatchModel>(exist);
            var result = await PatchAsync(existProduct);
            return result;
        }
    }
}
